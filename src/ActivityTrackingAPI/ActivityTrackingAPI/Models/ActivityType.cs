namespace ActivityTrackingAPI.Models
{
    public class ActivityTypeItem
    {
        public ActivityType Type { get; set; }
        public TimeSpan TotalElapsedTime { get; set; }
        public List<Activity> Activities { get; set; }
    }
}
