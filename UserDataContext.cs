using System.Data.Entity;

namespace Arushanyan_LB4.Data
{
    public class UserDataContext : DbContext
    {
        public UserDataContext() : base("name=UserDataContext") { }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasRequired(u => u.Role).WithMany(r => r.Users).HasForeignKey(u => u.Role_ID).WillCascadeOnDelete(false);
            base.OnModelCreating(modelBuilder);
        }
    }
}
