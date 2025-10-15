using System;
using System.IO;
using Renci.SshNet;

namespace FrameworkProject
{
    /// <summary>
    /// Concrete implementation of <see cref="ISftpClient"/> using SSH.NET library.
    /// Handles SFTP upload and file existence checks.
    /// </summary>
    public class SshNetSftpClient : ISftpClient
    {
        private readonly IConfig _cfg;

        public SshNetSftpClient(IConfig cfg) => _cfg = cfg;

        /// <inheritdoc />
        public void UploadFile(Stream stream, string remotePath)
        {
            using var sftp = new SftpClient(_cfg.SftpHost, _cfg.SftpPort, _cfg.SftpUser, _cfg.SftpPassword);
            sftp.Connect();

            EnsureRemoteDirectoryExists(sftp, remotePath);
            sftp.UploadFile(stream, remotePath, true);

            sftp.Disconnect();
        }

        /// <inheritdoc />
        public bool Exists(string remotePath)
        {
            using var sftp = new SftpClient(_cfg.SftpHost, _cfg.SftpPort, _cfg.SftpUser, _cfg.SftpPassword);
            sftp.Connect();

            var exists = sftp.Exists(remotePath);

            sftp.Disconnect();
            return exists;
        }

        /// <summary>
        /// Ensures all directories in the specified remote path exist, creating them if missing.
        /// </summary>
        /// <param name="sftp">The connected SFTP client.</param>
        /// <param name="remotePath">The full path of the target file or directory.</param>
        private static void EnsureRemoteDirectoryExists(SftpClient sftp, string remotePath)
        {
            var path = remotePath.Replace('\\', '/');
            var lastSlash = path.LastIndexOf('/');
            var dir = lastSlash <= 0 ? "/" : path.Substring(0, lastSlash);

            string current = dir.StartsWith("/") ? "/" : "";
            foreach (var segment in dir.Split('/', StringSplitOptions.RemoveEmptyEntries))
            {
                current = current == "/" ? $"/{segment}" : $"{current}/{segment}";
                if (!sftp.Exists(current))
                    sftp.CreateDirectory(current);
            }
        }
    }
}
