using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using Cloudinary;
using SecretCommunicator.Data;
using SecretCommunicator.Models;
using SecretCommunicator.Models.Library;
using Spring.IO;

namespace SecretCommunicator.WebApi.Webservices
{
    /// <summary>
    /// This Generic Handler handles the uploaded file and can be used from multiple places.
    /// </summary>
    public sealed class GenericUploadHandler : IHttpHandler, IRequiresSessionState 
    {
        //PubnubAPI pubnub = new PubnubAPI(
        //        "pub-2e41c0cf-3a9d-4d2e-83d2-1d7ae0a12b78",				// PUBLISH_KEY
        //        "sub-c29f5187-b88e-11e1-9907-d99166562f9b",				// SUBSCRIBE_KEY
        //        "sec-OWVhZjIwYTMtNDZjYy00NzdkLTk0M2QtZWQzY2U2OTU2MWE1", // SECRET_KEY
        //        true													// SSL_ON?
        //        );
        SessionState _sessionState = new SessionState();
        RestoreSession _restoreSession = new RestoreSession();
        string _channelName = string.Empty;
        string _channelPassword = string.Empty;

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
                    _restoreSession.LastChannel = new Channel();
                    _restoreSession.LastChannel.Messages = AppCache.MessageList.Where(m => m.ChannelId == chan.Id).Take(10).ToList();
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
                    _restoreSession.LastChannel = new Channel();
                    _restoreSession.LastChannel.Messages = AppCache.MessageList.Where(m => m.ChannelId == chan.Id).Take(10).ToList();
                }
            }

            chan = _restoreSession.LastChannel;
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
                            msg.CreatedDateTime = DateTime.Now.ToString();
                            msg.Type = MessageTypes.File;

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



                            msg.Type = MessageTypes.File;
                            // create full server path
                            string file = string.Format("{0}\\{1}", directory, filename);

                            // If file exists already, delete it (optional)
                            //if (System.IO.File.Exists(file) == true) System.IO.File.Delete(file);

                            if (string.IsNullOrEmpty(uploadFileName) == true) // IE Browsers
                            {
                                // Save file to server
                                context.Request.Files[0].SaveAs(file);
                            }
                            else // Other Browsers
                            {
                                // Save file to server
                                System.IO.FileStream fileStream = new System.IO.FileStream(file, System.IO.FileMode.OpenOrCreate);
                                context.Request.InputStream.CopyTo(fileStream); 
                                
                                fileStream.Close();
                            }

                            using (FileStream sr = new FileStream(file, FileMode.Open))
                            {
                                uploadToCloud(filename, uploadFileName, sr, msg, chan, file);
                            }
                            

                            //save in mongodb
                            _sessionState.SaveMessage(msg);

                            //send notification in pubnub

                            if (msg.Type == MessageTypes.File)
                            {
                                _sessionState.AuthClient();
                                msg.Content = _sessionState.DropboxClient.GetMediaLinkAsync(msg.Content).Result.Url;
                            }

                            PublishToPubNub(msg);

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

        private void uploadToCloud(string filename, string uploadFileName, Stream fileStream, Message msg, Channel chan, string file)
        {
            if (filename.EndsWith("png") || filename.EndsWith("jpeg") || filename.EndsWith("jpg") || filename.EndsWith("gif") || filename.EndsWith("bmp"))
            {
                try
                {
                    msg.Type = MessageTypes.Image;
                    var configuration = new AccountConfiguration("saykor", "277334748579534", "mUjzZ-X3jOuNKGswrAjocB-D-Rc");

                    var uploader = new Uploader(configuration);
                    string publicId = Path.GetFileNameWithoutExtension(filename);
                    var uploadResult = uploader.Upload(new UploadInformation(filename, fileStream)
                    {
                        PublicId = publicId,
                        Format = filename.Substring(filename.Length - 3),
                    });
                    //msg.Content = uploadResult.Url;
                    msg.Content = filename;
                }
                catch (Exception ex)
                {
                    //context.Response.Write("{ 'success': " + ex.Message + " }");
                    return;
                }
            }
            else
            {
                //upload to dropbox
                string cloudPath = "/" + chan.Name + "/" + filename;
                _sessionState.AuthClient();
                var result = _sessionState.DropboxClient.UploadFileAsync(new FileResource(file), cloudPath).Result;
                msg.Content = cloudPath;
            }
        }

        private void PublishToPubNub(Message msg)
        {
            if (msg != null)
            {
                msg.Status = "add";
                if (msg.Type == MessageTypes.File)
                {
                    _sessionState.AuthClient();
                    msg.Content = _sessionState.DropboxClient.GetMediaLinkAsync(msg.Content).Result.Url;
                }
                List<object> publishResult = _sessionState.Pubnub.Publish("NewMsgIn" + msg.ChannelId, msg);
            }
        }

        private Channel getChannel(HttpContext context)
        {
            Channel chan;
            if (!string.IsNullOrEmpty(context.Request["channelName"]))
                _channelName = context.Request["channelName"];

            if (!string.IsNullOrEmpty(context.Request["channelPassword"]))
                _channelPassword = context.Request["channelPassword"];

            chan = _sessionState.GetChannel(_channelName, _channelPassword, _sessionState.Pubnub);
            return chan;
        }

        public bool IsReusable
        {
            get { return false; }
        }

        private void GetSession(HttpContext context)
        {
            if (context.Session["sessionState"] != null)
                _sessionState = (SessionState)context.Session["sessionState"];

            if (context.Session["restoreSession"] != null)
                _restoreSession = (RestoreSession)context.Session["restoreSession"];
        }

        private void SaveSession(HttpContext context)
        {
            context.Session["sessionState"] = _sessionState;
            context.Session["restoreSession"] = _restoreSession;
        }
    }
}