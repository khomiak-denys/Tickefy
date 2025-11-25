using AutoMapper;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Abstractions.Repositories;
using Tickefy.Application.User.Common;

namespace Tickefy.Application.User.GetAll
{
    public class GetAllUsersQueryHandler : IQueryHandler<GetAllUsersQuery, List<UserResult>>
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
        public async Task<List<UserResult>> Handle(GetAllUsersQuery query, CancellationToken cancellationToken)
        {
            var tickets = await _userRepository.GetAll();
            var result = _mapper.Map<List<UserResult>>(tickets);

            return result;
        }
    }
}
