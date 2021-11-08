using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace trainee_interface_api.Models
{
    public class FlagCompleted
    {
        [Required]
        public Flag CompletedFlag { get; set; }
        [Required]
        public Team Team { get; set; }
        [Required, DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime Completed { get; set; }
    }
}
