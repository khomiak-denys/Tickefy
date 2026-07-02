using AutoMapper;
using Tickefy.API.ActivityLog.Responses;
using Tickefy.Application.ActivityLogs.Common;

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
