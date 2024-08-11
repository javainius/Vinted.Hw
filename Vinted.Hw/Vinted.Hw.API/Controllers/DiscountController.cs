using Microsoft.AspNetCore.Mvc;
using Vinted.Hw.Contracts;
using Vinted.Hw.Models;
using Vinted.Hw.Services;
using Vinted.Hw.Services.Interfaces;

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

        // all validation should be implemented inside of services
        // then these services should be wrapped with service excpeption wrappers
        [HttpPost("ProcessDiscounts")]
        public async Task<IActionResult> ProcessDiscounts(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            List<TransactionModel> transactionModels = await _transactionService.GetProcessedTransactions(file);

            return Ok(transactionModels.TransactionModelsToTransactionContracts());
        }

    }
}
