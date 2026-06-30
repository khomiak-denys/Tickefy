using AutoMapper;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.User.Common;
using Tickefy.Domain.Common.Errors;
using Tickefy.Domain.Common.Results;
using Tickefy.Domain.Users;

namespace Tickefy.Application.User.GetById
{
    public class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, Result<UserDetailsResult>>
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
        public async Task<Result<UserDetailsResult>> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(query.UserId, cancellationToken);
            if (user == null) return Result<UserDetailsResult>.Failure(new NotFoundError(nameof(user) + " " + query.UserId.ToString()));

            var result = _mapper.Map<UserDetailsResult>(user);
            return Result<UserDetailsResult>.Success(result);
        }
    }
}
