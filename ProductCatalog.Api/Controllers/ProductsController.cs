using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductCatalog.Application.Features.Products.Handlers;
using ProductCatalog.Application.Features.Products.Requests;

namespace ProductCatalog.Api.Controllers;

[Route("api/[controller]")]
[Authorize]
public class ProductsController : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromServices] GetAllProductsHandler handler)
    {
        var result = await handler.HandleAsync(new GetAllProductsRequest());
        return ToActionResult(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, [FromServices] GetProductByIdHandler handler)
    {
        var result = await handler.HandleAsync(new GetProductByIdRequest { Id = id });
        return ToActionResult(result);
    }

    [HttpGet("filter")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Filter(
        [FromQuery] string? category,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromServices] FilterProductsHandler handler)
    {
        var result = await handler.HandleAsync(new FilterProductsRequest
        {
            Category = category,
            MinPrice = minPrice,
            MaxPrice = maxPrice
        });

        return ToActionResult(result);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string? searchTerm, [FromServices] SearchProductsHandler handler)
    {
        var result = await handler.HandleAsync(new SearchProductsRequest { SearchTerm = searchTerm });
        return ToActionResult(result);
    }
}