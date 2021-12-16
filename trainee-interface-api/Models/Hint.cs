using System.ComponentModel.DataAnnotations;

namespace trainee_interface_api.Models
{
    public class Hint
    {
        [Key, Required]
        public int HintId { get; set; }
        [Required]
        public int ScenarioId { get; set; }
        [Required]
        public int FlagId { get; set; }
        [Required]
        public string HintText { get; set; }
        public string ImageUrl { get; set; }
        [Required]
        public int TimePenalty { get; set; }
    }
}
