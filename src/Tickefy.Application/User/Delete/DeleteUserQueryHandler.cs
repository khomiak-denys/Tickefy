using AutoMapper;
using Tickefy.Application.Abstractions.Data;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Abstractions.Repositories;
using Tickefy.Application.Exceptions;
using Tickefy.Domain.ActivityLog;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Tickefy.Application.User.Delete
{
    public class DeleteUserCommandHandler : ICommandHandler<DeleteUserCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public DeleteUserCommandHandler(
            IUserRepository userRepository,
            IUnitOfWork uow,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _uow = uow;
            _mapper = mapper;
        }
        public async Task Handle(DeleteUserCommand command, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetById(command.UserId);
            if (user == null) throw new NotFoundException(nameof(user), command.UserId.ToString());

            _userRepository.Delete(user);

            await _uow.SaveChangesAsync();
        }
    }
}
