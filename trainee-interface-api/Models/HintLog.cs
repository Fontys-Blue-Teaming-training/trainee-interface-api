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
        public int TeamId { get; set; }
        [Required]
        public int HintId { get; set; }
        [Required]
        public int ScenarioId { get; set; }
        [Required, DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime ReceivedTime { get; set; }

        public Hint Hint { get; set; }

        public HintLog(int teamId, int hintId, int scenarioId)
        {
            TeamId = teamId;
            HintId = hintId;
            ScenarioId = scenarioId;
        }
    }
}
