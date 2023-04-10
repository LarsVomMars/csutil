using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace csutil
{
    public class Encryption
    {
        public static string OpenFile(string fileName, string password, string iv)
        {
            var aes = new AesManaged();
            var key = Encoding.UTF8.GetBytes(password.PadRight(16, '#'));
            var byteIv = Encoding.UTF8.GetBytes(iv.PadRight(16, '#'));

            using (var fs = new FileStream(fileName, FileMode.Open))
            using (var cs = new CryptoStream(fs, aes.CreateDecryptor(key, byteIv), CryptoStreamMode.Read))
            using (var sr = new StreamReader(cs))
                return sr.ReadToEnd();
        }

        public static void SaveFile(string fileName, string content, string password, string iv)
        {
            var aes = new AesManaged();
            var key = Encoding.UTF8.GetBytes(password.PadRight(16, '#'));
            var byteIv = Encoding.UTF8.GetBytes(iv.PadRight(16, '#'));

            using (var fs = new FileStream(fileName, FileMode.OpenOrCreate))
            using (var cs = new CryptoStream(fs, aes.CreateEncryptor(key, byteIv), CryptoStreamMode.Write))
            using (var sw = new StreamWriter(cs))
                sw.Write(content);
        }
    }
}