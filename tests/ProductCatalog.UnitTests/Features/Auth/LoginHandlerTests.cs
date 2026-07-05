using Microsoft.Extensions.Logging;
using Moq;
using ProductCatalog.Application.Features.Auth.Handlers;
using ProductCatalog.Application.Features.Auth.Requests;
using ProductCatalog.Application.Interfaces;
using ProductCatalog.Application.Models;
using ProductCatalog.Domain.Enums;

namespace ProductCatalog.UnitTests.Features.Auth
{
    public class LoginHandlerTests
    {
        private readonly Mock<IAuthService> _authServiceMock = new();
        private readonly Mock<IJwtService> _jwtServiceMock = new();
        private readonly Mock<ILogger<LoginHandler>> _loggerMock = new();
        private readonly LoginHandler _handler;

        public LoginHandlerTests()
        {
            _handler = new LoginHandler(_authServiceMock.Object, _jwtServiceMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task HandleRequest_ReturnsFailure_WhenCredentialsAreInvalid()
        {
            _authServiceMock
                .Setup(a => a.GetTokenAsync("wronguser", "wrongpass"))
                .ReturnsAsync((string?)null);

            var request = new LoginRequest { Username = "wronguser", Password = "wrongpass" };

            var result = await _handler.HandleAsync(request);

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.InvalidInput, result.ErrorCode);
        }

        [Fact]
        public async Task HandleRequest_ReturnsToken_WhenCredentialsAreValid()
        {
            _authServiceMock.Setup(a => a.GetTokenAsync("emilys", "emilyspass")).ReturnsAsync("dummy-token");
            _authServiceMock.Setup(a => a.GetCurrentUserAsync("dummy-token"))
                .ReturnsAsync(new User { Id = 1, Username = "emilys", Role = "admin" });
            _jwtServiceMock.Setup(j => j.GenerateToken(1, "emilys", "admin")).Returns("jwt-token");

            var request = new LoginRequest { Username = "emilys", Password = "emilyspass" };

            var result = await _handler.HandleAsync(request);

            Assert.True(result.IsSuccess);
            Assert.Equal("jwt-token", result.Value!.Token);
        }

        [Fact]
        public async Task HandleRequest_ReturnsExternalServiceFailure_WhenGetTokenThrows()
        {
            _authServiceMock
                .Setup(a => a.GetTokenAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new HttpRequestException("boom"));

            var request = new LoginRequest { Username = "emilys", Password = "emilyspass" };

            var result = await _handler.HandleAsync(request);

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.ExternalService, result.ErrorCode);
        }

        [Fact]
        public async Task HandleRequest_ReturnsExternalServiceFailure_WhenGetCurrentUserThrows()
        {
            _authServiceMock.Setup(a => a.GetTokenAsync("emilys", "emilyspass")).ReturnsAsync("dummy-token");
            _authServiceMock
                .Setup(a => a.GetCurrentUserAsync("dummy-token"))
                .ThrowsAsync(new HttpRequestException("boom"));

            var request = new LoginRequest { Username = "emilys", Password = "emilyspass" };

            var result = await _handler.HandleAsync(request);

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.ExternalService, result.ErrorCode);
        }

        [Fact]
        public async Task HandleRequest_ReturnsNotFound_WhenCurrentUserIsNull()
        {
            _authServiceMock.Setup(a => a.GetTokenAsync("emilys", "emilyspass")).ReturnsAsync("dummy-token");
            _authServiceMock.Setup(a => a.GetCurrentUserAsync("dummy-token")).ReturnsAsync((User?)null);

            var request = new LoginRequest { Username = "emilys", Password = "emilyspass" };

            var result = await _handler.HandleAsync(request);

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.NotFound, result.ErrorCode);
        }
    }
}
