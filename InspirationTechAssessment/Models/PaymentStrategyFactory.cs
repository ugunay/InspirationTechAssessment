using InspirationTechAssessment.Enums;
using InspirationTechAssessment.Interfaces;
using System;
using System.Collections.Generic;

namespace InspirationTechAssessment.Models
{
    public class PaymentStrategyFactory
    {
        private readonly Dictionary<PaymentOrigin, Func<IPaymentStrategy>> paymentStrategies;

        public PaymentStrategyFactory()
        {
            paymentStrategies = new Dictionary<PaymentOrigin, Func<IPaymentStrategy>>();
        }

        public IPaymentStrategy this[PaymentOrigin paymentType] => CreatePaymentStrategy(paymentType);

        public IPaymentStrategy CreatePaymentStrategy(PaymentOrigin paymentType) => paymentStrategies[paymentType]();

        public void RegisterStrategy(PaymentOrigin paymentType, Func<IPaymentStrategy> factoryMethod)
        {
            if (factoryMethod is null) return;
            paymentStrategies[paymentType] = factoryMethod;
        }
    }
}