using Tickefy.Application.Users.Common;

namespace Tickefy.Application.Teams.Common
{
    public record TeamResult
     (
         Guid Id,
         string Name,
         string Category,
         UserResult Manager
     );
}
