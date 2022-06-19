using System.ComponentModel.DataAnnotations;

namespace ActivityTrackingAPI.Models
{
    public class Activity : ActivityPatch
    {
        [Key]
        public string Id { get; set; }
        [Required]
        public string FirmId { get; set; }
        [Required]
        public ActivityType Type { get; set; }
        public List<Attachment> Attachments { get; set; } = new List<Attachment>();

        public bool ShouldSerializeAttachments()
        {
            return Type == ActivityType.Email;
        }
    }

    public class ActivityPatch
    {
        public string Name { get; set; }
        public DateTime DateTimeStarted { get; set; }
        public DateTime DateTimeFinished { get; set; }
        public TimeSpan ElapsedTime { get; set; }
    }

    public class Attachment
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public enum ActivityType
    {
        PhoneCall,
        Email,
        Document,
        Appointment
    }
}
