using InspirationTechAssessment.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InspirationTechAssessment.Persistence.EntityConfigurations
{
    public class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.HasKey(a => a.Id);
            builder.HasMany(a => a.Transactions)
                .WithOne(t => t.Account);
            builder.HasData(
                new Account
                {
                    Id = 4755,
                    Balance = (decimal)1001.88
                },
                new Account
                {
                    Id = 9834,
                    Balance = (decimal)456.45
                },
                new Account
                {
                    Id = 7735,
                    Balance = (decimal)89.36
                }
            );
        }
    }
}