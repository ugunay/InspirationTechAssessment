using InspirationTechAssessment.Interfaces;

namespace InspirationTechAssessment.Models
{
    public class NoPayment : IPaymentStrategy
    {
        public decimal CalculateTotalAmount(decimal amount) => 0;
    }
}