using DataInputt.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace DataInputt
{
    public class DataContext : DbContext
    {
        public DbSet<Publication> Publications { get; set; }
        public DbSet<Medium> Media { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Publisher> Publishers { get; set; }

        public DataContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connection = new SqliteConnection("DataSource=:memory:");
                connection.Open();
                optionsBuilder.UseSqlite(connection);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Project>().Ignore(p => p.Tasks);
            modelBuilder.Entity<Project>().Ignore(p => p.Tools);
        }
    }
}
