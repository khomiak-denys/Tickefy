using Microsoft.Extensions.Logging;
using Tickefy.Application.Abstractions.Services;
using Tickefy.Application.AI.Dtos;
using Tickefy.Domain.Common.Category;
using Tickefy.Domain.Common.Priority;

namespace Tickefy.Infrastructure.Services.AI
{
    public class AiResponseParser : IAiResponseParser
    {
        private readonly ILogger<AiResponseParser> _logger;

        public AiResponseParser(ILogger<AiResponseParser> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public Category ParseCategory(AiResponse response)
        {
            if (Enum.TryParse<Category>(response.Category, ignoreCase: true, out var category))
            {
                return category;
            }

            _logger.LogWarning("Failed to parse category from Ai response: {ResponseCategory}", response.Category);
            return Category.Other;
        }

        public Priority ParsePriority(AiResponse response)
        {
            if (Enum.TryParse<Priority>(response.Priority, ignoreCase: true, out var priority))
            {
                return priority;
            }

            _logger.LogWarning("Failed to parse priority from Ai response: {ResponsePriority}", response.Priority);
            return Priority.Medium;
        }
    }
}
