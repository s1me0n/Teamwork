using System.Web;

namespace SecretCommunicator.WebApi.Webservices
{
    /// <summary>
    /// Based on a filename, this handler will retrieve the uploaded image
    /// </summary>
    public sealed class PreviewPhoto : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            string filename = context.Request.QueryString["filename"];
            string extension = System.IO.Path.GetExtension(filename).Trim('.');

            string directory = context.Server.MapPath("~/UploadFiles");
            string file = string.Format("{0}\\{1}", directory, filename);

            context.Response.ContentType = string.Format("image/{0}", extension);
            context.Response.WriteFile(file);
            //if (System.IO.File.Exists(file) == true) System.IO.File.Delete(file);
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }
}