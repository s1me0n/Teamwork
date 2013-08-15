using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Cloudinary;
using SecretCommunicator.Data;
using SecretCommunicator.Data.Helpers;
using SecretCommunicator.Data.Interfaces;
using SecretCommunicator.Models.Library;
using Spring.IO;
using Spring.Social.Dropbox.Api;
using Spring.Social.Dropbox.Connect;

namespace SecretCommunicator.WebApi.Controllers
{
    public class DropboxController : BaseApiController<Message>
    {
        SessionState _sessionState = new SessionState();
        RestoreSession _restoreSession = new RestoreSession() { GetUserChannels = new List<string>() };

        string _channelName = string.Empty;
        string _channelPassword = string.Empty;

        public DropboxController(IRepository repository)
            : base(repository)
        {
        }

        public override HttpResponseMessage Post(Message value)
        {
            var response = new HttpResponseMessage();
            var context = HttpContext.Current;

            GetSession();
            var channel = new Channel();
            if (_sessionState.CurrentUser == null)
            {
                channel = getChannel(context);
                if (channel != null)
                {
                    _sessionState.CurrentUser = new User() { ChannelName = new List<string>() };
                    _sessionState.CurrentUser.ChannelName.Add(channel.Name);
                    RestoreChannel(channel);
                }
                else
                {
                    response =
                    Request.CreateResponse(HttpStatusCode.Accepted, "{ 'success': login to channel pls }");
                    return response;
                }
            }
            else if (_restoreSession.LastChannel == null)
            {
                channel = getChannel(context);
                if (channel != null)
                {
                    RestoreChannel(channel);
                }
            }

            channel = _restoreSession.LastChannel;
            if (channel != null)
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
                            msg.ChannelId = channel.Id;
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
                                var fileStream = new FileStream(file, System.IO.FileMode.OpenOrCreate);
                                context.Request.InputStream.CopyTo(fileStream);

                                fileStream.Close();
                            }

                            using (var sr = new FileStream(file, FileMode.Open))
                            {
                                uploadToCloud(filename, uploadFileName, sr, msg, channel, file);
                            }

                            //save in mongodb
                            _sessionState.SaveMessage(msg);

                            //send notification in pubnub

                            //if (msg.Type == MessageTypes.File)
                            //{
                            //    //_sessionState.AuthClient();
                            //    msg.Content = _sessionState.DropboxClient.GetMediaLinkAsync(msg.Content).Result.Url;
                            //}

                            PublishToPubNub(msg);

                            // return the json object as successful
                            response =
                            Request.CreateResponse(HttpStatusCode.Accepted, "{ 'success': true }");
                            return response;
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
                    SaveSession();
                }
            }

            //var httpRequest = HttpContext.Current.Request;
            //if (httpRequest.Files.Count > 0)
            //{
            //    var docfiles = new List<string>();
            //    foreach (string file in httpRequest.Files)
            //    {
            //        var postedFile = httpRequest.Files[file];
            //        var filePath = HttpContext.Current.Server.MapPath("~/App_Data/" + postedFile.FileName);
            //        postedFile.SaveAs(filePath);

            //        docfiles.Add(DropboxShareFile(filePath, postedFile.FileName));
            //    }
            //    result = Request.CreateResponse(HttpStatusCode.Created, docfiles);
            //}
            //else
            //{
            //    result = Request.CreateResponse(HttpStatusCode.BadRequest);
            //}
            return response;
        }

        private void RestoreChannel(Channel channel)
        {
            _sessionState.GetChanMessages(channel, 10);
            _restoreSession.LastChannel = new Channel();
            _restoreSession.LastChannel.Messages = AppCache.MessageList.Where(m => m.ChannelId == channel.Id).Take(10).ToList();
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
                var result = _sessionState.DropboxShareFile(file, cloudPath);
                //_sessionState.AuthClient();
                //var result = _sessionState.DropboxClient.UploadFileAsync(new FileResource(file), cloudPath).Result;
                msg.Content = result;//cloudPath;
            }
        }

        private void PublishToPubNub(Message msg)
        {
            if (msg != null)
            {
                msg.Status = "add";
                //if (msg.Type == MessageTypes.File)
                //{
                //    _sessionState.AuthClient();
                //    msg.Content = _sessionState.DropboxClient.GetMediaLinkAsync(msg.Content).Result.Url;
                //}
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

        private void GetSession()
        {
            if (HttpContext.Current.Session["sessionState"] != null)
                _sessionState = (SessionState)HttpContext.Current.Session["sessionState"];

            if (HttpContext.Current.Session["restoreSession"] != null)
                _restoreSession = (RestoreSession)HttpContext.Current.Session["restoreSession"];
        }

        protected void SaveSession()
        {
            HttpContext.Current.Session["sessionState"] = _sessionState;
            HttpContext.Current.Session["restoreSession"] = _restoreSession;
        }
    }
}