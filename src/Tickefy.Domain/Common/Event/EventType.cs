namespace Tickefy.Domain.Common.Event
{
    public enum EventType
    {
        RequestCreated,
        StatusChanged,
        PriorityChanged,
        DeadlineChanged,
        TeamAssigned,
        UserAssigned,
        CommentAdded
    }
}
