using InspirationTechAssessment.Enums;

namespace InspirationTechAssessment.Models
{
    public class PaymentInfo
    {
        public long AccountId { get; set; }

        public PaymentOrigin PaymentOrigin { get; set; }

        public decimal Amount { get; set; }
    }
}