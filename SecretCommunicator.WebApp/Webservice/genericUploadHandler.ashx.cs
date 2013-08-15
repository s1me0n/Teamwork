using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using SecretCommunicator.WebData;
using SecretCommunicator.WebData.Library;
using System.Web.SessionState;
using Spring.IO;
using Spring.Social.OAuth1;
using Cloudinary;
using Newtonsoft.Json;

namespace SecretCommunicator.WebApp.Webservice
{
    /// <summary>
    /// This Generic Handler handles the uploaded file and can be used from multiple places.
    /// </summary>
    public sealed class genericUploadHandler : IHttpHandler, IRequiresSessionState 
    {
        PubnubAPI pubnub = new PubnubAPI(
                "pub-2e41c0cf-3a9d-4d2e-83d2-1d7ae0a12b78",				// PUBLISH_KEY
                "sub-c29f5187-b88e-11e1-9907-d99166562f9b",				// SUBSCRIBE_KEY
                "sec-OWVhZjIwYTMtNDZjYy00NzdkLTk0M2QtZWQzY2U2OTU2MWE1", // SECRET_KEY
                true													// SSL_ON?
                );
        boSessionState _sessionState = new boSessionState();
        RestoreSession _restoreSession = new RestoreSession();
        string channelName = string.Empty;
        string channelPassword = string.Empty;

        public void ProcessRequest(HttpContext context)
        {
            _sessionState.AuthClient();
            Channel chan;
            GetSession(context);
            if (_sessionState.CurrentUser == null)
            {
                chan = getChannel(context);
                if (chan != null)
                {
                    _sessionState.CurrentUser = new User() { ChannelName = new List<string>() };
                    _sessionState.CurrentUser.ChannelName.Add(chan.Name);
                    _sessionState.GetChanMessages(chan, 10);
                    _restoreSession.LastChannel = new List<dynamic>();
                    _restoreSession.LastChannel.Add(chan);
                    _restoreSession.LastChannel.Add(AppCache.MessageList.Where(m => m.ChannelId == chan.Id).Take(10).ToList());
                }
                else
                {
                    context.Response.Write("{ 'success': login to channel pls }");
                    return;
                }
            }
            else if (_restoreSession.LastChannel == null)
            {
                chan = getChannel(context);
                if (chan != null)
                {
                    _sessionState.GetChanMessages(chan, 10);
                    _restoreSession.LastChannel = new List<dynamic>();
                    _restoreSession.LastChannel.Add(chan);
                    _restoreSession.LastChannel.Add(AppCache.MessageList.Where(m => m.ChannelId == chan.Id).Take(10).ToList());
                }
            }


            chan = _restoreSession.LastChannel[0];
            if (chan != null)
            {
                // Set the response return data type
                context.Response.ContentType = "text/html";

                try
                {
                    // First check this header (cross browser support)
                    string uploadFileName = context.Request.Headers["X-File-Name"];

                    if (string.IsNullOrEmpty(uploadFileName) == false || context.Request.Files.Count > 0)
                    {
                        // Get the uploads physical directory on the server
                        string directory = context.Server.MapPath("~/UploadFiles/");
                        if (Functions.DirectoryExist(directory))
                        {
                            //create new message
                            Message msg = new Message();
                            msg.ChannelId = chan.Id;
                            msg.PrivateDateTime = DateTime.Now;
                            msg.PublicData = new MessageResource() { Type = MessageTypes.File };

                            string filename = string.Empty;
                            if (uploadFileName == null)
                            {
                                // get just the original filename
                                filename = System.IO.Path.GetFileName(context.Request.Files[0].FileName);
                            }
                            else
                            {
                                filename = uploadFileName;
                            }



                            msg.PublicData.Type = MessageTypes.File;
                            // create full server path
                            string file = string.Format("{0}\\{1}", directory, filename);

                            // If file exists already, delete it (optional)
                            //if (System.IO.File.Exists(file) == true) System.IO.File.Delete(file);

                            if (string.IsNullOrEmpty(uploadFileName) == true) // IE Browsers
                            {
                                // Save file to server
                                context.Request.Files[0].SaveAs(file);
                                uploadToCloud(filename, uploadFileName, context, msg, chan, file);
                            }
                            else // Other Browsers
                            {
                                // Save file to server
                                System.IO.FileStream fileStream = new System.IO.FileStream(file, System.IO.FileMode.OpenOrCreate);
                                uploadToCloud(filename, uploadFileName, context, msg, chan, file);
                                context.Request.InputStream.CopyTo(fileStream);
                                fileStream.Close();
                            }

                            //save in mongodb
                            msg.PrivateData = AppCache.AESProvider.EncryptToString(JsonConvert.SerializeObject(msg.PublicData));
                            _sessionState.SaveMessage(msg);

                            //send notification in pubnub

                            if (msg.PublicData.Type == MessageTypes.File)
                            {
                                _sessionState.AuthClient();
                                msg.PublicData.Value = _sessionState.Client.GetMediaLinkAsync(msg.PublicData.Value).Result.Url;
                            }

                            PublishToPubNub(chan.Name, msg);

                            // return the json object as successful
                            context.Response.Write("{ 'success': true }");
                            return;
                        }
                    }

                    // return the json object as unsuccessful
                    context.Response.Write("{ 'success': false }");
                }
                catch (Exception)
                {
                    // return the json object as unsuccessful
                    context.Response.Write("{ 'success': false }");
                }
                finally
                {
                    SaveSession(context);
                }
            }
        }

        private void uploadToCloud(string filename, string uploadFileName, HttpContext context, Message msg, Channel chan, string file)
        {
            if (filename.EndsWith("png") || filename.EndsWith("jpeg") || filename.EndsWith("jpg") || filename.EndsWith("gif") || filename.EndsWith("bmp"))
            {
                try
                {
                    msg.PublicData.Type = MessageTypes.Image;
                    var configuration = new AccountConfiguration("saykor", "277334748579534", "mUjzZ-X3jOuNKGswrAjocB-D-Rc");

                    var uploader = new Uploader(configuration);
                    string publicId = Path.GetFileNameWithoutExtension(filename);
                    Stream stream;
                    if (string.IsNullOrEmpty(uploadFileName) == true) // IE Browsers
                        stream = context.Request.Files[0].InputStream;
                    else // Other Browsers
                        stream = context.Request.InputStream;

                    var uploadResult = uploader.Upload(new UploadInformation(filename, stream)
                    {
                        PublicId = publicId,
                        Format = filename.Substring(filename.Length - 3),
                    });
                    msg.PublicData.Value = filename;
                }
                catch (Exception ex)
                {
                    context.Response.Write("{ 'success': " + ex.Message + " }");
                    return;
                }
            }
            else
            {
                //upload to dropbox
                string cloudPath = "/" + chan.Name + "/" + filename;
                _sessionState.AuthClient();
                var result = _sessionState.Client.UploadFileAsync(new FileResource(file), cloudPath).Result;
                msg.PublicData.Value = cloudPath;
            }
        }

        private void PublishToPubNub(string name, Message msg)
        {
            if (msg != null)
            {
                List<dynamic> pubnubMessage = new List<dynamic>();
                Dictionary<string, string> actionDict = new Dictionary<string, string>();
                actionDict.Add("Action", "add");
                actionDict.Add("Channel", name);
                pubnubMessage.Add(actionDict);
                msg.PrivateData = null;
                if (msg.PublicData.Type == MessageTypes.File)
                {
                    _sessionState.AuthClient();
                    msg.PublicData.Value = _sessionState.Client.GetMediaLinkAsync(msg.PublicData.Value).Result.Url;
                }
                pubnubMessage.Add(msg);
                List<object> publishResult = pubnub.Publish("NewMsgIn" + name, pubnubMessage);
            }
        }

        private Channel getChannel(HttpContext context)
        {
            Channel chan;
            if (!string.IsNullOrEmpty(context.Request["channelName"]))
                channelName = context.Request["channelName"];

            if (!string.IsNullOrEmpty(context.Request["channelPassword"]))
                channelPassword = context.Request["channelPassword"];

            chan = _sessionState.GetChannel(channelName, channelPassword, pubnub);
            return chan;
        }

        public bool IsReusable
        {
            get { return false; }
        }

        private void GetSession(HttpContext context)
        {
            if (context.Session["sessionState"] != null)
                _sessionState = (boSessionState)context.Session["sessionState"];

            if (context.Session["restoreSession"] != null)
                _restoreSession = (RestoreSession)context.Session["restoreSession"];
        }

        protected void SaveSession(HttpContext context)
        {
            context.Session["sessionState"] = _sessionState;
            context.Session["restoreSession"] = _restoreSession;
        }
    }
}