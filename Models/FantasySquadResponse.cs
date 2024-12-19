using System;
using System.ComponentModel.DataAnnotations;

namespace IssueTracking.Models
{
    public class FantasySquadResponse
    {
        [Key]
        public DateTime Date { get; set; }
        public string Response { get; set; }
    }
}
