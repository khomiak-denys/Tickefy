using AutoMapper;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Abstractions.Repositories;
using Tickefy.Application.Exceptions;
using Tickefy.Application.User.Common;

namespace Tickefy.Application.User.GetById
{
    public class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, UserResult>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public GetUserByIdQueryHandler(
            IUserRepository userRepository,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }
        public async Task<UserResult> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(query.UserId);
            if (user == null) throw new NotFoundException(nameof(user), query.UserId.ToString());

            var result = _mapper.Map<UserResult>(user);
            return result;
        }
    }
}
