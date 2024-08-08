using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Vinted.Hw.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DiscountController : ControllerBase
    {
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

        private IEnumerable<string> ProcessFileContent(IEnumerable<string> lines)
        {
            // Example processing logic, could be more complex
            return lines.Select(line => line.ToUpper());
        }
    }
}
