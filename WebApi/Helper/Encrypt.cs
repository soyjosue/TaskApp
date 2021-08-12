using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace WebApi.Helper
{
    public class Encrypt
    {
        public static string GetPasswordEncrypt(string password)
        {
            SHA256 sha256 = SHA256Managed.Create();
            ASCIIEncoding encoding = new ASCIIEncoding();

            byte[] stream = null;

            StringBuilder sb = new StringBuilder();
            stream = sha256.ComputeHash(encoding.GetBytes(password));

            for(int i =0; i < stream.Length; i++) sb.AppendFormat("{0:x2}", stream[1]);

            return sb.ToString();
        }
    }
}