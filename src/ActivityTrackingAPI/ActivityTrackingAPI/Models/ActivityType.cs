namespace ActivityTrackingAPI.Models
{
    public class ActivityTypeItem
    {
        public ActivityType Type { get; set; }
        public TimeSpan Duration { get; set; }
        public List<Activity> Activities { get; set; }
    }
}
