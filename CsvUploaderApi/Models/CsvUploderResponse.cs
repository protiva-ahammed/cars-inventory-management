namespace CsvUploaderApi.Models
{

    public class CsvProcessResponse
{
    public int RowsInserted { get; set; }
    public required string TableName { get; set; }
}
    
}

