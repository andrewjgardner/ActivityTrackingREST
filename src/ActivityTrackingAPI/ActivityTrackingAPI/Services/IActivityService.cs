using ActivityTrackingAPI.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ActivityTrackingAPI.Services
{
    public interface IActivityService
    {
        bool ActivitiesExist();
        bool ActivityExists(string id);
        Task<IEnumerable<ActivityTypeItem>> GetActivityTypesAsync(DateTime startDate, DateTime endDate);
        Task<Activity> GetActivityAsync(string id);
        Task<Activity> CreateActivityAsync(Activity activity);
        Task Save();
    }
}
