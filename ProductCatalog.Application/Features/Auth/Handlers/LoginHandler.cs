using ProductCatalog.Application.Common;
using ProductCatalog.Application.DTOs;
using ProductCatalog.Application.Features.Auth.Requests;
using ProductCatalog.Application.Interfaces;
using ProductCatalog.Domain.Common;
using ProductCatalog.Domain.Enums;
using ProductCatalog.Domain.Interfaces;

namespace ProductCatalog.Application.Features.Auth.Handlers
{
    public class LoginHandler : RequestHandler<LoginRequest, LoginResponseDto>
    {
        private readonly IAuthRepository _authRepository;
        private readonly IJwtService _jwtService;

        public LoginHandler(IAuthRepository authRepository, IJwtService jwtService)
        {
            _authRepository = authRepository;
            _jwtService = jwtService;
        }

        protected override async Task<Result<LoginResponseDto>> HandleRequest(LoginRequest request)
        {
            var token = await _authRepository.GetTokenAsync(request.Username, request.Password);
            if(token == null)
            {
                return Result<LoginResponseDto>.Failure(ErrorCode.InvalidInput, "Invalid email or password.");
            }

            var user = await _authRepository.GetCurrentUserAsync(token);

            var jwt = _jwtService.GenerateToken(user!.Id, user.Username, user.Role);

            return Result<LoginResponseDto>.Success(new LoginResponseDto { Token = jwt });
        }
    }
}
