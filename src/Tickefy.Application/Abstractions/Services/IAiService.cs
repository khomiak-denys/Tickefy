using Tickefy.Application.AI.Dtos;

namespace Tickefy.Application.Abstractions.Services
{
    /// <summary>
    /// Provides AI-based ticket analysis.
    /// </summary>
    public interface IAiService
    {
        /// <summary>
        /// Analyzes ticket details and returns suggested metadata.
        /// </summary>
        /// <param name="title">The ticket title.</param>
        /// <param name="description">The ticket description.</param>
        /// <param name="deadline">The ticket deadline.</param>
        Task<AiResponse> AnalyzeTicketAsync(string title, string description, DateTime deadline);

    }
}
