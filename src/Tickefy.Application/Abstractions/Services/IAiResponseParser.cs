using Tickefy.Application.AI.Dtos;
using Tickefy.Domain.Common.Category;
using Tickefy.Domain.Common.Priority;

namespace Tickefy.Application.Abstractions.Services
{
    public interface IAiResponseParser
    {
        public Category ParseCategory(AiResponse response);
        public Priority ParsePriority(AiResponse response);
    }
}
