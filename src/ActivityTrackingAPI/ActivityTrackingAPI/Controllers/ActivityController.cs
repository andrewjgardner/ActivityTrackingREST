using Microsoft.AspNetCore.Mvc;
using ActivityTrackingAPI.Models;
using Microsoft.AspNetCore.JsonPatch;
using ActivityTrackingAPI.Services;

namespace ActivityTrackingAPI.Controllers
{
    [Route("api/v1/activity")]
    [ApiController]
    public class ActivityController : ControllerBase
    {
        private readonly ActivityService _activityService;

        public ActivityController(ActivityService activityService)
        {
            _activityService = activityService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Activity>> GetActivity(string id)
        {
            if (!_activityService.ActivitiesExist())
            {
                return Problem("Entity set 'ActivityContext.Activities'  is null.");
            }

            var activity = await _activityService.GetActivityAsync(id);

            if (activity == null)
            {
                return NotFound();
            }

            return new ObjectResult(activity);
        }

        [HttpGet("types/{startDate}/{endDate}")]
        public async Task<ActionResult<IEnumerable<ActivityTypeItem>>> GetActivityTypesDateRange(DateTime startDate, DateTime endDate)
        {
            if (!_activityService.ActivitiesExist())
            {
                return Problem("Entity set 'ActivityContext.Activities'  is null.");
            }

            IEnumerable<ActivityTypeItem>? activityTypes = await _activityService.GetActivityTypesAsync(startDate, endDate);

            return new ObjectResult(activityTypes);
        }

        [HttpPatch]
        public async Task<ActionResult<Activity>> PatchActivity(string id, [FromBody] JsonPatchDocument<ActivityPatch> patchDocument)
        {
            if (!_activityService.ActivitiesExist())
            {
                return Problem("Entity set 'ActivityContext.Activities'  is null.");
            }

            if (patchDocument != null)
            {
                Activity? activity = await _activityService.GetActivityAsync(id);
                if (activity == null)
                {
                    return NotFound();
                }

                patchDocument.ApplyToSafely(activity, ModelState);

                if (!ModelState.IsValid)
                {
                    return ValidationProblem();
                }

                await _activityService.Save();
                return new ObjectResult(activity);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<ActionResult<Activity>> PostActivity(Activity activity)
        {
            if (!_activityService.ActivitiesExist())
            {
                return Problem("Entity set 'ActivityContext.Activities'  is null.");
            }

            if (activity.Type != ActivityType.Email && activity.Attachments.Count > 0)
            {
                return BadRequest("Cannot submit attachments to activity type which is not email.");
            }

            try
            {
                _activityService.CreateActivity(activity);
                await _activityService.Save();
            }
            catch when (_activityService.ActivityExists(activity.Id))
            {
                return Conflict();
            }

            return CreatedAtAction("GetActivity", new { id = activity.Id }, activity);
        }

    }
}
