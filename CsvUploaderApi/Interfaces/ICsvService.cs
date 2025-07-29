using CsvUploaderApi.Models;

namespace CsvUploaderApi.Interfaces
{
    public interface ICsvService
    {
        Task<CsvProcessResponse> ProcessCsvAsync(IFormFile file);
        Task<List<Dictionary<string, string>>> GetDataFromTableAsync(string tableName);

  
    }
}