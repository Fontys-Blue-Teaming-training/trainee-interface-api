using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace trainee_interface_api.Models
{
    public class StartedScenario
    {
        [Key, Required]
        public int Id { get; set; }
        [Required]
        public Team Team { get; set; }
        [Required]
        public Scenario Scenario{ get; set; }
        [Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}
