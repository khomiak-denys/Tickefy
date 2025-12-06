using AutoMapper;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.User.Common;
using Tickefy.Domain.User;

namespace Tickefy.Application.User.GetAll
{
    public class GetAllUsersQueryHandler : IQueryHandler<GetAllUsersQuery, List<UserDetailsResult>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public GetAllUsersQueryHandler(
            IUserRepository userRepository, 
            IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }
        public async Task<List<UserDetailsResult>> Handle(GetAllUsersQuery query, CancellationToken cancellationToken)
        {
            var tickets = await _userRepository.GetAll();
            var result = _mapper.Map<List<UserDetailsResult>>(tickets);

            return result;
        }
    }
}
