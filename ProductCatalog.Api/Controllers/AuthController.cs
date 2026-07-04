using Microsoft.AspNetCore.Mvc;
using ProductCatalog.Application.Features.Auth.Requests;
using ProductCatalog.Application.Features.Auth.Handlers;

namespace ProductCatalog.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, [FromServices] LoginHandler handler)
    {
        var result = await handler.HandleAsync(request);

        if (!result.IsSuccess)
            return BadRequest(result.ErrorMessage);

        return Ok(result.Value);
    }
}