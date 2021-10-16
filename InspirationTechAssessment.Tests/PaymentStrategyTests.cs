using InspirationTechAssessment.Models;
using System;
using Xunit;

namespace InspirationTechAssessment.Tests
{
    public class PaymentStrategyTests
    {
        [Theory]
        [InlineData(100, 102)]
        [InlineData(0, 0)]
        public void CalculateTotalAmount_WithMaster_ReturnsTwoPercentCommission(decimal amount, decimal totalAmount)
        {
            var payment = new MasterPayment();
            var result = payment.CalculateTotalAmount(amount);
            Assert.Equal(totalAmount, result);
        }

        [Fact]
        public void CalculateTotalAmount_WithMasterAndNegativeAmount_ThrowsException()
        {
            var payment = new MasterPayment();
            Assert.Throws<Exception>(() => payment.CalculateTotalAmount(-100));
        }

        [Theory]
        [InlineData(100, 101)]
        [InlineData(0, 0)]
        public void CalculateTotalAmount_WithVisa_ReturnsOnePercentCommission(decimal amount, decimal totalAmount)
        {
            var payment = new VisaPayment();
            var result = payment.CalculateTotalAmount(amount);
            Assert.Equal(totalAmount, result);
        }

        [Fact]
        public void CalculateTotalAmount_WithVisaAndNegativeAmount_ThrowsException()
        {
            var payment = new VisaPayment();
            Assert.Throws<Exception>(() => payment.CalculateTotalAmount(-100));
        }
    }
}