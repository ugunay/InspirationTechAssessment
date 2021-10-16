using InspirationTechAssessment.Enums;
using InspirationTechAssessment.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace InspirationTechAssessment.Persistence.EntityConfigurations
{
    public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.HasKey(t => t.Id);
            builder.Property(t => t.CreatedAt).IsRequired();
            builder.Property(t => t.PaymentOrigin).IsRequired();
            builder.Property(t => t.PaymentOrigin).HasConversion(po => po.ToString(), po => Enum.Parse<PaymentOrigin>(po));
        }
    }
}