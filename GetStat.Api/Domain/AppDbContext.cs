using System;
using GetStat.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GetStat.Api.Domain
{
    public class AppDbContext : IdentityDbContext<Account>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Tutor> Tutors { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Moderator> Moderators { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Group> Groups { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Id = "2BF94763-FE0B-48CD-B413-C8BC705380DC",
                    Name = "User",
                    NormalizedName = "USER"
                },
                new IdentityRole
                {
                    Id = "CD03DD4F-E829-4784-869D-AE4612A7EA0F",
                    Name = "Student",
                    NormalizedName = "STUDENT"
                },
                new IdentityRole
                {
                    Id = "BA6736CC-6D97-4314-92D3-803655469BC1",
                    Name = "Teacher",
                    NormalizedName = "TEACHER"
                },
                new IdentityRole
                {
                    Id = "3C3C481E-C832-42A9-B18D-2A157CC09457",
                    Name = "Tutor",
                    NormalizedName = "TUTOR"
                },
                new IdentityRole
                {
                    Id = "D3FCF42A-22C1-455E-825B-2BF65AA877FE",
                    Name = "Moderator",
                    NormalizedName = "MODERATOR"
                },
                new IdentityRole
                {
                    Id = "B0BAB2ED-F61E-4E83-81D7-7B96267D473B",
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                },
                new IdentityRole
                {
                    Id = "D9CBECB0-5A1E-41A7-A122-D3E1B0B73D94",
                    Name = "MainAdmin",
                    NormalizedName = "MAINADMIN"
                }
            );

        }
    }
}