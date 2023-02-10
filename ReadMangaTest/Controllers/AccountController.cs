using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ReadMangaTest.Data;
using ReadMangaTest.DTO;
using ReadMangaTest.Interfaces;

namespace ReadMangaTest.Controllers;

[Route("v1/api/auth")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly IAccountRepository _accountRepository;
    private readonly DataContext _context;
    private readonly IMapper _mapper;

    public AccountController(IMapper mapper, DataContext context, IAccountRepository accountRepository)
    {
        _mapper = mapper;
        _context = context;
        _accountRepository = accountRepository;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<AccountDto>> RegisterAsync([FromBody] AccountDto accountDto)
    {
        if (accountDto == null)
            return BadRequest();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            await _accountRepository.RegisterAsync(accountDto);
            return Ok(accountDto);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<AccountDto>> LoginAsync([FromBody] LoginDto accountDto )
    {
        if (accountDto == null)
            return BadRequest();

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            // var token = "";
            // if (await _accountRepository.LoginAsync(_mapper.Map<AccountDto>(accountDto)) != null)
            // {
            //     token = await _accountRepository.GenerateTokenAsync(await _accountRepository.LoginAsync(_mapper.Map<AccountDto>(accountDto)));
            // }
            var token = await _accountRepository.LoginAsync(_mapper.Map<AccountDto>(accountDto)) != null ? await _accountRepository.GenerateTokenAsync(await _accountRepository.LoginAsync(_mapper.Map<AccountDto>(accountDto))) : null;
            return Ok(token);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
    
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> LogoutAsync()
    {
        // Lấy ra jwt token trong request header
        _accountRepository.GetCurrentToken();
        // Huỷ token trong bộ nhớ cache hoặc trong cơ sở dữ liệu

        return Ok();
    }

}