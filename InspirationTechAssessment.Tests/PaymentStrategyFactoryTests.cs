using InspirationTechAssessment.Enums;
using InspirationTechAssessment.Models;
using System;
using Xunit;

namespace InspirationTechAssessment.Tests
{
    public class PaymentStrategyFactoryTests
    {
        [Theory]
        [InlineData(PaymentOrigin.MASTER, typeof(MasterPayment))]
        [InlineData(PaymentOrigin.VISA, typeof(VisaPayment))]
        public void CreatePaymentStrategy_ReturnsCorrectStrategy(PaymentOrigin paymentOrigin, Type paymentType)
        {
            var factory = new PaymentStrategyFactory();
            factory.RegisterStrategy(PaymentOrigin.MASTER, () => new MasterPayment());
            factory.RegisterStrategy(PaymentOrigin.VISA, () => new VisaPayment());

            var paymentStrategy = factory[paymentOrigin];
            Assert.True(paymentStrategy.GetType().Name.Equals(paymentType.Name));
        }
    }
}