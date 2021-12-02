using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace trainee_interface_api.Models
{
    public class HintLog
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string TeamName { get; set; }
        [Required]
        public int HintId { get; set; }
        [Required]
        public int ScenarioId { get; set; }
        [Required, DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime RecievedTime { get; set; }

        public HintLog(string teamName, int hintId, int scenarioId)
        {
            TeamName = teamName;
            HintId = hintId;
            ScenarioId = scenarioId;
        }
    }
}
