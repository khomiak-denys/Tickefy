using AutoMapper;
using Tickefy.Application.Team.Common;

namespace Tickefy.Application.Mapping
{
    public class TeamProfile : Profile 
    {
        public TeamProfile() 
        {
            CreateMap<Domain.Team.Team, TeamDetailsResult>()
                .ForMember(
                    dest => dest.Id,
                    opt => opt.MapFrom(src => src.Id.Value)) 

                .ForMember(
                    dest => dest.Category,
                    opt => opt.MapFrom(src => src.Category.ToString()))

                .ForMember(
                    dest => dest.ManagerId,
                    opt => opt.MapFrom(src => src.ManagerId.Value)) 

                .ForMember(
                    dest => dest.Members,
                    opt => opt.MapFrom(src => src.Members));

            CreateMap<Domain.Team.Team, TeamResult>()
                .ForMember(
                    dest => dest.Id,
                    opt => opt.MapFrom(src => src.Id.Value))
                .ForMember(
                    dest => dest.ManagerId,
                    opt => opt.MapFrom(src => src.ManagerId.Value));
        }
    }
}
