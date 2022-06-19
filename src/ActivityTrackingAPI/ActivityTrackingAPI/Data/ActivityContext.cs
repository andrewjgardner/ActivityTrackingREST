using ActivityTrackingAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ActivityTrackingAPI.Data
{
    public class ActivityContext : DbContext
    {
        public ActivityContext(DbContextOptions<ActivityContext> options) : base(options)
        {
        }

        public DbSet<Activity> Activities { get; set; } = null!;
    }
}
