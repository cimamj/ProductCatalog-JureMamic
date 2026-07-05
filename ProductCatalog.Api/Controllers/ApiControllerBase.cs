using Microsoft.AspNetCore.Mvc;
using ProductCatalog.Domain.Common;
using ProductCatalog.Domain.Enums;

namespace ProductCatalog.Api.Controllers
{
    [ApiController]
    public abstract class ApiControllerBase : ControllerBase
    {
        protected static IActionResult ToActionResult<T>(Result<T> result)
        {
            if (result.IsSuccess)
                return new OkObjectResult(result);

            return result.ErrorCode switch
            {
                ErrorCode.NotFound => new NotFoundObjectResult(result),
                ErrorCode.InvalidInput => new BadRequestObjectResult(result),
                ErrorCode.ExternalService => new ObjectResult(result) { StatusCode = StatusCodes.Status503ServiceUnavailable },
                _ => new ObjectResult(result) { StatusCode = StatusCodes.Status500InternalServerError }
            };
        }
    }
}
