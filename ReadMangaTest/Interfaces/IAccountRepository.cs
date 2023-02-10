using ReadMangaTest.DTO;
using ReadMangaTest.Models;

namespace ReadMangaTest.Interfaces;

public interface IAccountRepository
{
    Task<AccountDto> RegisterAsync(AccountDto account);
    Task<AccountDto> LoginAsync(AccountDto account);
    Task<AccountDto> LogoutAsync();
    Account GetCurrentUserAsync();
    
    string GetCurrentToken();
    Task<string> GenerateTokenAsync(AccountDto account);
}