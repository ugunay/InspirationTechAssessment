using InspirationTechAssessment.Interfaces;
using InspirationTechAssessment.Models;
using InspirationTechAssessment.Persistence;
using InspirationTechAssessment.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace InspirationTechAssessment.Services
{
    public class PaymentService : IPaymentService
    {
        private InspirationTechDbContext _dbContext;
        private PaymentStrategyFactory _factory;

        public PaymentService(InspirationTechDbContext dbContext, PaymentStrategyFactory factory)
        {
            _dbContext = dbContext;
            _dbContext.Database.EnsureCreated();
            _factory = factory;
        }

        public async Task<string> PayAsync(PaymentInfo paymentInfo)
        {
            if (paymentInfo.Amount < 0) throw new Exception("Payment amount cannot be negative!");
            var account = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.Id == paymentInfo.AccountId);
            var totalAmount = _factory[paymentInfo.PaymentOrigin].CalculateTotalAmount(paymentInfo.Amount);
            if (account.Balance < totalAmount) throw new Exception("Balance is not enough!");
            account.Balance -= _factory[paymentInfo.PaymentOrigin].CalculateTotalAmount(paymentInfo.Amount);
            var guid = Guid.NewGuid().ToString();
            var transaction = new Transaction()
            {
                Id = guid,
                Amount = paymentInfo.Amount,
                PaymentOrigin = paymentInfo.PaymentOrigin,
                CreatedAt = DateTime.UtcNow,
                Account = account
            };
            await _dbContext.Transactions.AddAsync(transaction);
            _dbContext.SaveChanges();
            return guid;
        }

        public async Task<decimal> AdjustAsync(AdjustmentInfo adjustmentInfo)
        {
            if (adjustmentInfo.Amount < 0) throw new Exception("Adjusment amount cannot be negative!");
            var transaction = await _dbContext.Transactions.Include(t => t.Account).FirstOrDefaultAsync(t => t.Id == adjustmentInfo.TransactionId);
            if (transaction == null) throw new Exception("No transaction exists with specified ID!");
            if (adjustmentInfo.Amount == transaction.Amount) throw new Exception("Adjustment amount should be different than the payment amount!");
            if (adjustmentInfo.Amount > transaction.Amount) return await IncreasePaymentAsync(transaction, adjustmentInfo.Amount);
            return await DecreasePaymentAsync(transaction, adjustmentInfo.Amount);
        }

        private async Task<decimal> IncreasePaymentAsync(Transaction transaction, decimal newAmount)
        {
            var balanceAmountToDecrease = _factory[transaction.PaymentOrigin].CalculateTotalAmount(newAmount - transaction.Amount);
            var account = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.Id == transaction.Account.Id);
            if (account.Balance < balanceAmountToDecrease) throw new Exception("Balance is not enough!");
            account.Balance -= balanceAmountToDecrease;
            transaction.Amount = newAmount;
            _dbContext.SaveChanges();
            return account.Balance;
        }

        private async Task<decimal> DecreasePaymentAsync(Transaction transaction, decimal newAmount)
        {
            var balanceAmountToIncrease = _factory[transaction.PaymentOrigin].CalculateTotalAmount(transaction.Amount - newAmount);
            var account = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.Id == transaction.Account.Id);
            account.Balance += balanceAmountToIncrease;
            transaction.Amount = newAmount;
            _dbContext.SaveChanges();
            return account.Balance;
        }
    }
}