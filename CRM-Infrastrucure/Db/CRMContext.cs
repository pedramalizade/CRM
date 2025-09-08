
using CRM.Domain.Entities;

namespace CRM.Infrastructure.Db
{
    public class CRMContext : DbContext
    {
        public CRMContext(DbContextOptions<CRMContext> options)
            : base(options)
        {
        }

        public DbSet<User> User { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<Business> Business { get; set; }
        public DbSet<Company> Company { get; set; }
        public DbSet<Note> Note { get; set; }
        public DbSet<Contact> Contact { get; set; }

    }
}