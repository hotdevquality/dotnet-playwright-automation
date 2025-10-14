using FrameworkProject;
using FluentAssertions;
using Reqnroll;

namespace Tests.Automation.Steps;

[Binding]
public class BatchSteps
{
    private readonly IBatchFileBuilder _builder;
    private readonly ISftpClient _sftp;
    private readonly IConfig _cfg;
    private List<PaymentRow> _rows = new();
    private byte[]? _payload;
    private string _remoteFile = string.Empty;

    public BatchSteps(IBatchFileBuilder builder, ISftpClient sftp, IConfig cfg)
    {
        _builder = builder;
        _sftp = sftp;
        _cfg = cfg;
    }

    [Given(@"a batch containing the following payments")]
    public void GivenBatchRows(Table table)
    {
        foreach (var r in table.Rows)
        {
            _rows.Add(new PaymentRow(r["Country"], r["Method"], r["Recipient"], r["SortCode"], r["AccountNumber"],
                r["Iban"], decimal.Parse(r["Amount"]), r["Currency"]));
        }
    }

    [When(@"I build a CSV batch file")]
    public void WhenBuildCsv()
    {
        _payload = _builder.BuildCsv(_rows);
        _remoteFile = $"{_cfg.SftpInboundPath}/payments_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv";
    }
    
    [When(@"I build an Excel batch file")]
    public void WhenBuildExcel()
    {
        _payload = _builder.BuildExcel(_rows);
        _remoteFile = $"{_cfg.SftpInboundPath}/payments_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xlsx";
    }

    [When(@"I upload the batch to SFTP")]
    public void WhenUploadSftp()
    {
        _payload.Should().NotBeNull();
        using var ms = new MemoryStream(_payload!);
        _sftp.UploadFile(ms, _remoteFile);
    }

    [Then(@"the file should exist in the SFTP inbound folder")]
    public void ThenFileShouldExist()
    {
        _sftp.Exists(_remoteFile).Should().BeTrue("file should be present after upload");
    }
}