using Google.GenAI;     
using System.Text.Json;
using System.Text.RegularExpressions;
using Tickefy.Application.Abstractions.Services;
using Tickefy.Application.AI.Dtos;


namespace Tickefy.Infrastructure.Services.AI
{

    public class AiService : IAiService
    {
        private readonly Client _client;

        public AiService(Client client)
        {
           _client = client;
        }

        public async Task<AiResponse> AnalyzeTicketAsync(string title, string description, DateTime deadline)
        {
             var prompt =
                 @$"You are an internal Ticketing AI classifier. 
                 Your task is to analyze the ticket details and assign a Category and Priority. 
                 Only return a JSON object.

                 Available Categories: IT, Design, Finance, HR, Legal, Content.
                 Available Priorities: Critical, High, Medium, Low.

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
           
            var json = response.Candidates[0].Content.Parts[0].Text;

           // var clean = Regex.Replace(json, "```.*?```", "", RegexOptions.Singleline);

            if (string.IsNullOrWhiteSpace(json))
                throw new InvalidOperationException("AI returned empty response.");

            var parsed = JsonSerializer.Deserialize<AiResponse>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return parsed ?? throw new InvalidOperationException("Invalid AI JSON format.");
        }

    }
}