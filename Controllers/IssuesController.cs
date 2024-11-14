using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using IssueTracking.Models;
using System.Threading.Tasks;
using System.Linq;

namespace IssueTracking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "UserPolicy")]
    public class IssuesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public IssuesController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets all issues.
        /// </summary>
        /// <returns>A list of issues.</returns>
        [HttpGet]
        public async Task<IActionResult> GetIssues()
        {
            var issues = await _context.Issues.Include(i => i.Project).ToListAsync();
            return Ok(issues);
        }

        /// <summary>
        /// Gets an issue by ID.
        /// </summary>
        /// <param name="id">The ID of the issue.</param>
        /// <returns>The issue with the specified ID.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetIssue(int id)
        {
            var issue = await _context.Issues.Include(i => i.Project).FirstOrDefaultAsync(i => i.Id == id);
            if (issue == null)
            {
                return NotFound();
            }
            return Ok(issue);
        }

        /// <summary>
        /// Creates a new issue.
        /// </summary>
        /// <param name="model">The issue model.</param>
        /// <returns>The created issue.</returns>
        [HttpPost]
        public async Task<IActionResult> CreateIssue([FromBody] IssueModel model)
        {
            var project = await _context.Projects.FindAsync(model.ProjectId);
            if (project == null)
            {
                return BadRequest("Project not found");
            }

            var issue = new Issue
            {
                Title = model.Title,
                Description = model.Description,
                Status = model.Status,
                ProjectId = model.ProjectId
            };

            _context.Issues.Add(issue);
            await _context.SaveChangesAsync();

            // Add audit log entry
            var auditLog = new AuditLog
            {
                Action = "Create Issue",
                Username = User.Identity.Name,
                Timestamp = DateTime.UtcNow,
                Details = $"Issue {issue.Id} created"
            };
            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();

            return Ok(issue);
        }

        /// <summary>
        /// Updates an issue.
        /// </summary>
        /// <param name="id">The ID of the issue.</param>
        /// <param name="model">The issue model.</param>
        /// <returns>The updated issue.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateIssue(int id, [FromBody] IssueModel model)
        {
            var issue = await _context.Issues.FindAsync(id);
            if (issue == null)
            {
                return NotFound();
            }

            var project = await _context.Projects.FindAsync(model.ProjectId);
            if (project == null)
            {
                return BadRequest("Project not found");
            }

            issue.Title = model.Title;
            issue.Description = model.Description;
            issue.Status = model.Status;
            issue.ProjectId = model.ProjectId;

            _context.Issues.Update(issue);
            await _context.SaveChangesAsync();

            // Add audit log entry
            var auditLog = new AuditLog
            {
                Action = "Update Issue",
                Username = User.Identity.Name,
                Timestamp = DateTime.UtcNow,
                Details = $"Issue {issue.Id} updated"
            };
            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();

            return Ok(issue);
        }

        /// <summary>
        /// Deletes an issue.
        /// </summary>
        /// <param name="id">The ID of the issue.</param>
        /// <returns>A success message.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIssue(int id)
        {
            var issue = await _context.Issues.FindAsync(id);
            if (issue == null)
            {
                return NotFound();
            }

            _context.Issues.Remove(issue);
            await _context.SaveChangesAsync();

            // Add audit log entry
            var auditLog = new AuditLog
            {
                Action = "Delete Issue",
                Username = User.Identity.Name,
                Timestamp = DateTime.UtcNow,
                Details = $"Issue {issue.Id} deleted"
            };
            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Issue deleted successfully" });
        }

        /// <summary>
        /// Updates the status of an issue.
        /// </summary>
        /// <param name="id">The ID of the issue.</param>
        /// <param name="model">The issue status model.</param>
        /// <returns>The updated issue.</returns>
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateIssueStatus(int id, [FromBody] IssueStatusModel model)
        {
            var issue = await _context.Issues.FindAsync(id);
            if (issue == null)
            {
                return NotFound();
            }

            issue.Status = model.Status;

            _context.Issues.Update(issue);
            await _context.SaveChangesAsync();

            // Add audit log entry
            var auditLog = new AuditLog
            {
                Action = "Update Issue Status",
                Username = User.Identity.Name,
                Timestamp = DateTime.UtcNow,
                Details = $"Issue {issue.Id} status updated to {model.Status}"
            };
            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();

            return Ok(issue);
        }

        /// <summary>
        /// Adds a tag to an issue.
        /// </summary>
        /// <param name="id">The ID of the issue.</param>
        /// <param name="model">The issue tag model.</param>
        /// <returns>The updated issue.</returns>
        [HttpPost("{id}/tags")]
        public async Task<IActionResult> AddTagToIssue(int id, [FromBody] IssueTagModel model)
        {
            var issue = await _context.Issues.Include(i => i.Tags).FirstOrDefaultAsync(i => i.Id == id);
            if (issue == null)
            {
                return NotFound();
            }

            var tag = await _context.Tags.FindAsync(model.Id);
            if (tag == null)
            {
                return BadRequest("Tag not found");
            }

            issue.Tags.Add(tag);
            await _context.SaveChangesAsync();

            // Add audit log entry
            var auditLog = new AuditLog
            {
                Action = "Add Tag to Issue",
                Username = User.Identity.Name,
                Timestamp = DateTime.UtcNow,
                Details = $"Tag {tag.Id} added to Issue {issue.Id}"
            };
            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();

            return Ok(issue);
        }

        /// <summary>
        /// Removes a tag from an issue.
        /// </summary>
        /// <param name="id">The ID of the issue.</param>
        /// <param name="tagId">The ID of the tag.</param>
        /// <returns>The updated issue.</returns>
        [HttpDelete("{id}/tags/{tagId}")]
        public async Task<IActionResult> RemoveTagFromIssue(int id, int tagId)
        {
            var issue = await _context.Issues.Include(i => i.Tags).FirstOrDefaultAsync(i => i.Id == id);
            if (issue == null)
            {
                return NotFound();
            }

            var tag = issue.Tags.FirstOrDefault(t => t.Id == tagId);
            if (tag == null)
            {
                return BadRequest("Tag not found on issue");
            }

            issue.Tags.Remove(tag);
            await _context.SaveChangesAsync();

            // Add audit log entry
            var auditLog = new AuditLog
            {
                Action = "Remove Tag from Issue",
                Username = User.Identity.Name,
                Timestamp = DateTime.UtcNow,
                Details = $"Tag {tag.Id} removed from Issue {issue.Id}"
            };
            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();

            return Ok(issue);
        }

        /// <summary>
        /// Searches for issues based on various criteria.
        /// </summary>
        /// <param name="keyword">The keyword to search for in the title or description.</param>
        /// <param name="status">The status of the issues to search for.</param>
        /// <param name="tag">The tag to search for.</param>
        /// <param name="assignee">The assignee to search for.</param>
        /// <returns>A list of issues matching the search criteria.</returns>
        [HttpGet("search")]
        public async Task<IActionResult> SearchIssues([FromQuery] string keyword, [FromQuery] IssueStatus? status, [FromQuery] string tag, [FromQuery] string assignee)
        {
            var query = _context.Issues.Include(i => i.Project).Include(i => i.Tags).AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(i => i.Title.Contains(keyword) || i.Description.Contains(keyword));
            }

            if (status.HasValue)
            {
                query = query.Where(i => i.Status == status.Value);
            }

            if (!string.IsNullOrEmpty(tag))
            {
                query = query.Where(i => i.Tags.Any(t => t.Name == tag));
            }

            if (!string.IsNullOrEmpty(assignee))
            {
                query = query.Where(i => i.Project.Assignee == assignee);
            }

            var issues = await query.ToListAsync();
            return Ok(issues);
        }
    }

    public class IssueModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public IssueStatus Status { get; set; }
        public int ProjectId { get; set; }
    }

    public class IssueStatusModel
    {
        public IssueStatus Status { get; set; }
    }

    public class IssueTagModel
    {
        public int Id { get; set; }
    }
}
