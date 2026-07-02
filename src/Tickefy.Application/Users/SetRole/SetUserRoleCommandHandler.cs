using Tickefy.Application.Abstractions.Data;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Domain.Common.Errors;
using Tickefy.Domain.Common.Results;
using Tickefy.Domain.Common.UserRole;
using Tickefy.Domain.Users;

namespace Tickefy.Application.Users.SetRole
{
    public class SetUserRoleCommandHandler : ICommandHandler<SetUserRoleCommand, Result>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _uow;
        public SetUserRoleCommandHandler(
            IUserRepository userRepository,
            IUnitOfWork uow)
        {
            _userRepository = userRepository;
            _uow = uow;
        }
        public async Task<Result> Handle(SetUserRoleCommand command, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(command.UserId, cancellationToken);
            if (user == null) return Result.Failure(new NotFoundError(nameof(user) + " " + command.UserId.ToString()));

            UserRoles role;
            var result = Enum.TryParse<UserRoles>(command.Role, ignoreCase: true, out role);

            if (!result)
            {
                role = UserRoles.Requester;
            }

            if (user.Role == UserRoles.Admin) return Result.Failure(new ForbiddenError("Admin role cant be changed"));

            user.SetRole(role);

            await _uow.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
