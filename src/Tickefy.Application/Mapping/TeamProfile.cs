using AutoMapper;
using Tickefy.Application.Team.Common;
using Tickefy.Domain.Primitives;

namespace Tickefy.Application.Mapping
{
    public class TeamProfile : Profile 
    {
        public TeamProfile() 
        {
            CreateMap<Domain.Team.Team, TeamDetailsResult>();
            CreateMap<Domain.Team.Team, TeamResult>();
        }
    }
}
