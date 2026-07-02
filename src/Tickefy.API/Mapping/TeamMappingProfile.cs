using AutoMapper;
using Tickefy.API.Team.Responses;
using Tickefy.Application.Teams.Common;

namespace Tickefy.API.Mapping
{
    public class TeamMappingProfile : Profile
    {

        public TeamMappingProfile()
        {
            CreateMap<TeamResult, TeamResponse>();
            CreateMap<TeamDetailsResult, TeamDetailResponse>();
        }
    }
}
