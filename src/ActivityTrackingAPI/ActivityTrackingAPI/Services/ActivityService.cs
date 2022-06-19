using ActivityTrackingAPI.Data;
using ActivityTrackingAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ActivityTrackingAPI.Services
{
    public class ActivityService : IActivityService
    {
        private readonly ActivityContext _context;

        public ActivityService(ActivityContext context)
        {
            _context = context;
        }

        public bool ActivitiesExist()
        {
            return _context.Activities != null;
        }

        public bool ActivityExists(string id)
        {
            return (_context.Activities?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public void CreateActivity(Activity activity)
        {
            _context.Activities.Add(activity);
        }

        public async Task<Activity?> GetActivityAsync(string id)
        {
            return await _context.Activities.Include(a => a.Attachments).FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<ActivityTypeItem>> GetActivityTypesAsync(DateTime startDate, DateTime endDate)
        {
            var activities = await _context.Activities
                .Include(a => a.Attachments)
                .Where(a => a.DateTimeStarted > startDate && a.DateTimeFinished < endDate)
                .ToArrayAsync();

            return activities
                .GroupBy(a => a.Type)
                .Select(s => new ActivityTypeItem
                {
                    Type = s.Key,
                    TotalElapsedTime = new TimeSpan(s.Sum(r => r.ElapsedTime.Ticks)),
                    Activities = s.ToList()
                });
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }
    }
}
