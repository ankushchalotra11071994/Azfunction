using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.IO;

namespace Company.Function;

public class MyfirstHttpFunction
{
    private readonly ILogger<MyfirstHttpFunction> _logger;

    public MyfirstHttpFunction(ILogger<MyfirstHttpFunction> logger)
    {
        _logger = logger;
    }

     
    [Function("GreetUserFunction")]
    public IActionResult GreetUser(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("Processing GreetUserFunction request.");

        string? name = req.Query["name"].ToString();
        if (string.IsNullOrEmpty(name))
        {
            // Try to read from request body if not in query
            using var reader = new StreamReader(req.Body);
            var body = reader.ReadToEndAsync().Result;
            if (!string.IsNullOrEmpty(body))
            {
                try
                {
                    var data = System.Text.Json.JsonDocument.Parse(body);
                    if (data.RootElement.TryGetProperty("name", out var nameProp))
                    {
                        name = nameProp.GetString() ?? string.Empty;
                    }
                }
                catch (System.Text.Json.JsonException)
                {
                    _logger.LogWarning("Invalid JSON in request body.");
                }
            }
        }

        string responseMessage = string.IsNullOrEmpty(name)
            ? "Please provide a name in the query string or in the request body as JSON."
            : $"Hello, {name}! Welcome to Azure Functions.";

        _logger.LogInformation($"Response: {responseMessage}");
        return new OkObjectResult(responseMessage);
    }
}