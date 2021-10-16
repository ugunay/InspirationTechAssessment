using InspirationTechAssessment.Interfaces;
using InspirationTechAssessment.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace InspirationTechAssessment.Services
{
    public class AccountService : IAccountService
    {
        private InspirationTechDbContext _dbContext;

        public AccountService(InspirationTechDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbContext.Database.EnsureCreated();
        }

        public async Task<decimal> GetBalanceAsync(long accountId)
        {
            var accounts = await _dbContext.Accounts.ToListAsync();
            var account = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.Id == accountId);
            if (account == null) throw new Exception("Invalid account id");
            return account.Balance;
        }
    }
}