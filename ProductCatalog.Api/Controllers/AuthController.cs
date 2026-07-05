using Microsoft.AspNetCore.Mvc;
using ProductCatalog.Application.Features.Auth.Requests;
using ProductCatalog.Application.Features.Auth.Handlers;

namespace ProductCatalog.Api.Controllers;

[Route("api/[controller]")]
public class AuthController : ApiControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, [FromServices] LoginHandler handler)
    {
        var result = await handler.HandleAsync(request);
        return ToActionResult(result);
    }
}