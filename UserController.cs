using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[ApiController]
[Route("api")]
public class UserController : ControllerBase
{
    private readonly IUserRepository _repository;

    public UserController(IUserRepository repository)
    {
        _repository = repository;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (request == null || string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
        {
            return BadRequest(new { success = false, message = "Invalid data" });
        }

        var exists = await _repository.UserExists(request.Username);
        if (exists)
        {
            return Conflict(new { success = false, message = "User already exists" });
        }

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
        await _repository.CreateUser(request.Username, hashedPassword);

        return Ok(new { success = true });
    }
}
