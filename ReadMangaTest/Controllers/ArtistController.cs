using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ReadMangaTest.Data;
using ReadMangaTest.DTO;
using ReadMangaTest.Interfaces;

namespace ReadMangaTest.Controllers;

[Route("v1/api/artist")]
[ApiController]
public class ArtistController : ControllerBase
{
    private readonly IArtistRepository _artistRepository;
    private readonly IComicRepository _comicRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IChapterRepository _chapterRepository;
    private readonly DataContext _context;
    private readonly IMapper _mapper;
    private readonly IUriService _uriService;

    public ArtistController(IUriService uriService, IMapper mapper, DataContext context, IChapterRepository chapterRepository, ICategoryRepository categoryRepository, IComicRepository comicRepository, IArtistRepository artistRepository)
    {
        _uriService = uriService;
        _mapper = mapper;
        _context = context;
        _chapterRepository = chapterRepository;
        _categoryRepository = categoryRepository;
        _comicRepository = comicRepository;
        _artistRepository = artistRepository;
    }


    [HttpGet]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> GetArtistsAsync()
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var artists = await _artistRepository.GetAllAsync();
            return Ok(artists);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, "Internal Server Error " + e.Message);
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> GetArtistAsync(int id)
    {
        if (!_artistRepository.IsExists(id))
            return NotFound();
        var artist = await _artistRepository.GetByIdAsync(id);
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        return Ok(artist);
    }

    [HttpPost]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> PostArtistAsync([FromBody] PostArtistDto artistDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        if (artistDto == null)
            return BadRequest("Null");

        if (_artistRepository.IsExists(artistDto.Name, artistDto.Id))
        {
            ModelState.AddModelError("Name", "Artist already exists");
            return StatusCode(422, ModelState);
        }

        try
        {
            await _artistRepository.AddAsync(_mapper.Map<ArtistDto>(artistDto));
            return Ok(artistDto);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, "Internal Server Error "+ e);
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> PutArtistAsync([FromBody] PostArtistDto portArtistDto, [FromRoute] int id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (id == null)
        {
            return BadRequest("Null");
        }

        if (id != portArtistDto.Id)
        {
            return BadRequest("Invalid artist id");
        }

        if (_artistRepository.IsExists(portArtistDto.Name, id))
        {
            ModelState.AddModelError("Name", "Artist already exists");
            return StatusCode(422, ModelState);
        }

        try
        {
            var artistDto = _mapper.Map<ArtistDto>(portArtistDto);
            await  _artistRepository.UpdateAsync(artistDto, id);
            return Ok(artistDto);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, "Internal Server Error "+ e);
        }
    }

    [HttpPut("{id}/store")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> StoreAsync([FromRoute] int id)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            await _artistRepository.StoreAsync(id);
            return Ok("Stored!");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, "Internal Server Error "+ e);
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> DeleteAsync([FromRoute] int id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (id == 1)
        {
            return BadRequest("Undeletable artist!");
        }

        try
        {
            await _artistRepository.DeleteAsync(id);
            return Ok("Deleted!");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, "Internal Server Error "+ e);
        }
    }
}