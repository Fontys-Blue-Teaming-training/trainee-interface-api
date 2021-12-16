using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using trainee_interface_api.Models;

namespace trainee_interface_api.Contexts
{
    public class DatabaseContext: DbContext
    {
        public DbSet<Flag> Flags { get; set; }
        public DbSet<FlagCompleted> FlagsCompleted { get; set; }
        public DbSet<Scenario> Scenarios { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<StartedScenario> StartedScenarios { get; set; }
        public DbSet<Hint> Hints { get; set; }
        public DbSet<HintLog> HintLogs { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = "server=localhost;user=root;password=root;database=redteamingdb";
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.EnableDetailedErrors();

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Team>()
                .HasIndex(p => new { p.Name })
                .IsUnique(true);

            modelBuilder.Entity<Scenario>()
                .HasData(
                    new Scenario() { Id = 1, Name = "SSH Bruteforce" },
                    new Scenario() { Id = 2, Name = "Forensics" },
                    new Scenario() { Id = 3, Name = "Malware" }
                );

            modelBuilder.Entity<Flag>()
                .HasData(
                    new Flag() { Id = 1, Description = "Firewall NAT rule flag", Points = 1000, FlagCode = "BT_SSH{F1R3W4LL_N4T}", ScenarioId = 1 },
                    new Flag() { Id = 2, Description = "Flag inside all network traffic", Points = 2500, FlagCode = "BT_SSH{N3TW0RK_TR4FFIC}", ScenarioId = 1 },
                    new Flag() { Id = 3, Description = "Username of bruteforce attempt", Points = 2000, FlagCode = "BT_SSH{SSH_US3RN4ME}", ScenarioId = 1 },
                    new Flag() { Id = 4, Description = "Flag inside the SOC alert", Points = 1000, FlagCode = "BT_SSH{S0C_4L3RT}", ScenarioId = 1 },

                    new Flag() { Id = 5, Description = "Copyright of BBQ image", Points = 1000, FlagCode = "BT_FORENSICS{Nucl3ar_C0d3}", ScenarioId = 2 },
                    new Flag() { Id = 6, Description = "Flag inside the journal", Points = 1000, FlagCode = "BT_FORENSICS{J0urNal_Fr3nch}", ScenarioId = 2 },
                    new Flag() { Id = 7, Description = "Flag inside mail to Gerda", Points = 1000, FlagCode = "BT_FORENSICS{3Mai1_G3rdA}", ScenarioId = 2 },

                    new Flag() { Id = 8, Description = "Flag inside the Suricata Alert", Points = 1000, FlagCode = "BT_MALWARE{Tw1TcH}", ScenarioId = 3 },
                    new Flag() { Id = 9, Description = "Flag inside the Hive escalation", Points = 1000, FlagCode = "BT_MALWARE{H1VE}", ScenarioId = 3 },
                    new Flag() { Id = 10, Description = "Flag inside the Folder of the Malware", Points = 1000, FlagCode = "BT_MALWARE{L0CaT10n}", ScenarioId = 3 }
                );

            modelBuilder.Entity<Hint>()
                .HasData(
                new Hint() { HintId = 1, ScenarioId = 1, HintText = "Scout around in the Firewall Rule Section, try to look for the flag in one of the rules", FlagId = 1, TimePenalty = 100 },
                new Hint() { HintId = 2, ScenarioId = 1, HintText = "Try to find the search query used during the traffic spoofing", FlagId = 2, TimePenalty = 100 },
                new Hint() { HintId = 3, ScenarioId = 1, HintText = "Use the SOC to find alerts for more information", FlagId = 3, TimePenalty = 100 },
                new Hint() { HintId = 4, ScenarioId = 1, HintText = "Find out who has been bruteforcing the system, maybe his username hides a clue", FlagId = 4, TimePenalty = 100 },

                new Hint() { HintId = 5, ScenarioId = 2, HintText = "Maybe the image that the actor has found, contains secret data", FlagId = 5, TimePenalty = 100 },
                new Hint() { HintId = 6, ScenarioId = 2, HintText = "Let's find out if the actors journal hides something we haven't looked at yet!", FlagId = 6, TimePenalty = 100 },
                new Hint() { HintId = 7, ScenarioId = 2, HintText = "Hmm, let's look at the email account and find out if anything secret is hiding there", FlagId = 7, TimePenalty = 100 },

                new Hint() { HintId = 8, ScenarioId = 3, HintText = "Try checking Suricata for any weird alerts", FlagId = 8, TimePenalty = 100 },
                new Hint() { HintId = 9, ScenarioId = 3, HintText = "The Hive System should contain any clues for us to find", FlagId= 9, TimePenalty = 100 },
                new Hint() { HintId = 10, ScenarioId = 3, HintText = "We should have eyes where the malware is located, let's try to figure out where to find the flag in there", FlagId = 10, TimePenalty = 100 }
                );

        }
    }
}
