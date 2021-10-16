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
    public class AccountServiceTests
    {
        private static DbContextOptions<InspirationTechDbContext> CreateNewContextOptions() => new DbContextOptionsBuilder<InspirationTechDbContext>()
            .UseInMemoryDatabase("accountTestDB")
            .Options;

        private AccountService _accountService;

        [Fact]
        public async Task AccountService_WithInvalidId_ThrowsException()
        {
            using var context = new InspirationTechDbContext(CreateNewContextOptions());
            context.Database.EnsureCreated();
            await context.Accounts.AddAsync(new Account() { Id = 1, Balance = 200 });
            await context.SaveChangesAsync();
            _accountService = new AccountService(context);

            await Assert.ThrowsAsync<Exception>(async () => await _accountService.GetBalanceAsync(3));
            context.Database.EnsureDeleted();
        }

        [Fact]
        public async Task AccountService_WithValidId_ReturnsCorrectBalance()
        {
            using var context = new InspirationTechDbContext(CreateNewContextOptions());
            context.Database.EnsureCreated();
            await context.Accounts.AddAsync(new Account() { Id = 1, Balance = 200 });
            await context.SaveChangesAsync();
            _accountService = new AccountService(context);

            var balance = await _accountService.GetBalanceAsync(1);

            Assert.Equal(200, balance);
            context.Database.EnsureDeleted();
        }
    }
}