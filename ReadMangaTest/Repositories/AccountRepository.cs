using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ReadMangaTest.Data;
using ReadMangaTest.DTO;
using ReadMangaTest.Interfaces;
using ReadMangaTest.Models;



namespace ReadMangaTest.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;
    private readonly IConfiguration _config;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly TokenOptions _tokenOptions;

    public AccountRepository(IMapper mapper, DataContext context, IHttpContextAccessor httpContextAccessor, IConfiguration config, IOptions<TokenOptions> tokenOptions)
    {
        _mapper = mapper;
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _config = config;
        _tokenOptions = tokenOptions.Value;
    }

    public async Task<AccountDto> RegisterAsync(AccountDto accountDto)
    {
        try
        {
            accountDto.Username = accountDto.Username.ToLower();
            // var currentUser = await _context.Accounts.Where(p => p.Username == accountDto.Username).FirstOrDefaultAsync();
            var currentAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.Username == accountDto.Username);

            if (currentAccount != null)
            {
                throw new Exception("Username already exists");
            }

            using var hmac = new HMACSHA512();
            var passwordBytes = Encoding.UTF8.GetBytes(accountDto.Password);

            var account = new Account()
            {
                Username = accountDto.Username,
                PasswordHash = hmac.ComputeHash(passwordBytes),
                PasswordSalt = hmac.Key,
                Role = accountDto.Role,
            };
            
            await _context.Accounts.AddAsync(account);
            await _context.SaveChangesAsync();
            
            return accountDto;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public Account GetCurrentUser()
    {
        if (_httpContextAccessor.HttpContext?.User.Identity is ClaimsIdentity identity)
        {
            var accountClaims = identity.Claims;

            return new Account()
            {
                Username = accountClaims.FirstOrDefault(o => o.Type == ClaimTypes.NameIdentifier)?.Value,
                Role = accountClaims.FirstOrDefault(o => o.Type == ClaimTypes.Role)?.Value,
            };
        }
        return null;
    }
    public async Task<AccountDto> LoginAsync(AccountDto accountDto)
    {
        try
        {
            accountDto.Username = accountDto.Username.ToLower();

            var currentUser = await _context.Accounts.FirstOrDefaultAsync(p => p.Username == accountDto.Username);
            if (currentUser == null)
            {
                throw new Exception("Account not found!");
            }
            using var hmac = new HMACSHA512(currentUser.PasswordSalt);
            var passwordBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(accountDto.Password));
            for (int i = 0; i < currentUser.PasswordHash.Length; i++)
            {
                if (currentUser.PasswordHash[i] != passwordBytes[i])
                {
                    return null;
                }
            }

            return accountDto;
        }
        catch (Exception ex)
        {
            throw new Exception("Message: " + ex.Message);
        }
    }
    
    

    public Task<AccountDto> LogoutAsync()
    {
        throw new NotImplementedException();
    }

    public Account GetCurrentUserAsync()
    {
        if (_httpContextAccessor.HttpContext?.User.Identity is ClaimsIdentity identity)
        {
            var userClaims = identity.Claims;

            return new Account
            {
                Username = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.NameIdentifier)?.Value,
                Role = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Role)?.Value,
            };
        }
        return null;
    }
 
    public async Task<string> GenerateTokenAsync(AccountDto accountDto)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var account = await _context.Accounts.FirstOrDefaultAsync(p => p.Username == accountDto.Username);

        var claims = new[]
        {
            new Claim(ClaimTypes.Role, account.Role),
        };

        var token = new JwtSecurityToken(_config["Jwt:Issuer"],
            _config["JWT:Audience"],
            claims,
            expires: DateTime.Now.AddMinutes(15),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    public string GetCurrentToken()
    {
        string token = string.Empty;

        // Lấy token từ header hoặc cookie của request
        var authorizationHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"];
        if (!string.IsNullOrEmpty(authorizationHeader))
        {
            token = authorizationHeader.ToString()!.Replace("Bearer ", string.Empty);
        }
        else
        {
            var cookie = _httpContextAccessor.HttpContext?.Request.Cookies["token"];
            if (cookie != null)
            {
                token = cookie;
                // _httpContextAccessor.HttpContext?.Response.Cookies.Delete();
            }
        }
        Console.WriteLine(token);
        return token;
    }
}