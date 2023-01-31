using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ReadMangaTest.Data;
using ReadMangaTest.DTO;
using ReadMangaTest.Interfaces;
using ReadMangaTest.Models;

namespace ReadMangaTest.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ComicController : ControllerBase
{
    private readonly IArtistRepository _artistRepository;
    private readonly IComicRepository _comicRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IChapterRepository _chapterRepository;
    private readonly DataContext _context;
    private readonly IMapper _mapper;

    public ComicController(IComicRepository repository, IArtistRepository artistRepository, DataContext context,
        IMapper mapper, ICategoryRepository categoryRepository, IChapterRepository chapterRepository)
    {
        _comicRepository = repository;
        _artistRepository = artistRepository;
        _categoryRepository = categoryRepository;
        _chapterRepository = chapterRepository;
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerable<Comic>))]
    public async Task<IActionResult> GetComics()
    {
        try
        {
            var comics = await _comicRepository.GetAllAsync();


            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(comics);

        }
        catch (Exception e)
        {
            return StatusCode(500, "Internal Server Error" + e.Message);
        }

    }

    [HttpGet("{id}")]
    [ProducesResponseType(200, Type = typeof(Comic))]
    [ProducesResponseType(400)]
    public async Task<IActionResult> GetComic(int id)
    {
        // if (!_characterRepository.AnyCharacterExists(id))
        //     return NotFound();
        try
        {
            var comic = await _comicRepository.GetByIdAsync(id);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(comic);
        }
        catch (Exception e)
        {
            return StatusCode(500, "Internal Server Error" + e.Message);
        }
    }

    [HttpPost("{artistId}")]
    [ProducesResponseType(201, Type = typeof(Comic))]
    [ProducesResponseType(400)]
    public async Task<IActionResult> PostComic([FromBody] ComicDto comicDto, [FromRoute] int artistId)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var comic = _mapper.Map<Comic>(comicDto);

            comic.Artist = await _artistRepository.GetByIdAsync(artistId);
            _context.Comics.Add(comic);
            // await _comicRepository.AddArtistToComic(artistId, comic);
            await _context.SaveChangesAsync();

            return Ok(comic);
        }
        catch (Exception e)
        {
            return StatusCode(500, "Internal Server Error " + e);
        }
    }

    [HttpPost("{comicId}/Category")]
    [ProducesResponseType(201, Type = typeof(Comic))]
    [ProducesResponseType(400)]
    public async Task<IActionResult> AddCategoryToComic([FromRoute] int comicId, [FromBody] CategoryDto categoryDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // var category = _mapper.Map<Category>(categoryDto);
        try
        {
            var category = _mapper.Map<Category>(categoryDto);
            var comic = await _context.Comics.FindAsync(comicId);
            if (comic == null)
            {
                return NotFound();
            }

            var comicCategory = new ComicCategory()
            {
                ComicId = comicId,
                Comic = comic,
                CategoryId = category.Id,
                Category = category
            };
            comic.comicCategories.Add(comicCategory);
            category.comicCategories.Add(comicCategory);
            await _context.SaveChangesAsync();

            return Ok();
        }
        catch (Exception e)
        {
            return StatusCode(500, "Internal Server Error" + e.Message);
        }
    }

    [HttpPost("{comicId}/Artist")]
    [ProducesResponseType(201, Type = typeof(Comic))]
    [ProducesResponseType(400)]
    public async Task<IActionResult> AddArtistToComic([FromRoute] int comicId, [FromBody] ArtistDto artistDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var artist = _mapper.Map<Artist>(artistDto);
            var comic = await _context.Comics.FindAsync(comicId);
            if (comic == null)
            {
                return NotFound();
            }

            comic.Artist = artist;
            // await _context.SaveChangesAsync();
            artist.Comics.Add(comic);
            await _context.SaveChangesAsync();
            return Ok();
        }
        catch (Exception e)
        {
            return StatusCode(500, "Internal Server Error" + e.Message);
        }
    }

    [HttpPut("{comicId}")]
    [ProducesResponseType(200, Type = typeof(Comic))]
    [ProducesResponseType(400)]
    public async Task<IActionResult> PutComic([FromRoute] int comicId, [FromBody] ComicDto comicDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var comic = _mapper.Map<Comic>(comicDto);
            await _comicRepository.UpdateAsync(comic);
            return NoContent();
        }
        catch (Exception e)
        {
            return StatusCode(500, "Internal Server Error" + e.Message);
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(200, Type = typeof(Comic))]
    [ProducesResponseType(400)]
    public async Task<IActionResult> DeleteComic(int id)
    {
        var comic = await _comicRepository.GetByIdAsync(id);
        if (comic == null)
        {
            return NotFound();
        }

        await _comicRepository.DeleteAsync(id);
        return NoContent();
    }
}