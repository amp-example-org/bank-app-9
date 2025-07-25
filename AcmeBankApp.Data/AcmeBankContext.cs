using System.Data.Entity;
using AcmeBankApp.Core.Models;

namespace AcmeBankApp.Data
{
    public class AcmeBankContext : DbContext
    {
        public AcmeBankContext() : base("DefaultConnection")
        {
            // Legacy pattern - disable initializer to prevent auto-migrations
            Database.SetInitializer<AcmeBankContext>(null);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Legacy EF6 fluent configuration
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Account>().ToTable("Accounts");
            modelBuilder.Entity<Transaction>().ToTable("Transactions");

            // Configure relationships
            modelBuilder.Entity<Account>()
                .HasRequired(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId);

            modelBuilder.Entity<Transaction>()
                .HasRequired(t => t.FromAccount)
                .WithMany()
                .HasForeignKey(t => t.FromAccountId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Transaction>()
                .HasOptional(t => t.ToAccount)
                .WithMany()
                .HasForeignKey(t => t.ToAccountId)
                .WillCascadeOnDelete(false);

            base.OnModelCreating(modelBuilder);
        }
    }
}
