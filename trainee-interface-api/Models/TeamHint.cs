using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace trainee_interface_api.Models
{
    public class TeamHint
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int HintId { get; set; }
        [Required]
        public int TeamId { get; set; }
    }
}
