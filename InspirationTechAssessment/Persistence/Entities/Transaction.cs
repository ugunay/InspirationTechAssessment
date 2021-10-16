using InspirationTechAssessment.Enums;
using System;

namespace InspirationTechAssessment.Persistence.Entities
{
    public class Transaction
    {
        public string Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public decimal Amount { get; set; }
        public PaymentOrigin PaymentOrigin { get; set; }
        public Account Account { get; set; }
    }
}