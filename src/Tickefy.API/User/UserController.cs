using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tickefy.API.User.Responses;
using Tickefy.Application.Ticket.GetAll;
using Tickefy.Application.User.GetAll;

namespace Tickefy.API.User
{
    [ApiController]
    [Route("api/v1/users")]
    public class UserController : ControllerBase 
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserController(
            IMediator mediator,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
        {
            _mediator = mediator;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }


        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllAsync()
        {
            var query = new GetAllUsersQuery();
            var result = await _mediator.Send(query);

            var response = _mapper.Map<List<UserResponse>>(result);
            return Ok(response);
        }
    }
}
