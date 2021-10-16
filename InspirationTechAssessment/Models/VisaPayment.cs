using InspirationTechAssessment.Interfaces;
using System;

namespace InspirationTechAssessment.Models
{
    public class VisaPayment : IPaymentStrategy
    {
        public decimal CalculateTotalAmount(decimal amount)
        {
            if (amount < 0) throw new Exception("Amount cannot be negative!");
            return amount * (decimal)1.01;
        }
    }
}