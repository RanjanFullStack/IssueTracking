using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace IssueTracking.Models
{
    public class Issue
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        public IssueStatus Status { get; set; }

        public int ProjectId { get; set; }

        public Project Project { get; set; }

        public ICollection<Tag> Tags { get; set; }
    }

    public enum IssueStatus
    {
        Open,
        InProgress,
        Resolved
    }
}
