using AutoMapper;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Exceptions;
using Tickefy.Application.User.Common;
using Tickefy.Domain.User;

namespace Tickefy.Application.User.GetByLogin
{
    public class GetUserByLoginQueryHandler : IQueryHandler<GetUserByLoginQuery, UserDetailsResult>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public GetUserByLoginQueryHandler(
            IUserRepository userRepository,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }
        public async Task<UserDetailsResult> Handle(GetUserByLoginQuery query, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByLoginAsync(query.Login);
            if (user == null) throw new NotFoundException(nameof(user), query.Login);

            var result = _mapper.Map<UserDetailsResult>(user);
            return result;
        }
    }
}