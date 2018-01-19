using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace TG.JSON
{
    /// <summary>
    /// Provides AES Encryption.
    /// </summary>
    public class EncryptionHandler : IDisposable, IEncryptionHandler
    {
        byte[] cryptKey,
            iv = new byte[] { 68, 65, 43, 114, 98, 118, 120, 103, 101, 79, 102, 107, 100, 111, 51, 33 };
        Rijndael aes;
        Random randomizer = new Random();

        /// <summary>
        /// Creates an instance of <see cref="EncryptionHandler"/>.
        /// </summary>
        /// <param name="key">The key, no larger than 32 bytes, to use during encryption and decryption.</param>
        public EncryptionHandler(byte[] key)
        {
            if (key == null || (key != null && key.Length == 0))
                throw new ArgumentNullException("key");

            aes = Rijndael.Create();
            if (key.Length > 32)
                throw new CryptographicException("Key size too large.");
            cryptKey = new byte[32];
            for (int i = 0; i < key.Length; i++)
                cryptKey[i] = key[i];

            aes.Key = cryptKey;
        }

        /// <summary>
        /// Creates an instance of <see cref="EncryptionHandler"/>.
        /// </summary>
        /// <param name="key">The key to use during encryption and decryption.</param>
        public EncryptionHandler(string key) : this(Encoding.UTF8.GetBytes(key)) { }

        /// <summary>
        /// Encrypts a byte array to a byte array.
        /// </summary>
        /// <param name="bytes">A byte array to encrypt.</param>
        /// <returns>Encrypted byte array.</returns>
        public byte[] Encrypt(byte[] bytes)
        {
            if (cryptKey == null || iv == null)
                return null;
            if (cryptKey.Length < 32)
                return null;
            byte[] value = new byte[bytes.Length + 1];
            Array.Copy(bytes, 0, value, 1, bytes.Length);
            byte[] randomByte = new byte[1];
            randomizer.NextBytes(randomByte);
            value[0] = randomByte[0];
            using (var enc = aes.CreateEncryptor(cryptKey, iv))
            {
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, enc, CryptoStreamMode.Write))
                    {
                        cs.Write(value, 0, value.Length);
                        cs.FlushFinalBlock();
                        return ms.ToArray();
                    }
                }
            }
        }

        /// <summary>
        /// Encrypts text to a byte array.
        /// </summary>
        /// <param name="text">The string of text to encrypt.</param>
        /// <returns>Encrypted byte array.</returns>
        public byte[] Encrypt(string text)
        {
            return Encrypt(System.Text.Encoding.Unicode.GetBytes(text));
        }

        /// <summary>
        /// Encrypts text to a base64 string.
        /// </summary>
        /// <param name="text">The string of text to encrypt.</param>
        /// <returns>Encrypted base64 string.</returns>
        public string EncryptBase64(string text)
        {
            return Convert.ToBase64String(Encrypt(text));
        }

        /// <summary>
        /// Encrypts a byte array to a base64 string.
        /// </summary>
        /// <param name="bytes">A byte array to encrypt.</param>
        /// <returns>Encrypted base64 string.</returns>
        public string EncryptBase64(byte[] bytes)
        {
            return Convert.ToBase64String(Encrypt(bytes));
        }

        /// <summary>
        /// Decrypts a byte array to a byte array.
        /// </summary>
        /// <param name="bytes">A byte array to decrypt.</param>
        /// <returns>Unencrypted byte array.</returns>
        public byte[] Decrypt(byte[] bytes)
        {
            if (cryptKey == null || iv == null)
                return null;
            if (cryptKey.Length < 32)
                return null;
            using (var enc = aes.CreateDecryptor(cryptKey, iv))
            {
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, enc, CryptoStreamMode.Write))
                    {
                        cs.Write(bytes, 0, bytes.Length);
                        cs.FlushFinalBlock();
                        bytes = ms.ToArray();
                        bytes = new byte[ms.Length - 1];
                        ms.Position = 1;
                        ms.Read(bytes, 0, bytes.Length);
                        return bytes;
                    }
                }
            }
        }

        /// <summary>
        /// Decrypts byte array to a Unicode string.
        /// </summary>
        /// <param name="bytes">The byte data to decrypt.</param>
        /// <returns>Unencrypted string.</returns>
        public string DecryptToString(byte[] bytes)
        {
            return Encoding.Unicode.GetString(Decrypt(bytes));
        }

        /// <summary>
        /// Decrypts a base64 string to an unencrypted Unicode string.
        /// </summary>
        /// <param name="base64">Base64 string to decrypt.</param>
        /// <returns>Unencrypted string.</returns>
        public string DecryptBase64(string base64)
        {
            if (string.IsNullOrEmpty(base64))
                return base64;
            return DecryptToString(Convert.FromBase64String(base64));
        }

        /// <summary>
        /// Decrypts a base64 string to an unencrypted byte array.
        /// </summary>
        /// <param name="base64">Base64 string to decrypt.</param>
        /// <returns>Unencrypted byte array.</returns>
        public byte[] DecryptBase64ToByte(string base64)
        {
            return Decrypt(Convert.FromBase64String(base64));
        }

        /// <summary>
        /// Gets the encryption key value currently set.
        /// </summary>
        public byte[] EncryptionKey
        {
            get { return cryptKey; }
        }

        /// <summary>
        /// Returns the EncryptionKey as a UTF8 string.
        /// </summary>
        /// <returns>string</returns>
        public string EncryptionKeyAsString()
        {
            return Encoding.UTF8.GetString(cryptKey);
        }

        /// <summary>
        /// Disposes the Crypto class.
        /// </summary>
        public void Dispose()
        {
            aes.Clear();
            aes = null;
            cryptKey = null;
            iv = null;
        }
    }
}
