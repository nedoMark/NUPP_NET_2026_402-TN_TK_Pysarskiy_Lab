using Microsoft.AspNetCore.Authorization;
using Game;
using Game.Infrastructure.Models;
using Game.REST.Models;
using Microsoft.AspNetCore.Mvc;
namespace Game.REST.Controllers;
[ApiController]
[Route("api/characters")]
public sealed class CharactersController(ICrudServiceAsync<CharacterModel> service) : ControllerBase
{
    private static CharacterResponse Map(CharacterModel item) => new(item.Id, item.Name, item.Health);
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CharacterResponse>>> GetAll([FromQuery] int page = 1, [FromQuery] int amount = 20)
    {
        if (page < 1 || amount < 1 || amount > 100 || (long)(page - 1) * amount > int.MaxValue) return BadRequest("Неправильні параметри сторінки.");
        return Ok((await service.ReadAllAsync(page, amount)).Select(Map));
    }
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CharacterResponse>> Get(Guid id)
    {
        try { return Ok(Map(await service.ReadAsync(id))); }
        catch (KeyNotFoundException) { return NotFound(); }
    }
    [Authorize(Roles = "Editor,Admin")]
    [HttpPost]
    public async Task<ActionResult<CharacterResponse>> Create(CharacterRequest request)
    {
        var entity = new CharacterModel { Name = request.Name, Health = request.Health };
        await service.CreateAsync(entity);
        return CreatedAtAction(nameof(Get), new { id = entity.Id }, Map(entity));
    }
    [Authorize(Roles = "Editor,Admin")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, CharacterRequest request)
    {
        CharacterModel entity;
        try { entity = await service.ReadAsync(id); } catch (KeyNotFoundException) { return NotFound(); }
        entity.Name = request.Name; entity.Health = request.Health;
        return await service.UpdateAsync(entity) ? NoContent() : NotFound();
    }
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        CharacterModel entity;
        try { entity = await service.ReadAsync(id); } catch (KeyNotFoundException) { return NotFound(); }
        return await service.RemoveAsync(entity) ? NoContent() : NotFound();
    }
}

