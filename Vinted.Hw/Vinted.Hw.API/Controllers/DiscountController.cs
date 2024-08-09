using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vinted.Hw.Contracts;
using Vinted.Hw.Models;
using Vinted.Hw.Services;

namespace Vinted.Hw.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DiscountController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public DiscountController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpPost("ProcessDiscounts")]
        public async Task<IActionResult> ProcessDiscounts(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            List<string> lines = new List<string>();
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    lines.Add(line);
                }
            }

            var processedContent = ProcessFileContent(lines);

            return Ok(processedContent);
        }

        private List<TransactionContract> ProcessFileContent(List<string> lines)
        {
            List<string> transactionLines = lines.Select(line => line.ToUpper()).ToList();
            List<TransactionContract> transactionContracts = _transactionService.GetProcessedTransactions(transactionLines).TransactionModelsToTransactionContracts();

            return transactionContracts;
        }
    }
}
