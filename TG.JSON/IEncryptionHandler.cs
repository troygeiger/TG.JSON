namespace TG.JSON
{
    /// <summary>
    /// An interface for handling encryption.
    /// </summary>
    public interface IEncryptionHandler
    {
        /// <summary>
        /// Gets the encryption key value currently set.
        /// </summary>
        byte[] EncryptionKey { get; }

        /// <summary>
        /// Decrypts a byte array to a byte array.
        /// </summary>
        /// <param name="bytes">A byte array to decrypt.</param>
        /// <returns>Unencrypted byte array.</returns>
        byte[] Decrypt(byte[] bytes);

        /// <summary>
        /// Decrypts a base64 string to an unencrypted Unicode string.
        /// </summary>
        /// <param name="base64">Base64 string to decrypt.</param>
        /// <returns>Unencrypted string.</returns>
        string DecryptBase64(string base64);

        /// <summary>
        /// Decrypts a base64 string to an unencrypted byte array.
        /// </summary>
        /// <param name="base64">Base64 string to decrypt.</param>
        /// <returns>Unencrypted byte array.</returns>
        byte[] DecryptBase64ToByte(string base64);

        /// <summary>
        /// Decrypts byte array to a Unicode string.
        /// </summary>
        /// <param name="bytes">The byte data to decrypt.</param>
        /// <returns>Unencrypted string.</returns>
        string DecryptToString(byte[] bytes);

        /// <summary>
        /// Do your cleanup.
        /// </summary>
        void Dispose();

        /// <summary>
        /// Encrypts a byte array to a byte array.
        /// </summary>
        /// <param name="bytes">A byte array to encrypt.</param>
        /// <returns>Encrypted byte array.</returns>
        byte[] Encrypt(byte[] bytes);

        /// <summary>
        /// Encrypts text to a byte array.
        /// </summary>
        /// <param name="text">The string of text to encrypt.</param>
        /// <returns>Encrypted byte array.</returns>
        byte[] Encrypt(string text);

        /// <summary>
        /// Encrypts a byte array to a base64 string.
        /// </summary>
        /// <param name="bytes">A byte array to encrypt.</param>
        /// <returns>Encrypted base64 string.</returns>
        string EncryptBase64(byte[] bytes);

        /// <summary>
        /// Encrypts text to a base64 string.
        /// </summary>
        /// <param name="text">The string of text to encrypt.</param>
        /// <returns>Encrypted base64 string.</returns>
        string EncryptBase64(string text);

        /// <summary>
        /// Returns the EncryptionKey as a UTF8 string.
        /// </summary>
        /// <returns>string</returns>
        string EncryptionKeyAsString();
    }
}