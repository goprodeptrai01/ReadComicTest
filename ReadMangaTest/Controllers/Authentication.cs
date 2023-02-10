// using System.IdentityModel.Tokens.Jwt;
// using System.Security.Claims;
// using System.Text;
// using DevOne.Security.Cryptography.BCrypt;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.IdentityModel.Tokens;
// using ReadMangaTest.Models;
//
// namespace ReadMangaTest.Controllers;
//
// [Route("v1/api/auth")]
// [ApiController]
// public class Authentication : ControllerBase
// {
//     [HttpPost]
//     [Route("api/[controller]/[action]")]
//     public IActionResult Login(Account account)
//     {
//         // Verify credentials and role from database
//         var role = "admin";
//
//         // Generate JWT token
//         var tokenHandler = new JwtSecurityTokenHandler();
//         var key = Encoding.ASCII.GetBytes(Configuration["Jwt:Key"]);
//         var tokenDescriptor = new SecurityTokenDescriptor
//         {
//             Subject = new ClaimsIdentity(new[]
//             {
//                 new Claim(ClaimTypes.Name, account.Username),
//                 new Claim(ClaimTypes.Role, account.Role)
//             }),
//             // Expires = DateTime.UtcNow.AddHours(2),
//             SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
//         };
//         var token = tokenHandler.CreateToken(tokenDescriptor);
//         var tokenString = tokenHandler.WriteToken(token);
//
//         // Return the token
//         return Ok(new { token });
//     }
//     
//     [HttpPost]
//     [Route("api/[controller]/[action]")]
//     public IActionResult Logout()
//     {
//         // Invalidate the JWT token
//         // ...
//
//         return Ok();
//     }
//     
//     [HttpPost]
//     [Route("api/[controller]/[action]")]
//     public IActionResult SignUp(Account account)
//     {
//         // Hash the password
//         account.Password = BCryptHelper.HashPassword(account.Password, BCryptHelper.GenerateSalt());
//
//         // Save the account to database
//         // ...
//
//         return Ok();
//     }
// }