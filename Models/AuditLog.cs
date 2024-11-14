using System.ComponentModel.DataAnnotations;

namespace IssueTracking.Models
{
    public class AuditLog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Action { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public DateTime Timestamp { get; set; }

        public string Details { get; set; }
    }
}
