using InspirationTechAssessment.Models;
using System.Threading.Tasks;

namespace InspirationTechAssessment.Interfaces
{
    public interface IPaymentService
    {
        Task<string> PayAsync(PaymentInfo paymentInfo);

        Task<decimal> AdjustAsync(AdjustmentInfo adjustmentInfo);
    }
}