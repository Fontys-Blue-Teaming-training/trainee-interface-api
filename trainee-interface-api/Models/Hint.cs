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
        public string HintText { get; set; }
        [Required]
        public string ImageUrl { get; set; }
    }
}
