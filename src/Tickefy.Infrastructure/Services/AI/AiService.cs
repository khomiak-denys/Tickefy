using Google.GenAI;     
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Tickefy.Application.Abstractions.Services;
using Tickefy.Application.AI.Dtos;


namespace Tickefy.Infrastructure.Services.AI
{

    public class AiService : IAiService
    {
        private readonly Client _client;
        private readonly ILogger<AiService> _logger;

        public AiService(Client client, ILogger<AiService> logger)
        {
           _client = client;
           _logger = logger;
        }

        public async Task<AiResponse> AnalyzeTicketAsync(string title, string description, DateTime deadline)
        {
             var prompt =
                 @$"You are an internal Ticketing AI classifier. 
                 Your task is to analyze the ticket details and assign a Category and Priority. 
                 Only return a JSON object.

                 Available Categories: Finance, IT, Design, Marketing, HumanResources, Legal, AccessAndSecurity, Other
                 Available Priorities: Critical, High, Medium, Low.
                 Priority should depend on how much time was given to complete the task.


                 Ticket data:
                 Title: {title}
                 Description: {description}
                 Deadline: {deadline:O}

                 Return ONLY raw JSON. 
                 Do NOT use markdown formatting. 
                 Do NOT wrap anything in backticks. 
                 Output must start with '{{' and end with '}}'.

                 Strictly return JSON in the exact format:
                 {{""Category"": ""..."", ""Priority"": ""...""}}";

            var response = await _client.Models.GenerateContentAsync(
                model: "gemini-2.0-flash",
                contents: prompt
            );


            var parts = response?.Candidates?
                .SelectMany(c => c.Content.Parts)
                .ToList();

            var json = string.Join("", parts
                .Where(p => p.Text != null)
                .Select(p => p.Text));

            if (string.IsNullOrWhiteSpace(json))
                throw new InvalidOperationException("AI returned empty response.");

            _logger.LogInformation("Raw AI JSON: {Json}", json);
            
            var parsed = JsonSerializer.Deserialize<AiResponse>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
            _logger.LogWarning($"AI Response. Priority: {parsed.Priority}, category: {parsed.Category}");

            return parsed ?? throw new InvalidOperationException("Invalid AI JSON format.");
        }

    }
}