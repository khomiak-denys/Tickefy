using Microsoft.Extensions.Logging;
using Tickefy.Application.Abstractions.Data;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Domain.Common.Errors;
using Tickefy.Domain.Common.Results;
using Tickefy.Domain.Users;

namespace Tickefy.Application.Users.UpdateProfile
{
    public class UpdateProfileCommandHandler : ICommandHandler<UpdateProfileCommand, Result>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<UpdateProfileCommandHandler> _logger;

        public UpdateProfileCommandHandler(
            IUserRepository userRepository,
            IUnitOfWork uow,
            ILogger<UpdateProfileCommandHandler> logger)
        {
            _userRepository = userRepository;
            _uow = uow;
            _logger = logger;
        }

        public async Task<Result> Handle(UpdateProfileCommand command, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(command.UserId, cancellationToken);
            if (user == null)
            {
                _logger.LogWarning("Update profile failed: user {UserId} not found", command.UserId.Value);
                return Result.Failure(new NotFoundError(nameof(user) + " " + command.UserId));
            }

            user.Update(command.FirstName ?? user.FirstName, command.LastName ?? user.LastName);

            await _uow.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Profile updated successfully for user {UserId}", command.UserId.Value);

            return Result.Success();
        }
    }
}
