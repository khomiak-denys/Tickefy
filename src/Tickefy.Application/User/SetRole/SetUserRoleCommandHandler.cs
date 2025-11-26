using Tickefy.Application.Abstractions.Data;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Abstractions.Repositories;
using Tickefy.Application.Exceptions;
using Tickefy.Domain.Common.UserRole;

namespace Tickefy.Application.User.SetRole
{
    public class SetUserRoleCommandHandler : ICommandHandler<SetUserRoleCommand>
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
        public async Task Handle(SetUserRoleCommand command, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(command.UserId);
            if (user == null) throw new NotFoundException(nameof(user), command.UserId.ToString());

            UserRoles role;

            var result = Enum.TryParse<UserRoles>(command.Role, ignoreCase: true, out role);

            if (!result)
            {
                role = UserRoles.Requester;
            }

            if (user.Role == UserRoles.Admin) throw new ForbiddenException("Admin role cant be changed");



            user.SetRole(role);

            await _uow.SaveChangesAsync();
        }
    }
}
