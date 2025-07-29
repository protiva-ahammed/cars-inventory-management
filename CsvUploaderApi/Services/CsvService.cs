using System.Globalization;
using CsvHelper;
using CsvUploaderApi.Interfaces;
using CsvUploaderApi.Models;
using Npgsql;

public class CsvService : ICsvService
{
    private readonly IConfiguration _configuration;

    public CsvService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public async Task<CsvProcessResponse> ProcessCsvAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("No file uploaded.");

        var records = new List<Dictionary<string, string>>();

        using (var reader = new StreamReader(file.OpenReadStream()))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            if (!await csv.ReadAsync())
                throw new ArgumentException("CSV is empty or unreadable.");

            csv.ReadHeader();

            var headers = csv.HeaderRecord;
            if (headers == null || headers.Length == 0)
                throw new ArgumentException("CSV does not contain headers.");

            while (await csv.ReadAsync())
            {
                var record = new Dictionary<string, string>();
                foreach (var header in headers)
                {
                    record[header] = csv.GetField(header) ?? string.Empty;
                }
                records.Add(record);
            }

            if (records.Count == 0)
                throw new ArgumentException("CSV has headers but no data rows.");

            var tableName = "uploaded_table_" + Guid.NewGuid().ToString("N").Substring(0, 8);

            var connStr = _configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(connStr))
                throw new InvalidOperationException("Database connection string is missing.");

            using (var conn = new NpgsqlConnection(connStr))
            {
                await conn.OpenAsync();

                // Create Table
                var createTableSql = $"CREATE TABLE \"{tableName}\" ({string.Join(", ", records[0].Keys.Select(k => $"\"{k}\" TEXT"))});";
                using (var cmd = new NpgsqlCommand(createTableSql, conn))
                {
                    await cmd.ExecuteNonQueryAsync();
                }

                // Insert Data
                foreach (var record in records)
                {
                    var columns = string.Join(", ", record.Keys.Select(k => $"\"{k}\""));
                    var values = string.Join(", ", record.Values.Select(v => $"'{v.Replace("'", "''")}'"));

                    var insertSql = $"INSERT INTO \"{tableName}\" ({columns}) VALUES ({values});";
                    using (var cmd = new NpgsqlCommand(insertSql, conn))
                    {
                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                return new CsvProcessResponse { RowsInserted = records.Count, TableName = tableName };
            }
        }
    }

    public async Task<List<Dictionary<string, string>>> GetDataFromTableAsync(string tableName)
{
    var result = new List<Dictionary<string, string>>();
    var connStr = _configuration.GetConnectionString("DefaultConnection");

    using var conn = new NpgsqlConnection(connStr);
    await conn.OpenAsync();

    var query = $"SELECT * FROM \"{tableName}\"";  // Use quotes for safety
    using var cmd = new NpgsqlCommand(query, conn);
    using var reader = await cmd.ExecuteReaderAsync();

    while (await reader.ReadAsync())
    {
        var row = new Dictionary<string, string>();
        for (int i = 0; i < reader.FieldCount; i++)
        {
            var key = reader.GetName(i);
            var value = reader[i]?.ToString() ?? "";
            row[key] = value;
        }
        result.Add(row);
    }

    return result;
}

}

