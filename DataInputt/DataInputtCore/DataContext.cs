using DataInputt.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace DataInputt
{
    public class DataContext : DbContext
    {
        public DbSet<Publication> Publications { get; set; }
        public DbSet<Medium> Media { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Publisher> Publishers { get; set; }

        private static bool _created = false;
        public DataContext()
        {
            if (!_created)
            {
                _created = true;
                Database.EnsureDeleted();
                Database.EnsureCreated();
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(@"Data Source=(localdb)\MSSQLLocalDB; Filename=TestDbEFCore.db");
        }
    }
}
