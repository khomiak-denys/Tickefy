using Microsoft.Extensions.Logging;
using Tickefy.Application.Abstractions.Data;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Domain.Common.Errors;
using Tickefy.Domain.Common.Results;
using Tickefy.Domain.Users;

namespace Tickefy.Application.Users.Delete
{
    public class DeleteUserCommandHandler : ICommandHandler<DeleteUserCommand, Result>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<DeleteUserCommandHandler> _logger;

        public DeleteUserCommandHandler(
            IUserRepository userRepository,
            IUnitOfWork uow,
            ILogger<DeleteUserCommandHandler> logger)
        {
            _userRepository = userRepository;
            _uow = uow;
            _logger = logger;
        }
        public async Task<Result> Handle(DeleteUserCommand command, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(command.UserId, cancellationToken);
            if (user == null)
            {
                _logger.LogWarning("Delete user failed: user {UserId} not found", command.UserId.Value);
                return Result.Failure(new NotFoundError(nameof(user) + " " + command.UserId.ToString()));
            }

            _userRepository.Delete(user);

            await _uow.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("User {UserId} deleted successfully", command.UserId.Value);

            return Result.Success();
        }
    }
}
