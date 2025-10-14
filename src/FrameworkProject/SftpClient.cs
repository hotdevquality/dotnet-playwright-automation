// SftpClient.cs
using Renci.SshNet;

namespace FrameworkProject;

public interface ISftpClient
{
    void UploadFile(Stream stream, string remotePath);
    bool Exists(string remotePath);
}

public class SshNetSftpClient : ISftpClient
{
    private readonly IConfig _cfg;
    public SshNetSftpClient(IConfig cfg) => _cfg = cfg;

    public void UploadFile(Stream stream, string remotePath)
    {
        using var sftp = new SftpClient(_cfg.SftpHost, _cfg.SftpPort, _cfg.SftpUser, _cfg.SftpPassword);
        sftp.Connect();
        EnsureRemoteDirectoryExists(sftp, remotePath);
        sftp.UploadFile(stream, remotePath, true);
        sftp.Disconnect();
    }
    
    public bool Exists(string remotePath)
    {
        using var sftp = new SftpClient(_cfg.SftpHost, _cfg.SftpPort, _cfg.SftpUser, _cfg.SftpPassword);
        sftp.Connect();
        var exists = sftp.Exists(remotePath);
        sftp.Disconnect();
        return exists;
    }

    private static void EnsureRemoteDirectoryExists(SftpClient sftp, string remotePath)
    {
        // Normalize and extract the directory portion
        var path = remotePath.Replace('\\', '/');
        var lastSlash = path.LastIndexOf('/');
        var dir = lastSlash <= 0 ? "/" : path.Substring(0, lastSlash);

        // Build each segment and create if missing
        string current = dir.StartsWith("/") ? "/" : "";
        foreach (var segment in dir.Split('/', StringSplitOptions.RemoveEmptyEntries))
        {
            current = current == "/" ? $"/{segment}" : $"{current}/{segment}";
            if (!sftp.Exists(current))
                sftp.CreateDirectory(current);
        }
    }
}