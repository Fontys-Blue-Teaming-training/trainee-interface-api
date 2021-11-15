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

        // Change this
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = "server=localhost;user=root;password=redteamingdatabase;database=redteamingdb";
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
                    new Scenario() { Id = 2, Name = "Forensics" }
                );

            modelBuilder.Entity<Flag>()
                .HasData(
                    new Flag() { Description = "Firewall NAT rule flag", Points = 1000, FlagCode = "BT_SSH{F1R3W4LL_N4T}", ScenarioId = 1 },
                    new Flag() { Description = "Flag inside all network traffic", Points = 2500, FlagCode = "BT_SSH{N3TW0RK_TR4FFIC}", ScenarioId = 1 },
                    new Flag() { Description = "Username of bruteforce attempt", Points = 2000, FlagCode = "BT_SSH{SSH_US3RN4ME}", ScenarioId = 1 },

                    new Flag() { Description = "Copyright of BBQ image", Points = 1000, FlagCode = "BT_FORENSICS{Nucl3ar_C0d3}", ScenarioId = 2 },
                    new Flag() { Description = "Flag inside the journal", Points = 1000, FlagCode = "BT_FORENSICS{J0urNal_Fr3nch}", ScenarioId = 2 }
                );
        }
    }
}
