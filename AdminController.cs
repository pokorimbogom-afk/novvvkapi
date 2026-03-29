using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[ApiController]
[Route("api/admin")]
public class AdminController : ControllerBase
{
    private readonly IUserRepository _repository;

    public AdminController(IUserRepository repository)
    {
        _repository = repository;
    }

    [HttpPost("set-role")]
    public async Task<IActionResult> SetRole([FromBody] SetRoleRequest request)
    {
        if (request == null || string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Role))
        {
            return BadRequest(new { success = false, message = "Invalid data" });
        }

        var validRoles = new[] { "User", "Vip", "Owner" };
        if (!System.Array.Exists(validRoles, r => r == request.Role))
        {
            return BadRequest(new { success = false, message = "Invalid role" });
        }

        var updated = await _repository.UpdateUserRole(request.Username, request.Role);
        if (!updated)
        {
            return NotFound(new { success = false, message = "User not found" });
        }

        return Ok(new { success = true, message = "Role updated" });
    }
}

public class SetRoleRequest
{
    public string Username { get; set; }
    public string Role { get; set; }
}
