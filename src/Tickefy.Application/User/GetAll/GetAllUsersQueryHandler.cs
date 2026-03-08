using AutoMapper;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.User.Common;
using Tickefy.Domain.Common.Results;
using Tickefy.Domain.User;

namespace Tickefy.Application.User.GetAll
{
    public class GetAllUsersQueryHandler : IQueryHandler<GetAllUsersQuery, Result<List<UserDetailsResult>>>
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
        public async Task<Result<List<UserDetailsResult>>> Handle(GetAllUsersQuery query, CancellationToken cancellationToken)
        {
            var tickets = await _userRepository.GetAll();
            var result = _mapper.Map<List<UserDetailsResult>>(tickets);

            return Result<List<UserDetailsResult>>.Success(result);
        }
    }
}
