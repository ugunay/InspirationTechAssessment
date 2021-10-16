using InspirationTechAssessment.Enums;
using InspirationTechAssessment.Models;
using InspirationTechAssessment.Persistence;
using InspirationTechAssessment.Persistence.Entities;
using InspirationTechAssessment.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;

namespace InspirationTechAssessment.Tests.ServiceTests
{
    public class PaymentServiceTests
    {
        private static DbContextOptions<InspirationTechDbContext> CreateNewContextOptions() => new DbContextOptionsBuilder<InspirationTechDbContext>()
            .UseInMemoryDatabase("paymentTestDB")
            .Options;

        private PaymentService _paymentService;
        private PaymentStrategyFactory _factory;

        public PaymentServiceTests()
        {
            _factory = new PaymentStrategyFactory();
            _factory.RegisterStrategy(PaymentOrigin.MASTER, () => new MasterPayment());
            _factory.RegisterStrategy(PaymentOrigin.VISA, () => new VisaPayment());
        }

        [Theory]
        [InlineData(200, 100, 98)]
        public async Task PaymentService_WithEnoughBalanceAndNonNegativeAmount_PaysCorrectly(decimal balance, decimal amount, decimal balanceAfterPay)
        {
            using var context = new InspirationTechDbContext(CreateNewContextOptions());
            context.Database.EnsureCreated();
            await context.Accounts.AddAsync(new Account() { Id = 1, Balance = balance });
            await context.SaveChangesAsync();
            _paymentService = new PaymentService(context, _factory);

            await _paymentService.PayAsync(new PaymentInfo { AccountId = 1, Amount = amount, PaymentOrigin = PaymentOrigin.MASTER });
            var account = await context.Accounts.FirstOrDefaultAsync(a => a.Id == 1);

            Assert.Equal(balanceAfterPay, account.Balance);
            context.Database.EnsureDeleted();
        }

        [Theory]
        [InlineData(200, 0, 200)]
        public async Task PaymentService_WithEnoughBalanceAndZeroAmount_PaysCorrectly(decimal balance, decimal amount, decimal balanceAfterPay)
        {
            using var context = new InspirationTechDbContext(CreateNewContextOptions());
            context.Database.EnsureCreated();
            await context.Accounts.AddAsync(new Account() { Id = 1, Balance = balance });
            await context.SaveChangesAsync();
            _paymentService = new PaymentService(context, _factory);

            await _paymentService.PayAsync(new PaymentInfo { AccountId = 1, Amount = amount, PaymentOrigin = PaymentOrigin.MASTER });
            var account = await context.Accounts.FirstOrDefaultAsync(a => a.Id == 1);

            Assert.Equal(balanceAfterPay, account.Balance);
            context.Database.EnsureDeleted();
        }

        [Fact]
        public async Task PaymentService_PayWithNegativeAmount_ThrowsException()
        {
            using var context = new InspirationTechDbContext(CreateNewContextOptions());
            context.Database.EnsureCreated();
            await context.Accounts.AddAsync(new Account() { Id = 1, Balance = 200 });
            await context.SaveChangesAsync();
            _paymentService = new PaymentService(context, _factory);

            await Assert.ThrowsAsync<Exception>(async () => await _paymentService.PayAsync(new PaymentInfo { AccountId = 1, Amount = -100, PaymentOrigin = PaymentOrigin.VISA }));
            context.Database.EnsureDeleted();
        }

        [Fact]
        public async Task PaymentService_WithInsufficentBalance_ThrowsException()
        {
            using var context = new InspirationTechDbContext(CreateNewContextOptions());
            context.Database.EnsureCreated();
            await context.Accounts.AddAsync(new Account() { Id = 1, Balance = 100 });
            await context.SaveChangesAsync();
            _paymentService = new PaymentService(context, _factory);

            await Assert.ThrowsAsync<Exception>(async () => await _paymentService.PayAsync(new PaymentInfo { AccountId = 1, Amount = 100, PaymentOrigin = PaymentOrigin.VISA }));
            context.Database.EnsureDeleted();
        }

        [Theory]
        [InlineData(200, 0)]
        public async Task PaymentService_WithProperAmount_CreatesTransactionCorrectly(decimal balance, decimal amount)
        {
            using var context = new InspirationTechDbContext(CreateNewContextOptions());
            context.Database.EnsureCreated();
            await context.Accounts.AddAsync(new Account() { Id = 1, Balance = balance });
            await context.SaveChangesAsync();
            _paymentService = new PaymentService(context, _factory);

            var transactionId = await _paymentService.PayAsync(new PaymentInfo { AccountId = 1, Amount = amount, PaymentOrigin = PaymentOrigin.MASTER });
            var transaction = await context.Transactions.FirstOrDefaultAsync(t => t.Id == transactionId);

            Assert.Equal(1, transaction.Account.Id);
            context.Database.EnsureDeleted();
        }

        [Fact]
        public async Task PaymentService_AdjustWithNegativeAmount_ThrowsException()
        {
            using var context = new InspirationTechDbContext(CreateNewContextOptions());
            context.Database.EnsureCreated();
            await context.Accounts.AddAsync(new Account() { Id = 1, Balance = 200 });
            await context.SaveChangesAsync();
            _paymentService = new PaymentService(context, _factory);

            var transactionId = await _paymentService.PayAsync(new PaymentInfo { AccountId = 1, Amount = 10, PaymentOrigin = PaymentOrigin.MASTER });
            await Assert.ThrowsAsync<Exception>(async () => await _paymentService.AdjustAsync(new AdjustmentInfo { TransactionId = transactionId, Amount = -100 }));
            context.Database.EnsureDeleted();
        }

        [Fact]
        public async Task PaymentService_AdjustWithInvalidTransactionId_ThrowsException()
        {
            using var context = new InspirationTechDbContext(CreateNewContextOptions());
            context.Database.EnsureCreated();
            await context.Accounts.AddAsync(new Account() { Id = 1, Balance = 200 });
            await context.SaveChangesAsync();
            _paymentService = new PaymentService(context, _factory);

            var transactionId = "test";
            await Assert.ThrowsAsync<Exception>(async () => await _paymentService.AdjustAsync(new AdjustmentInfo { TransactionId = transactionId, Amount = -100 }));
            context.Database.EnsureDeleted();
        }

        [Fact]
        public async Task PaymentService_AdjustWithSameAmount_ThrowsException()
        {
            using var context = new InspirationTechDbContext(CreateNewContextOptions());
            context.Database.EnsureCreated();
            await context.Accounts.AddAsync(new Account() { Id = 1, Balance = 200 });
            await context.SaveChangesAsync();
            _paymentService = new PaymentService(context, _factory);

            var transactionId = await _paymentService.PayAsync(new PaymentInfo { AccountId = 1, Amount = 10, PaymentOrigin = PaymentOrigin.MASTER });
            await Assert.ThrowsAsync<Exception>(async () => await _paymentService.AdjustAsync(new AdjustmentInfo { TransactionId = transactionId, Amount = 10 }));
            context.Database.EnsureDeleted();
        }

        [Fact]
        public async Task PaymentService_DecreaseTransactionAmount_AdjustsCorrectly()
        {
            using var context = new InspirationTechDbContext(CreateNewContextOptions());
            context.Database.EnsureCreated();
            await context.Accounts.AddAsync(new Account() { Id = 1, Balance = 200 });
            await context.SaveChangesAsync();
            _paymentService = new PaymentService(context, _factory);

            var transactionId = await _paymentService.PayAsync(new PaymentInfo { AccountId = 1, Amount = 100, PaymentOrigin = PaymentOrigin.MASTER });
            await _paymentService.AdjustAsync(new AdjustmentInfo { TransactionId = transactionId, Amount = 50 });
            var account = await context.Accounts.FirstOrDefaultAsync(a => a.Id == 1);

            Assert.Equal(149, account.Balance);
            context.Database.EnsureDeleted();
        }

        [Fact]
        public async Task PaymentService_IncreaseTransactionAmountWitInsufficentBalance_ThrowsException()
        {
            using var context = new InspirationTechDbContext(CreateNewContextOptions());
            context.Database.EnsureCreated();
            await context.Accounts.AddAsync(new Account() { Id = 1, Balance = 200 });
            await context.SaveChangesAsync();
            _paymentService = new PaymentService(context, _factory);

            var transactionId = await _paymentService.PayAsync(new PaymentInfo { AccountId = 1, Amount = 100, PaymentOrigin = PaymentOrigin.MASTER });
            await Assert.ThrowsAsync<Exception>(async () => await _paymentService.AdjustAsync(new AdjustmentInfo { TransactionId = transactionId, Amount = 199 }));
            context.Database.EnsureDeleted();
        }

        [Fact]
        public async Task PaymentService_IncreaseTransactionAmountWithSufficentBalance_AdjustsCorrectly()
        {
            using var context = new InspirationTechDbContext(CreateNewContextOptions());
            context.Database.EnsureCreated();
            await context.Accounts.AddAsync(new Account() { Id = 1, Balance = 200 });
            await context.SaveChangesAsync();
            _paymentService = new PaymentService(context, _factory);

            var transactionId = await _paymentService.PayAsync(new PaymentInfo { AccountId = 1, Amount = 100, PaymentOrigin = PaymentOrigin.MASTER });
            await _paymentService.AdjustAsync(new AdjustmentInfo { TransactionId = transactionId, Amount = 150 });
            var account = await context.Accounts.FirstOrDefaultAsync(a => a.Id == 1);

            Assert.Equal(47, account.Balance);
            context.Database.EnsureDeleted();
        }
    }
}