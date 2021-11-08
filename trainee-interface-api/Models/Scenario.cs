using System.ComponentModel.DataAnnotations;

namespace trainee_interface_api.Models
{
    public class Scenario
    {
        [Key, Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
