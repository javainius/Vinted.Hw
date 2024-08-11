using Microsoft.AspNetCore.Mvc;
using Vinted.Hw.API.Mappings;
using Vinted.Hw.Contracts;
using Vinted.Hw.Models;
using Vinted.Hw.Services;
using Vinted.Hw.Services.Interfaces;

namespace Vinted.Hw.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpPost("ProcessTransactions")]
        public async Task<IActionResult> ProcessTransactions(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            List<TransactionModel> transactionModels = await _transactionService.GetProcessedTransactions(file);

            return Ok(transactionModels.TransactionModelsToTransactionContracts());
        }

    }
}
