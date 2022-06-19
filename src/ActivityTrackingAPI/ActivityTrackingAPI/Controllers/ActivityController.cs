using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ActivityTrackingAPI.Data;
using ActivityTrackingAPI.Models;
using Microsoft.AspNetCore.JsonPatch;
using Newtonsoft.Json;

namespace ActivityTrackingAPI.Controllers
{
    [Route("api/v1/activity")]
    [ApiController]
    public class ActivityController : ControllerBase
    {
        private readonly ActivityContext _context;

        public ActivityController(ActivityContext context)
        {
            _context = context;
        }

        [HttpGet("types/{startDate}/{endDate}")]
        public async Task<ActionResult<IEnumerable<Activity>>> GetActivityTypesDateRange(DateTime startDate, DateTime endDate)
        {
            if (_context.Activities == null)
            {
                return NotFound();
            }

            var activities = await _context.Activities
                .Where(a => a.DateTimeStarted > startDate && a.DateTimeEnded < endDate)
                .ToArrayAsync();

            var activityTypes = activities
                .GroupBy(a => a.Type)
                .Select(s => new
                {
                    Type = Enum.GetName(typeof(ActivityType), s.Key),
                    Duration = s.Sum(r => r.TimeSpan.Ticks),
                    Activities = s.ToList()
                });

            return new ObjectResult(activityTypes);
        }

        // PATCH : api/v1/Activity/5
        [HttpPatch]
        public async Task<IActionResult> PatchActivity(string id, [FromBody] JsonPatchDocument<ActivityPatch> patchDocument)
        {
            if (patchDocument != null)
            {
                var activity = await _context.Activities.FindAsync(id);
                if (activity == null)
                {
                    return NotFound();
                }

                patchDocument.ApplyToSafely(activity, ModelState);

                if (!ModelState.IsValid)
                {
                    return ValidationProblem();
                }

                await _context.SaveChangesAsync();
                return new ObjectResult(activity);
            }
            else
            {
                return BadRequest();
            }
        }

        // POST: api/Activity
        [HttpPost]
        public async Task<ActionResult<Activity>> PostActivity(Activity activity)
        {
            if (_context.Activities == null)
            {
                return Problem("Entity set 'ActivityContext.Activities'  is null.");
            }

            if (activity.Type != ActivityType.Email && activity.Attachments.Count > 0)
            {
                return BadRequest("Cannot submit attachments to activity type which is not email.");
            }

            _context.Activities.Add(activity);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException) when (ActivityExists(activity.Id))
            {
                return Conflict();
            }

            return CreatedAtAction("GetActivity", new { id = activity.Id }, activity);
        }

        private bool ActivityExists(string id)
        {
            return (_context.Activities?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
