using AutoMapper;
using Tickefy.Application.ActivityLog.Common;

namespace Tickefy.Application.Mapping
{
    public class ActivityLogProfile : Profile
    {
        public ActivityLogProfile()
        {
            CreateMap<Domain.ActivityLog.ActivityLog, LogResult>();
        }
    }
}
