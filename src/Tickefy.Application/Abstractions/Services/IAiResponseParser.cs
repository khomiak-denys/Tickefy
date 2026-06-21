using Tickefy.Application.AI.Dtos;
using Tickefy.Domain.Common.Category;
using Tickefy.Domain.Common.Priority;

namespace Tickefy.Application.Abstractions.Services
{
    /// <summary>
    /// Parses AI responses into domain-specific ticket metadata.
    /// </summary>
    public interface IAiResponseParser
    {
        /// <summary>
        /// Parses a ticket category from an AI response.
        /// </summary>
        /// <param name="response">The AI response to parse.</param>
        public Category ParseCategory(AiResponse response);

        /// <summary>
        /// Parses a ticket priority from an AI response.
        /// </summary>
        /// <param name="response">The AI response to parse.</param>
        public Priority ParsePriority(AiResponse response);
    }
}
