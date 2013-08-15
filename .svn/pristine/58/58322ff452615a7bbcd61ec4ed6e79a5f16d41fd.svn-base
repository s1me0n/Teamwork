using System.IO;
using SecretCommunicator.Models.Library;

namespace SecretCommunicator.Data
{
    public class Functions
    {
        public static bool DirectoryExist(string DirectoryPatch)
        {
            DirectoryInfo objDirectory = new DirectoryInfo(DirectoryPatch);
            if (objDirectory.Exists)
            {
                return true;
            }
            else
            {
                try
                {
                    Directory.CreateDirectory(DirectoryPatch);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        public static Message CreatePublicMessage(Message value)
        {
            //PublicMessage msg = new PublicMessage();
            //msg.Id = value.Id;
            //msg.Type = value.Type;
            //msg.Values = AppCache.AESProvider.DecryptString(value.Value);
            //msg.CreatedDateTime = value.CreatedDateTime;
            return value;
        }
    }
}