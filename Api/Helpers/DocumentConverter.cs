using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using Domain.Model;
using OfficeOpenXml;

namespace Api.Helpers;

public static class DocumentConverter
{
    // Write data to CSV asynchronously
    public static async Task WriteCsv<T>(string filePath, List<T> data, CancellationToken cancellationToken)
    {
        if (!File.Exists(filePath)) File.Create(filePath);
        await using StreamWriter writer = new(filePath, false, Encoding.UTF8);
        await using CsvWriter csv = new(writer, new CsvConfiguration(CultureInfo.InvariantCulture));
        await csv.WriteRecordsAsync(data, cancellationToken);
    }

    public static async IAsyncEnumerable<T> ReadFromCsv<T>(string filePath,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        if (!File.Exists(filePath)) yield break;
        CsvConfiguration config = new(CultureInfo.InvariantCulture) { HasHeaderRecord = true };
        using StreamReader streamReader = new(filePath);
        using CsvReader csvReader = new(streamReader, config);
        await foreach (T record in csvReader.GetRecordsAsync<T>(cancellationToken))
        {
            yield return record;
        }
    }

    private const string DefaultWorkSheet = "Sheet1";

    // Write data to Excel asynchronously
    public static async Task WriteExcelAsync<T>(string filePath, List<T> data, CancellationToken cancellationToken)
    {
        if (!File.Exists(filePath)) File.Create(filePath);
        FileInfo fileInfo = new(filePath);
        using ExcelPackage package = new(fileInfo);
        ExcelWorksheet? worksheet = package.Workbook.Worksheets.Add(DefaultWorkSheet);

        PropertyInfo[] properties = typeof(T).GetProperties(); // Get all properties dynamically

        // Write Header Row (Property Names)
        for (int col = 0; col < properties.Length; col++)
        {
            if (cancellationToken.IsCancellationRequested) break;
            worksheet.Cells[1, col + 1].Value = properties[col].Name;
        }

        // Write Data Rows
        for (int row = 0; row < data.Count; row++)
        {
            if (cancellationToken.IsCancellationRequested) break;
            for (int col = 0; col < properties.Length; col++)
            {
                if (cancellationToken.IsCancellationRequested) break;
                worksheet.Cells[row + 2, col + 1].Value = properties[col].GetValue(data[row]);
            }
        }

        await package.SaveAsync(cancellationToken);
    }


    // Read data from Excel asynchronously
    public static async Task<List<T>> ReadExcelAsync<T>(string filePath, CancellationToken cancellationToken) where T : new()
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        List<T> data = [];
        FileInfo fileInfo = new(filePath);
        using ExcelPackage package = new(fileInfo);
        ExcelWorksheet? worksheet = package.Workbook.Worksheets[DefaultWorkSheet];

        if (worksheet == null) return data;

        int rowCount = worksheet.Dimension.Rows;
        int colCount = worksheet.Dimension.Columns;
        PropertyInfo[] properties = typeof(T).GetProperties(); // Get all properties dynamically

        for (int row = 2; row <= rowCount; row++) // Start from row 2 (skip headers)
        {
            if (cancellationToken.IsCancellationRequested) break;
            T user = new();

            for (int col = 1; col <= colCount; col++)
            {
                if (cancellationToken.IsCancellationRequested) break;
                PropertyInfo property = properties[col - 1]; // Match column index to property index
                string? cellValue = worksheet.Cells[row, col].Text;

                if (property.PropertyType == typeof(int) && int.TryParse(cellValue, out int intValue))
                {
                    property.SetValue(user, intValue);
                }
                else
                {
                    property.SetValue(user, cellValue);
                }
            }

            data.Add(user);
        }

        return await Task.FromResult(data);
    }
}