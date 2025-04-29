using Microsoft.EntityFrameworkCore;
using VoteBirthy.Models;
using System;

namespace VoteBirthy.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Gift> Gifts { get; set; }
        public DbSet<Vote> Votes { get; set; }
        public DbSet<VoteOption> VoteOptions { get; set; }
        public DbSet<VoteCast> VoteCasts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure entity properties
            modelBuilder.Entity<Employee>()
                .Property(e => e.EmployeeId)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Gift>()
                .Property(g => g.GiftId)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Vote>()
                .Property(v => v.VoteId)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<VoteOption>()
                .Property(vo => vo.VoteOptionId)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<VoteCast>()
                .Property(vc => vc.VoteCastId)
                .ValueGeneratedOnAdd();

            // Configure relationships to avoid multiple cascade paths
            modelBuilder.Entity<Vote>()
                .HasOne(v => v.StartedBy)
                .WithMany(e => e.StartedVotes)
                .HasForeignKey(v => v.StartedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<VoteCast>()
                .HasOne(vc => vc.Voter)
                .WithMany(e => e.Casts)
                .HasForeignKey(vc => vc.VoterId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure unique constraint for Votes (one per birthday employee per year)
            modelBuilder.Entity<Vote>()
                .HasIndex(v => new { v.BirthdayEmpId, v.StartDate })
                .IsUnique()
                .HasDatabaseName("UQ_OneOpenVotePerEmpYr");

            // Configure unique constraint for VoteCasts (one per employee per vote option)
            modelBuilder.Entity<VoteCast>()
                .HasIndex(vc => new { vc.VoteOptionId, vc.VoterId })
                .IsUnique()
                .HasDatabaseName("UQ_OneVotePerUserPerVote");

            // Seed data would be added here for testing
        }
    }
} 