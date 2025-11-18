using Tickefy.Domain.Common.EntityBase;
using Tickefy.Domain.Common.UserRole;
using Tickefy.Domain.Primitives;

namespace Tickefy.Domain.User   
{
    public class User : EntityBase<UserId>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public UserRoles Role { get; set; }
    }
}
