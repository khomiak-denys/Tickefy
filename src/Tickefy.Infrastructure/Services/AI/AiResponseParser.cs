using Tickefy.Application.Abstractions.Services;
using Tickefy.Application.AI.Dtos;
using Tickefy.Domain.Common.Category;
using Tickefy.Domain.Common.Priority;

namespace Tickefy.Infrastructure.Services.AI
{
    public class AiResponseParser : IAiResponseParser
    {
        public Category ParseCategory(AiResponse response)
        {
            if (Enum.TryParse<Category>(response.Category, ignoreCase: true, out var category))
            {
                return category;
            }

            return Category.Other; 
        }

        public Priority ParsePriority(AiResponse response)
        {
            if (Enum.TryParse<Priority>(response.Priority, ignoreCase: true, out var priority))
            {
                return priority;
            }

            return Priority.Medium; 
        }
    }
}
