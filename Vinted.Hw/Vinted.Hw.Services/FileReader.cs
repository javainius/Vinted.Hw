using Microsoft.AspNetCore.Http;

namespace Vinted.Hw.API
{
    public static class FileReader
    {
        public static async Task<List<string>> GetTransactionLines(IFormFile file)
        {
            List<string> transactionLines = new();

            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                while (!reader.EndOfStream)
                {
                    string? line = await reader.ReadLineAsync();
                    transactionLines.Add(line);
                }
            }

            return transactionLines;
        }
    }
}
