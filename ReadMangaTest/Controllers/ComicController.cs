using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ReadMangaTest.Interfaces;
using ReadMangaTest.Models;

namespace ReadMangaTest.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ComicController: ControllerBase
{
    private readonly IComicRepository _repository;

    public ComicController(IComicRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerable<Comic>))]
    public async Task<IActionResult> GetComics()
    {
        var comics = await _repository.GetAllAsync();
        

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        return Ok(comics);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(200, Type = typeof(Comic))]
    [ProducesResponseType(400)]
    public async Task<IActionResult> GetComic(int id)
    {
        // if (!_characterRepository.AnyCharacterExists(id))
        //     return NotFound();
        var comic = await _repository.GetByIdAsync(id);
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        return Ok(comic);
    }

    [HttpPost]
    [ProducesResponseType(200, Type = typeof(Comic))]
    [ProducesResponseType(400)]
    public async Task<IActionResult> PostComic(Comic comic)
    {
        await _repository.AddAsync(comic);
        return CreatedAtAction(nameof(GetComic), new { id = comic.Id }, comic);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(200, Type = typeof(Comic))]
    [ProducesResponseType(400)]
    public async Task<IActionResult> PutComic(int id, Comic comic)
    {
        if (id != comic.Id)
        {
            return BadRequest();
        }
        await _repository.UpdateAsync(comic);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(200, Type = typeof(Comic))]
    [ProducesResponseType(400)]
    public async Task<IActionResult> DeleteComic(int id)
    {
        var comic = await _repository.GetByIdAsync(id);
        if (comic == null)
        {
            return NotFound();
        }
        await _repository.DeleteAsync(id);
        return NoContent();
    }
}