using AutoMapper;
using Tickefy.Application.ActivityLogs.Common;

namespace Tickefy.Application.Mapping
{
    public class ActivityLogProfile : Profile
    {
        public ActivityLogProfile()
        {
            CreateMap<Domain.ActivityLogs.ActivityLog, LogResult>();
        }
    }
}
