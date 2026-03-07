using Tickefy.Application.Abstractions.Data;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Abstractions.Services;
using Tickefy.Application.Exceptions;
using Tickefy.Domain.Common.Errors;
using Tickefy.Domain.Common.Results;
using Tickefy.Domain.User;

namespace Tickefy.Application.Auth.Register
{
    public class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand, Result<Guid>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _uow;
        private readonly IPasswordHasher _passwordHasher;
        public RegisterUserCommandHandler(
            IUserRepository userRepository,
            IUnitOfWork uow,
            IPasswordHasher passwordHasher
            )
        {
            _userRepository = userRepository;
            _uow = uow;
            _passwordHasher = passwordHasher;
        }
        public async Task<Result<Guid>> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
        {
            var existingUser = await _userRepository.GetByLoginAsync(command.Login);
            if (existingUser != null)
            {
                return Result<Guid>.Failure(new AlreadyExistsError("User"));
            }
            var passwordHash = _passwordHasher.HashPassword(command.Password);
            var user = Domain.User.User.Create(command.FirstName, command.LastName, command.Login, passwordHash);

            _userRepository.Add(user);
            await _uow.SaveChangesAsync(cancellationToken);
            return Result<Guid>.Success(user.Id.Value);
        }
    }
}
