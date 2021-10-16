using InspirationTechAssessment.Persistence.Entities;
using System.Threading.Tasks;

namespace InspirationTechAssessment.Interfaces
{
    public interface IAccountService
    {
        Task<decimal> GetBalanceAsync(long accountId);
    }
}