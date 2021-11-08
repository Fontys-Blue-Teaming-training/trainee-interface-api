using System.ComponentModel.DataAnnotations;

namespace trainee_interface_api.Models
{
    public class Flag
    {
        [Key, Required]
        public int Id { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public int Points { get; set; }
        [Required]
        public string FlagCode { get; set; }
        [Required]
        public int ScenarioId { get; set; }
        public Scenario Scenario { get; set; }
    }
}