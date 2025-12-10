using AutoMapper;
using MediatR;
using Tickefy.Application.Abstractions.Data;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Exceptions;
using Tickefy.Domain.Common.UserRole;
using Tickefy.Domain.Team;
using Tickefy.Domain.User;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Tickefy.Application.Team.RemoveMember
{
    public class RemoveMemberCommandHandler : ICommandHandler<RemoveMemberCommand, Unit>
    {
        private readonly ITeamRepository _teamRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _uow;
        public RemoveMemberCommandHandler(
            ITeamRepository teamRepository,
            IUserRepository userRepository,
            IUnitOfWork uow)
        {
            _teamRepository = teamRepository;
            _userRepository = userRepository;
            _uow = uow;
        }
        public async Task<Unit> Handle(RemoveMemberCommand command, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(command.MemberId);
            if (user == null) throw new NotFoundException(nameof(user), command.MemberId.Value);
            if (user.Role == UserRoles.Admin || user.Role == UserRoles.Manager) throw new ForbiddenException("Cant remove manager or admin to team");

            var team = await _teamRepository.GetByIdAsync(command.TeamId);

            if (team == null) throw new NotFoundException(nameof(team), command.ManagerId.Value);

            var manager = await _userRepository.GetByIdAsync(command.ManagerId);
            if (manager == null) throw new NotFoundException(nameof(manager), command.ManagerId.Value);
            if (team.ManagerId != command.ManagerId && manager.Role != UserRoles.Admin) throw new ForbiddenException("Become a manager to remove users");

            team.RemoveMember(user);
            user.SetRole(UserRoles.Requester);

            await _uow.SaveChangesAsync();
            
            return Unit.Value;
        }
    }
}
