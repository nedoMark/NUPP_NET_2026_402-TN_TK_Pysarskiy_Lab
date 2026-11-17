using Microsoft.AspNetCore.Authorization;
using Game;
using Game.Infrastructure.Models;
using Game.REST.Models;
using Microsoft.AspNetCore.Mvc;
namespace Game.REST.Controllers;
[ApiController]
[Route("api/weapons")]
public sealed class WeaponsController(ICrudServiceAsync<WeaponModel> service) : ControllerBase
{
    private static WeaponResponse Map(WeaponModel item) => new(item.Id, item.Name, item.Damage);
    [HttpGet]
    public async Task<ActionResult<IEnumerable<WeaponResponse>>> GetAll([FromQuery] int page = 1, [FromQuery] int amount = 20)
    {
        if (page < 1 || amount < 1 || amount > 100 || (long)(page - 1) * amount > int.MaxValue) return BadRequest("Неправильні параметри сторінки.");
        return Ok((await service.ReadAllAsync(page, amount)).Select(Map));
    }
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<WeaponResponse>> Get(Guid id)
    {
        try { return Ok(Map(await service.ReadAsync(id))); }
        catch (KeyNotFoundException) { return NotFound(); }
    }
    [Authorize(Roles = "Editor,Admin")]
    [HttpPost]
    public async Task<ActionResult<WeaponResponse>> Create(WeaponRequest request)
    {
        var entity = new WeaponModel { Name = request.Name, Damage = request.Damage };
        await service.CreateAsync(entity);
        return CreatedAtAction(nameof(Get), new { id = entity.Id }, Map(entity));
    }
    [Authorize(Roles = "Editor,Admin")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, WeaponRequest request)
    {
        WeaponModel entity;
        try { entity = await service.ReadAsync(id); } catch (KeyNotFoundException) { return NotFound(); }
        entity.Name = request.Name; entity.Damage = request.Damage;
        return await service.UpdateAsync(entity) ? NoContent() : NotFound();
    }
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        WeaponModel entity;
        try { entity = await service.ReadAsync(id); } catch (KeyNotFoundException) { return NotFound(); }
        return await service.RemoveAsync(entity) ? NoContent() : NotFound();
    }
}

