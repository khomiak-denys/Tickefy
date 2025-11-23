using Tickefy.Application.AI.Dtos;

namespace Tickefy.Application.Abstractions.Services
{
    public interface IAiService
    {
        Task<AiResponse> AnalyzeTicketAsync(string title, string description, DateTime deadline);

    }
}
