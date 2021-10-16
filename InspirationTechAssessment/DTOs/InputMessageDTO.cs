using InspirationTechAssessment.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InspirationTechAssessment.Models.DTOs
{
    public class InputMessageDTO : IValidatableObject
    {
        public string MessageType { get; set; }

        public string TransactionId { get; set; }
        public long AccountId { get; set; }

        public string Origin { get; set; }

        public decimal Amount { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!Enum.TryParse(MessageType, true, out PaymentMessage paymentMessage))
            {
                yield return new ValidationResult("Invalid message type", new[] { nameof(PaymentMessage) });
            }
            MessageType = paymentMessage.ToString();

            if (!Enum.TryParse(Origin, true, out PaymentOrigin paymentOrigin) && MessageType == nameof(PaymentMessage.PAYMENT))
            {
                yield return new ValidationResult("Invalid  origin", new[] { nameof(PaymentMessage) });
            }
            Origin = paymentOrigin.ToString();
        }
    }
}