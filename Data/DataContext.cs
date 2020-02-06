using Data.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace Data
{
    public class DataContext : DbContext
    {
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Manager> Managers { get; set; }
        public DbSet<Tag> Tags { get; set; }

        public DbSet<TeamMember> TeamMembers { get; set; }
        public DbSet<OC> OCs { get; set; }
        public DbSet<OCUser> OCUsers { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Manager>().HasKey(ba => new { ba.UserID, ba.ProjectID });
            builder.Entity<TeamMember>().HasKey(ba => new { ba.UserID, ba.ProjectID });
            builder.Entity<Tag>().HasKey(ba => new { ba.TaskID, ba.UserID });
            builder.Entity<OCUser>().HasKey(ba => new { ba.UserID, ba.OCID });

        }
    }
}
