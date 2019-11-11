using DataInputt.Models;
using System.Data.Entity;

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
    }
}
