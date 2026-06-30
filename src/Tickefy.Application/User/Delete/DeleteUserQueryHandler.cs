using Tickefy.Application.Abstractions.Data;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Domain.Common.Errors;
using Tickefy.Domain.Common.Results;
using Tickefy.Domain.User;

namespace Tickefy.Application.User.Delete
{
    public class DeleteUserCommandHandler : ICommandHandler<DeleteUserCommand, Result>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _uow;

        public DeleteUserCommandHandler(
            IUserRepository userRepository,
            IUnitOfWork uow)
        {
            _userRepository = userRepository;
            _uow = uow;
        }
        public async Task<Result> Handle(DeleteUserCommand command, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(command.UserId, cancellationToken);
            if (user == null) return Result.Failure(new NotFoundError(nameof(user) + " " + command.UserId.ToString()));

            _userRepository.Delete(user);

            await _uow.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
