using Microsoft.Extensions.Logging;
using Tickefy.Application.Abstractions.Data;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Domain.Common.Errors;
using Tickefy.Domain.Common.Results;
using Tickefy.Domain.Common.UserRole;
using Tickefy.Domain.Teams;
using Tickefy.Domain.Users;

namespace Tickefy.Application.Teams.Create
{
    public class CreateTeamCommandHandler : ICommandHandler<CreateTeamCommand, Result>
    {
        private readonly IUserRepository _userRepository;
        private readonly ITeamRepository _teamRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateTeamCommandHandler> _logger;

        public CreateTeamCommandHandler(
            IUserRepository userRepository,
            ITeamRepository teamRepository,
            IUnitOfWork unitOfWork,
            ILogger<CreateTeamCommandHandler> logger)
        {
            _userRepository = userRepository;
            _teamRepository = teamRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result> Handle(CreateTeamCommand command, CancellationToken cancellationToken)
        {
            var manager = await _userRepository.GetByIdAsync(command.UserId, cancellationToken);

            if (manager is null)
            {
                _logger.LogWarning("Team creation failed: manager {UserId} not found", command.UserId.Value);
                return Result.Failure(new NotFoundError("User (Manager) not found."));
            }

            var existingTeam = await _teamRepository.GetByNameAsync(command.Name, cancellationToken);
            if (existingTeam is not null)
            {
                _logger.LogWarning("Team creation failed: team with name {TeamName} already exists", command.Name);
                return Result.Failure(new AlreadyExistsError(existingTeam.Name));
            }

            var team = Domain.Teams.Team.Create(
                name: command.Name,
                description: command.Description
            );

            team.SetManager(manager.Id);
            team.SetCategory(command.Category);

            team.AddMember(manager);

            manager.SetRole(UserRoles.Manager);

            _teamRepository.Add(team);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Team {TeamName} created successfully with manager {ManagerId}", team.Name, manager.Id.Value);

            return Result.Success();
        }
    }
}
