using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace DromHubSettings.Helpers
{
    public static class CryptoHelper
    {
        // Получение ключа из переменной окружения (ключ должен быть в виде base64 строки)
        private static byte[] GetKey()
        {
            string keyBase64 = Environment.GetEnvironmentVariable("ENCRYPTION_KEY");
            if (string.IsNullOrWhiteSpace(keyBase64))
                throw new Exception("Encryption key not found in environment variables.");
            return Convert.FromBase64String(keyBase64);
        }

        // Получение вектора инициализации из переменной окружения
        private static byte[] GetIV()
        {
            string ivBase64 = Environment.GetEnvironmentVariable("ENCRYPTION_IV");
            if (string.IsNullOrWhiteSpace(ivBase64))
                throw new Exception("Encryption IV not found in environment variables.");
            return Convert.FromBase64String(ivBase64);
        }

        public static string EncryptString(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return plainText;

            using (Aes aes = Aes.Create())
            {
                aes.Key = GetKey();
                aes.IV = GetIV();

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    using (StreamWriter sw = new StreamWriter(cs))
                    {
                        sw.Write(plainText);
                    }
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        public static string DecryptString(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                return cipherText;

            using (Aes aes = Aes.Create())
            {
                aes.Key = GetKey();
                aes.IV = GetIV();

                using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(cipherText)))
                using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read))
                using (StreamReader sr = new StreamReader(cs))
                {
                    return sr.ReadToEnd();
                }
            }
        }
    }
}
