using InspirationTechAssessment.Persistence.Entities;
using InspirationTechAssessment.Persistence.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace InspirationTechAssessment.Persistence
{
    public class InspirationTechDbContext : DbContext
    {
        public InspirationTechDbContext()
        {
        }

        public InspirationTechDbContext(DbContextOptions<InspirationTechDbContext> options) : base(options)
        {
        }

        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new AccountConfiguration());
            builder.ApplyConfiguration(new TransactionConfiguration());
        }
    }
}