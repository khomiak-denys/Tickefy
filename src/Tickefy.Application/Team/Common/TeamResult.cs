using Tickefy.Application.User.Common;

namespace Tickefy.Application.Team.Common
{
    public record TeamResult
     (
         Guid Id,
         string Name,
         string Category,
         UserResult Manager
     );
}
