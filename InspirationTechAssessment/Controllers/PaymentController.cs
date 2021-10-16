using InspirationTechAssessment.Enums;
using InspirationTechAssessment.Interfaces;
using InspirationTechAssessment.Models;
using InspirationTechAssessment.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace InspirationTechAssessment.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost]
        public async Task<ActionResult> Post(InputMessageDTO inputMessage)//
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (inputMessage.MessageType == nameof(PaymentMessage.PAYMENT))
            {
                return Ok(await _paymentService.PayAsync(new PaymentInfo { AccountId = inputMessage.AccountId, Amount = inputMessage.Amount, PaymentOrigin = Enum.Parse<PaymentOrigin>(inputMessage.Origin) }));
            }
            return Ok(await _paymentService.AdjustAsync(new AdjustmentInfo { TransactionId = inputMessage.TransactionId, Amount = inputMessage.Amount }));
        }
    }
}