using System.Data;
using Tickefy.Application.Abstractions.Data;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Abstractions.Repositories;
using Tickefy.Application.Exceptions;
using Tickefy.Domain.Common.UserRole;
using Tickefy.Domain.Primitives;
using Tickefy.Domain.Team;
using Tickefy.Domain.User;

namespace Tickefy.Application.Team.Create
{
    // Припускаємо, що команда повертає TeamId
    public class CreateTeamCommandHandler : ICommandHandler<CreateTeamCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly ITeamRepository _teamRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateTeamCommandHandler(
            IUserRepository userRepository,
            ITeamRepository teamRepository,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _teamRepository = teamRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(CreateTeamCommand command, CancellationToken cancellationToken)
        {
            var manager = await _userRepository.GetByIdAsync(command.UserId);

            if (manager is null)
            {
                throw new NotFoundException("User (Manager) not found.");
            }

            var team = Domain.Team.Team.Create(
                name: command.Name,
                description: command.Description
            );

            team.SetManager(manager.Id);
            team.SetCategory(command.Category);

            team.AddMember(manager);

            manager.SetRole(UserRoles.Manager);

            _teamRepository.Add(team);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}