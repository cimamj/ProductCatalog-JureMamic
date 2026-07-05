using Microsoft.Extensions.Logging;
using ProductCatalog.Application.Common;
using ProductCatalog.Application.DTOs;
using ProductCatalog.Application.Features.Auth.Requests;
using ProductCatalog.Application.Interfaces;
using ProductCatalog.Application.Models;
using ProductCatalog.Domain.Common;
using ProductCatalog.Domain.Enums;

namespace ProductCatalog.Application.Features.Auth.Handlers
{
    public class LoginHandler : RequestHandler<LoginRequest, LoginResponseDto>
    {
        private readonly IAuthService _authService;
        private readonly IJwtService _jwtService;
        private readonly ILogger<LoginHandler> _logger;

        public LoginHandler(IAuthService authService, IJwtService jwtService, ILogger<LoginHandler> logger)
        {
            _authService = authService;
            _jwtService = jwtService;
            _logger = logger;
        }

        protected override async Task<Result<LoginResponseDto>> HandleRequest(LoginRequest request)
        {
            string? token;
                
            try
            {
                token = await _authService.GetTokenAsync(request.Username, request.Password);
                if (token == null)
                    return Result<LoginResponseDto>.Failure(ErrorCode.InvalidInput, "Invalid email or password.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while retrieving token for user {Username}.", request.Username);
                return Result<LoginResponseDto>.Failure(ErrorCode.ExternalService, "Service not available.");
            }

            User? user;
            try
            {
                user = await _authService.GetCurrentUserAsync(token);
                if (user == null)
                    return Result<LoginResponseDto>.Failure(ErrorCode.NotFound, "Token missing.");
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while retrieving current user for user {Username}.", request.Username);
                return Result<LoginResponseDto>.Failure(ErrorCode.ExternalService, "Service not available.");
            }

            var jwt = _jwtService.GenerateToken(user!.Id, user.Username, user.Role);

            return Result<LoginResponseDto>.Success(new LoginResponseDto { Token = jwt });
        }
    }
}
