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

        // GET: api/Activity
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Activity>>> GetActivities()
        {
            if (_context.Activities == null)
            {
                return NotFound();
            }
            return await _context.Activities.Include(a => a.Attachments).ToListAsync();
        }

        // GET: api/Activity/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Activity>> GetActivity(string id)
        {
            if (_context.Activities == null)
            {
                return NotFound();
            }
            var activity = await _context.Activities.Include(a => a.Attachments).FirstOrDefaultAsync(a => a.Id == id);

            if (activity == null)
            {
                return NotFound();
            }

            return activity;
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

        // PUT: api/Activity/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutActivity(string id, Activity activity)
        {
            if (id != activity.Id)
            {
                return BadRequest();
            }

            _context.Entry(activity).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!ActivityExists(id))
            {
                return NotFound();
            }

            return NoContent();
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

        // DELETE: api/Activity/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteActivity(string id)
        {
            if (_context.Activities == null)
            {
                return NotFound();
            }
            var activity = await _context.Activities.FindAsync(id);
            if (activity == null)
            {
                return NotFound();
            }

            _context.Activities.Remove(activity);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ActivityExists(string id)
        {
            return (_context.Activities?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
