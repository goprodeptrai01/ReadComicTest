namespace ReadMangaTest.DTO;

public class AccountDto
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }
}

public class LoginDto
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}