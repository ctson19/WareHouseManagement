using Microsoft.AspNetCore.Mvc;
using WareManagement.DTO.ProductDTO;
using WareManagement.Service.Exceptions;
using WareManagement.Service.Interfaces;

namespace WareManagement.Controllers;

[Route("api/[controller]")]
public class ProductController : BaseApiController
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet("units")]
    public async Task<IActionResult> GetUnits()
    {
        try
        {
            var userId = GetCurrentUserId();
            var units = await _productService.GetUnitsAsync(userId);
            return Ok(units);
        }
        catch (ForbiddenException)
        {
            return Forbid();
        }
        catch (Exception)
        {
            return Problem();
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetPaged([FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _productService.GetPagedAsync(userId, search, page, pageSize);
            return Ok(new { items = result.Items, total = result.Total, page, pageSize });
        }
        catch (ForbiddenException)
        {
            return Forbid();
        }
        catch (Exception)
        {
            return Problem();
        }
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var dto = await _productService.GetByIdAsync(userId, id);
            return Ok(dto);
        }
        catch (ForbiddenException)
        {
            return Forbid();
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception)
        {
            return Problem();
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductRequestDto request)
    {
        if (request is null)
            return BadRequest(new { message = "Yêu cầu không được để trống" });

        try
        {
            var userId = GetCurrentUserId();
            var created = await _productService.CreateAsync(userId, request);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ForbiddenException)
        {
            return Forbid();
        }
        catch (ConflictException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception)
        {
            return Problem();
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProductRequestDto request)
    {
        if (request is null)
            return BadRequest(new { message = "Yêu cầu không được để trống" });

        try
        {
            var userId = GetCurrentUserId();
            var updated = await _productService.UpdateAsync(userId, id, request);
            return Ok(updated);
        }
        catch (ForbiddenException)
        {
            return Forbid();
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ConflictException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception)
        {
            return Problem();
        }
    }
}
