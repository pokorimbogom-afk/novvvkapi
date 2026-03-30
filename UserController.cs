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

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (request == null || string.IsNullOrEmpty(request.Login) || string.IsNullOrEmpty(request.Pass))
        {
            return BadRequest(new { success = false, error = "Invalid data" });
        }

        var user = await _repository.GetUserByUsername(request.Login);
        if (user == null)
        {
            return Unauthorized(new { success = false, error = "Invalid credentials" });
        }

        if (!BCrypt.Net.BCrypt.Verify(request.Pass, user.PasswordHash))
        {
            return Unauthorized(new { success = false, error = "Invalid credentials" });
        }

        return Ok(new { 
            success = true, 
            user = new {
                id = user.Id,
                login = user.Username,
                role = user.Role,
                hwid = user.Hwid,
                subscription = user.Subscription
            }
        });
    }

    [HttpGet("changelog.txt")]
    public async Task<IActionResult> GetChangelog()
    {
        var changelogData = await _repository.GetActiveChangelog();
        if (changelogData == null)
        {
            return Content("#date=Нет данных\n❌ История обновлений пуста", "text/plain; charset=utf-8");
        }
        
        var changelog = $"#date={changelogData.Date}\n{changelogData.Content}";
        return Content(changelog, "text/plain; charset=utf-8");
    }
}
