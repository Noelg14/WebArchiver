using Microsoft.EntityFrameworkCore;
using System.Reflection;
using WebArchiver.Entities;

namespace WebArchiver.Data
{
    public class PagesContext : DbContext
    {
        public PagesContext(DbContextOptions<PagesContext> options) : base(options)
        {

        }
        public DbSet<Pages> Pages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}

