namespace FrameworkProject;

public interface IConfig
{
    string BaseUrl { get; }
    string SftpHost { get; }
    int SftpPort { get; }
    string SftpUser { get; }
    string SftpPassword { get; }
    string SftpInboundPath { get; }
}

public class Config : IConfig
{
    public string BaseUrl { get; init; } = "http://localhost:8080";
    public string SftpHost { get; init; } = "localhost";
    public int SftpPort { get; init; } = 2222;
    public string SftpUser { get; init; } = "testuser";
    public string SftpPassword { get; init; } = "password";
    public string SftpInboundPath { get; init; } = "/upload/inbound";
}