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

        public CreateTeamCommandHandler(
            IUserRepository userRepository,
            ITeamRepository teamRepository,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _teamRepository = teamRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(CreateTeamCommand command, CancellationToken cancellationToken)
        {
            var manager = await _userRepository.GetByIdAsync(command.UserId, cancellationToken);

            if (manager is null)
            {
                return Result.Failure(new NotFoundError("User (Manager) not found."));
            }

            var existingTeam = await _teamRepository.GetByNameAsync(command.Name, cancellationToken);
            if (existingTeam is not null) return Result.Failure(new AlreadyExistsError(existingTeam.Name));

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

            return Result.Success();
        }
    }
}
