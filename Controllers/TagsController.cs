using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using IssueTracking.Models;
using System.Threading.Tasks;

namespace IssueTracking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TagsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TagsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets all tags.
        /// </summary>
        /// <returns>A list of tags.</returns>
        [HttpGet]
        public async Task<IActionResult> GetTags()
        {
            var tags = await _context.Tags.ToListAsync();
            return Ok(tags);
        }

        /// <summary>
        /// Gets a tag by ID.
        /// </summary>
        /// <param name="id">The ID of the tag.</param>
        /// <returns>The tag with the specified ID.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTag(int id)
        {
            var tag = await _context.Tags.FindAsync(id);
            if (tag == null)
            {
                return NotFound();
            }
            return Ok(tag);
        }

        /// <summary>
        /// Creates a new tag.
        /// </summary>
        /// <param name="model">The tag model.</param>
        /// <returns>The created tag.</returns>
        [HttpPost]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> CreateTag([FromBody] TagModel model)
        {
            var tag = new Tag
            {
                Name = model.Name,
                Description = model.Description
            };

            _context.Tags.Add(tag);
            await _context.SaveChangesAsync();

            // Add audit log entry
            var auditLog = new AuditLog
            {
                Action = "Create Tag",
                Username = User.Identity.Name,
                Timestamp = DateTime.UtcNow,
                Details = $"Tag {tag.Id} created"
            };
            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();

            return Ok(tag);
        }

        /// <summary>
        /// Updates an existing tag.
        /// </summary>
        /// <param name="id">The ID of the tag.</param>
        /// <param name="model">The tag model.</param>
        /// <returns>The updated tag.</returns>
        [HttpPut("{id}")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> UpdateTag(int id, [FromBody] TagModel model)
        {
            var tag = await _context.Tags.FindAsync(id);
            if (tag == null)
            {
                return NotFound();
            }

            tag.Name = model.Name;
            tag.Description = model.Description;

            _context.Tags.Update(tag);
            await _context.SaveChangesAsync();

            // Add audit log entry
            var auditLog = new AuditLog
            {
                Action = "Update Tag",
                Username = User.Identity.Name,
                Timestamp = DateTime.UtcNow,
                Details = $"Tag {tag.Id} updated"
            };
            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();

            return Ok(tag);
        }

        /// <summary>
        /// Deletes a tag.
        /// </summary>
        /// <param name="id">The ID of the tag.</param>
        /// <returns>A success message.</returns>
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> DeleteTag(int id)
        {
            var tag = await _context.Tags.FindAsync(id);
            if (tag == null)
            {
                return NotFound();
            }

            _context.Tags.Remove(tag);
            await _context.SaveChangesAsync();

            // Add audit log entry
            var auditLog = new AuditLog
            {
                Action = "Delete Tag",
                Username = User.Identity.Name,
                Timestamp = DateTime.UtcNow,
                Details = $"Tag {tag.Id} deleted"
            };
            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Tag deleted successfully" });
        }
    }

    public class TagModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
