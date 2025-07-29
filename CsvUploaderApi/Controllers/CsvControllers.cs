using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using CsvHelper;
using Npgsql;
using System.Data;
using Microsoft.Extensions.Configuration;
using CsvUploaderApi.Interfaces;
using CsvUploaderApi.Models;

namespace CsvUploaderApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CsvUploadController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ICsvService _csvService;

        public CsvUploadController(IConfiguration configuration, ICsvService csvService)
        {
            _configuration = configuration;
            _csvService = csvService;
        }


        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadCsv([FromForm] IFormFile file)
        {
            try
            {
                CsvProcessResponse row = await _csvService.ProcessCsvAsync(file);
                return Ok(row);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Unexpected error: {ex.Message}");
            }
        }

        [HttpGet("data")]
        public async Task<IActionResult> GetUploadedData([FromQuery] string tableName)
        {
            if (string.IsNullOrWhiteSpace(tableName))
                return BadRequest("Table name is required.");

            try
            {
                var records = await _csvService.GetDataFromTableAsync(tableName);
                return Ok(records);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Unexpected error: {ex.Message}");
            }
        }
    }
}