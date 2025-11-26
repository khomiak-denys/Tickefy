using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tickefy.Application.Team.Common
{
    public record TeamResult
     (
         Guid Id,
         string Name,
         string Category,
         Guid ManagerId
     );
}
