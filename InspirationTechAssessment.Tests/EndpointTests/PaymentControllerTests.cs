using InspirationTechAssessment.Controllers;
using InspirationTechAssessment.Interfaces;
using InspirationTechAssessment.Models;
using InspirationTechAssessment.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using Xunit;

namespace InspirationTechAssessment.Tests.EndpointTests
{
    public class PaymentControllerTests
    {
        private Mock<IPaymentService> _service;

        [Fact]
        public void Pay_WithInvalidMessageType_ThrowsError()
        {
            _service = new Mock<IPaymentService>();
            var controllerUnderTest = new PaymentController(_service.Object);

            Assert.ThrowsAsync<Exception>(() => controllerUnderTest.Post(new InputMessageDTO() { MessageType = "play", Amount = 10, Origin = "master" }));
        }

        [Fact]
        public void Pay_WithValidMessageTypeButInvalidOrigin_ThrowsError()
        {
            _service = new Mock<IPaymentService>();
            var controllerUnderTest = new PaymentController(_service.Object);

            Assert.ThrowsAsync<Exception>(() => controllerUnderTest.Post(new InputMessageDTO() { MessageType = "payment", Amount = 10, Origin = "notexists" }));
        }

        [Fact]
        public void Pay_WithCaseInsensitiveMessageTypeOrOrigin_PaysCorrectly()
        {
            _service = new Mock<IPaymentService>();
            _service.Setup(s => s.PayAsync(It.IsAny<PaymentInfo>()))
               .ReturnsAsync("guid");
            var controllerUnderTest = new PaymentController(_service.Object);

            var OkResult = controllerUnderTest.Post(new InputMessageDTO() { MessageType = "payment", Amount = 100, Origin = "visa" }).Result as OkObjectResult;
            Assert.IsType<OkObjectResult>(OkResult);
        }

        [Fact]
        public void Adjust_WithCaseInsensitiveMessageTypeOrOrigin_PaysCorrectly()
        {
            _service = new Mock<IPaymentService>();
            _service.Setup(s => s.AdjustAsync(It.IsAny<AdjustmentInfo>()))
               .ReturnsAsync(100);
            var controllerUnderTest = new PaymentController(_service.Object);

            var OkResult = controllerUnderTest.Post(new InputMessageDTO() { MessageType = "adjustment", TransactionId = "test" }).Result as OkObjectResult;
            Assert.IsType<OkObjectResult>(OkResult);
        }
    }
}