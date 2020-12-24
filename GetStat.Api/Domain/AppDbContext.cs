using System;
using GetStat.Domain.Models;
using GetStat.Domain.Models.Test;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GetStat.Api.Domain
{
    public sealed class AppDbContext : IdentityDbContext<Account>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Setting> Settings{ get; set; }
        public DbSet<ResultTest> ResultTests{ get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.UseHiLo();
            //metanit
            //modelBuilder
            //    .Entity<Test>()
            //    .HasOne(u => u.Settings)
            //    .WithOne(p => p.Test)
            //    .HasForeignKey<Setting>(p => p.TestId);

            modelBuilder.Entity<Test>()
                .HasOne<Setting>(s => s.Settings)
                .WithOne(ad => ad.Test)
                .HasForeignKey<Setting>(ad => ad.TestId);


            modelBuilder.Entity<Test>()
                .HasMany<Question>(g => g.Questions)
                .WithOne(s => s.Test)
                .HasForeignKey(s => s.TestId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Question>()
                .HasMany<Answer>(g => g.Answers)
                .WithOne(s => s.Question)
                .HasForeignKey(s => s.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Setting>().Property(x => x.Code)
                .HasDefaultValueSql("SUBSTRING(CONVERT(varchar(40), NEWID()),0,9)");

            modelBuilder.Entity<Question>()
                .Property(x => x.CorrectAnswer)
                    .HasDefaultValue(-1);

        }
    }
}