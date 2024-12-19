using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IssueTracking.Models
{
    public class FantasySquadResponse
    {
        [Key, Column(Order = 0)]
        public DateTime Date { get; set; }

        [Key, Column(Order = 1)]
        public string ApiName { get; set; }

        public string Response { get; set; }
    }
}
