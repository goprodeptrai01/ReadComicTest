using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ReadMangaTest.Data;
using ReadMangaTest.DTO;
using ReadMangaTest.Interfaces;

namespace ReadMangaTest.Controllers;

[Route("v1/api/chapter")]
[ApiController]
public class ChapterController : ControllerBase
{
    private readonly IArtistRepository _artistRepository;
    private readonly IComicRepository _comicRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IChapterRepository _chapterRepository;
    private readonly DataContext _context;
    private readonly IMapper _mapper;
    private readonly IUriService _uriService;

    public ChapterController(IArtistRepository artistRepository, IComicRepository comicRepository, ICategoryRepository categoryRepository, IChapterRepository chapterRepository, DataContext context, IMapper mapper, IUriService uriService)
    {
        _artistRepository = artistRepository;
        _comicRepository = comicRepository;
        _categoryRepository = categoryRepository;
        _chapterRepository = chapterRepository;
        _context = context;
        _mapper = mapper;
        _uriService = uriService;
    }
    
    
    [HttpGet]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> GetChaptersAsync()
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var chapters = await _chapterRepository.GetAllAsync();

            return Ok(chapters);
        }
        catch (Exception e)
        {
            return StatusCode(500, "Internal Server Error" + e.Message);
        }

    }
    
    [HttpGet("{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetChapterAsync(int id)
    {
        if (!_chapterRepository.IsExists(id))
            return NotFound();

        var chapter = await _chapterRepository.GetByIdAsync(id);

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return Ok(chapter);
    }
    
    [HttpGet("{comicId}/comic")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetChaptersByComicAsync(int comicId)
    {
        if (!_chapterRepository.IsExists(comicId))
            return NotFound();

        var chapters = await _chapterRepository.GetChaptersByComicAsync(comicId);

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return Ok(chapters);
    }

    [HttpPost("{comicId}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> PostComicAsync([FromBody] ChapterDto chapterDto, [FromRoute] int comicId)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (chapterDto == null)
            return BadRequest("Null");

        if (_chapterRepository.IsExists(chapterDto.Name, chapterDto.Id))
        {
            ModelState.AddModelError("Name", "Chapter already exists");
            return StatusCode(422, ModelState);
        }

        try
        {
            await _chapterRepository.AddAsync(chapterDto, comicId);
            return Ok(chapterDto);
        }
        catch (Exception e)
        {
            return StatusCode(500, "Internal Server Error " + e);
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> PutComicAsync([FromRoute] int id, [FromBody] ChapterDto chapterDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (chapterDto == null)
        {
            return BadRequest("Null");
        }
        
        if (id!= chapterDto.Id)
            return BadRequest("Invalid comic id");

        if (_chapterRepository.IsAvailable(chapterDto.Name, id))
        {
            ModelState.AddModelError("Name", "Chapter already exists");
            return StatusCode(422, ModelState);
        }
        
        try
        {
            await _chapterRepository.UpdateAsync(chapterDto, id);
            return Ok(chapterDto);
        }
        catch (Exception e)
        {
            return StatusCode(500, "Internal Server Error" + e.Message);
        }
    }

    [HttpPut("{id}/store")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> StoreAsync([FromRoute] int id)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            await _chapterRepository.StoreAsync(id);
            return Ok("Stored!");
        }
        catch (Exception e)
        {
            return StatusCode(500, e);
        }
    }
    
    [HttpPut("{id}/multipal-store")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> MultiStoreAsync([FromRoute] string ids)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var idArray = ids.Split(',').Select(int.Parse).ToArray();
            
            await _chapterRepository.MultiStoreAsync(idArray);
            return Ok("Stored!");
        }
        catch (Exception e)
        {
            return StatusCode(500, e);
        }
    }
    
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            await _chapterRepository.DeleteAsync(id);
            return Ok("Deleted!");
        }
        catch (Exception e)
        {
            return StatusCode(500, e);
        }
    }
    
    [HttpDelete("{id}/multipal-delete")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> MultiDeleteAsync([FromRoute] string ids)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var idArray = ids.Split(',').Select(int.Parse).ToArray();
            
            await _chapterRepository.MultiDeleteAsync(idArray);
            return Ok("Deleted!");
        }
        catch (Exception e)
        {
            return StatusCode(500, e);
        }
    }
}