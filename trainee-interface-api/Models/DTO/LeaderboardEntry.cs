using System;

namespace trainee_interface_api.Models.DTO
{
    public class LeaderboardEntry
    {
        public int FlagId { get; set; }
        public int TeamId { get; set; }
        public string TeamName { get; set; }
        public int Points { get; set; }
        public double TotalSeconds { get; set; }
    }
}
