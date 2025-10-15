using System.IO;

namespace FrameworkProject
{
    /// <summary>
    /// Defines an abstraction for interacting with an SFTP server.
    /// </summary>
    public interface ISftpClient
    {
        /// <summary>
        /// Uploads a file stream to the specified remote path on the SFTP server.
        /// </summary>
        /// <param name="stream">The input stream to upload.</param>
        /// <param name="remotePath">The full destination path on the server.</param>
        void UploadFile(Stream stream, string remotePath);

        /// <summary>
        /// Checks whether a file or directory exists at the given path on the SFTP server.
        /// </summary>
        /// <param name="remotePath">The path to check for existence.</param>
        /// <returns><c>true</c> if the path exists; otherwise, <c>false</c>.</returns>
        bool Exists(string remotePath);
    }
}