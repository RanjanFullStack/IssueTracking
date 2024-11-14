using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace IssueTracking.Models
{
    public class Project
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public string Assignee { get; set; }

        public ICollection<Issue> Issues { get; set; }
    }
}
