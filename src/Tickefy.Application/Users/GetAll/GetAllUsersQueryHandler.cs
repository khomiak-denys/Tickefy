using Microsoft.Extensions.Logging;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Users.Common;
using Tickefy.Domain.Common.Results;
using Tickefy.Domain.Users;
using Tickefy.Application.Common.Models;

namespace Tickefy.Application.Users.GetAll
{
    public class GetAllUsersQueryHandler : IQueryHandler<GetAllUsersQuery, Result<PaginationResult<UserDetailsResult>>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<GetAllUsersQueryHandler> _logger;

        public GetAllUsersQueryHandler(
            IUserRepository userRepository,
            ILogger<GetAllUsersQueryHandler> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }
        public async Task<Result<PaginationResult<UserDetailsResult>>> Handle(GetAllUsersQuery query, CancellationToken cancellationToken)
        {
            var pagedData = await _userRepository.GetAllAsync(query.Page, query.PageSize, cancellationToken);

            var pagedUsers = pagedData.Items
                .Select(UserDetailsResult.FromEntity)
                .ToList();

            var result = PaginationResult<UserDetailsResult>.Create(pagedUsers, query.Page, query.PageSize, pagedData.TotalCount);

            _logger.LogInformation("Retrieved {Count} users (Total: {TotalCount}, Page: {Page}, PageSize: {PageSize})", pagedUsers.Count, pagedData.TotalCount, query.Page, query.PageSize);

            return Result<PaginationResult<UserDetailsResult>>.Success(result);
        }
    }
}
