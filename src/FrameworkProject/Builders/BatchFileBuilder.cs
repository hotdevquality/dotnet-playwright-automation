using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using OfficeOpenXml;

namespace FrameworkProject;

public record PaymentRow(
    string Country,
    string Method,
    string Recipient,
    string SortCode,
    string AccountNumber,
    string Iban,
    decimal Amount,
    string Currency);

public interface IBatchFileBuilder
{
    byte[] BuildCsv(IEnumerable<PaymentRow> rows);
    byte[] BuildExcel(IEnumerable<PaymentRow> rows);
}

public class BatchFileBuilder : IBatchFileBuilder
{
    public byte[] BuildCsv(IEnumerable<PaymentRow> rows)
    {
        using var mem = new MemoryStream();
        using var writer = new StreamWriter(mem);
        var cfg = new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = true };
        using var csv = new CsvWriter(writer, cfg);
        csv.WriteRecords(rows);
        writer.Flush();
        return mem.ToArray();
    }

    public byte[] BuildExcel(IEnumerable<PaymentRow> rows)
    {
        using var mem = new MemoryStream();
        ExcelPackage.License.SetNonCommercialPersonal("Timothy Hassan");
        using var pck = new ExcelPackage(mem);
        var ws = pck.Workbook.Worksheets.Add("Payments");
        var headers = new[]
            { "Country", "Method", "Recipient", "SortCode", "AccountNumber", "Iban", "Amount", "Currency" };
        for (int i = 0; i < headers.Length; i++) ws.Cells[1, i + 1].Value = headers[i];
        int r = 2;
        foreach (var row in rows)
        {
            ws.Cells[r, 1].Value = row.Country;
            ws.Cells[r, 2].Value = row.Method;
            ws.Cells[r, 3].Value = row.Recipient;
            ws.Cells[r, 4].Value = row.SortCode;
            ws.Cells[r, 5].Value = row.AccountNumber;
            ws.Cells[r, 6].Value = row.Iban;
            ws.Cells[r, 7].Value = (double)row.Amount;
            ws.Cells[r, 8].Value = row.Currency;
            r++;
        }

        ws.Cells.AutoFitColumns();
        pck.Save();
        return mem.ToArray();
    }
}