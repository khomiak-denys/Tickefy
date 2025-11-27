using AutoMapper;
using Tickefy.API.ActivityLog.Responses;
using Tickefy.Application.ActivityLog.Common;

namespace Tickefy.API.Mapping
{
    public class LogMappingProfile : Profile
    {
        public LogMappingProfile() 
        {
            CreateMap<LogResult, LogResponse>();
        }
    }
}
