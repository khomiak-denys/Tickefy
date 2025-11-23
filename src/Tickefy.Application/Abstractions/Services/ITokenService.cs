using Tickefy.Domain.Common.UserRole;

namespace Tickefy.Application.Abstractions.Services
{
    public interface ITokenService
    {
        public Task<string> GetToken(Guid id, string login, UserRoles role);
    }
}
