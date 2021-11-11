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
                    new Flag() { Id = 1, Description = "Test flag", Points = 1000, FlagCode = "BLUETEAM{TH1S_1S_4_FL4G}", ScenarioId = 1 },
                    new Flag() { Id = 2, Description = "Second flag", Points = 1500, FlagCode = "BLUETEAM{S3C0ND_FL4G_LM40}", ScenarioId = 1 }
                );
        }
    }
}
