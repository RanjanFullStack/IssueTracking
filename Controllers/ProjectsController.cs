using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace IssueTracking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "AdminPolicy")]
    public class ProjectsController : ControllerBase
    {
        /// <summary>
        /// Gets all projects.
        /// </summary>
        /// <returns>A list of projects.</returns>
        [HttpGet]
        public IActionResult GetProjects()
        {
            // Implement logic to get projects
            return Ok(new { Message = "Projects retrieved successfully" });
        }

        /// <summary>
        /// Creates a new project.
        /// </summary>
        /// <param name="model">The project model.</param>
        /// <returns>A success message.</returns>
        [HttpPost]
        public IActionResult CreateProject([FromBody] ProjectModel model)
        {
            // Implement logic to create a project
            return Ok(new { Message = "Project created successfully" });
        }

        /// <summary>
        /// Updates an existing project.
        /// </summary>
        /// <param name="id">The ID of the project.</param>
        /// <param name="model">The project model.</param>
        /// <returns>A success message.</returns>
        [HttpPut("{id}")]
        public IActionResult UpdateProject(int id, [FromBody] ProjectModel model)
        {
            // Implement logic to update a project
            return Ok(new { Message = "Project updated successfully" });
        }

        /// <summary>
        /// Deletes a project.
        /// </summary>
        /// <param name="id">The ID of the project.</param>
        /// <returns>A success message.</returns>
        [HttpDelete("{id}")]
        public IActionResult DeleteProject(int id)
        {
            // Implement logic to delete a project
            return Ok(new { Message = "Project deleted successfully" });
        }
    }

    public class ProjectModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
