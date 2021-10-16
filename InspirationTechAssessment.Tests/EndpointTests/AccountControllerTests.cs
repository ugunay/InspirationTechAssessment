using InspirationTechAssessment.Controllers;
using InspirationTechAssessment.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace InspirationTechAssessment.Tests.EndpointTests
{
    public class AccountControllerTests
    {
        private Mock<IAccountService> _service;

        [Fact]
        public void GetBalance_WithExistingAccountId_ReturnsOkResult()
        {
            _service = new Mock<IAccountService>();
            _service.Setup(s => s.GetBalanceAsync(It.IsAny<long>()))
                .ReturnsAsync(100);
            var controllerUnderTest = new AccountController(_service.Object);

            var OkResult = controllerUnderTest.GetBalanceAsync(1).Result.Result as OkObjectResult;
            Assert.IsType<OkObjectResult>(OkResult);
            Assert.Equal((decimal)100, OkResult.Value);
        }
    }
}