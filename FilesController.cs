using Microsoft.AspNetCore.Mvc;
using System.IO;

[ApiController]
[Route("[controller]")]
public class FilesController : ControllerBase
{
    [HttpGet("launcher/version.txt")]
    public IActionResult GetLauncherVersion()
    {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "launcher", "version.txt");
        if (!System.IO.File.Exists(filePath))
            return NotFound();
        
        var content = System.IO.File.ReadAllText(filePath);
        return Content(content, "text/plain");
    }

    [HttpGet("launcher/client.zip")]
    public IActionResult GetClientZip()
    {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "launcher", "client.zip");
        if (!System.IO.File.Exists(filePath))
            return NotFound();
        
        var fileBytes = System.IO.File.ReadAllBytes(filePath);
        return File(fileBytes, "application/zip", "client.zip");
    }

    [HttpGet("loader/loader_version.txt")]
    public IActionResult GetLoaderVersion()
    {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "loader", "loader_version.txt");
        if (!System.IO.File.Exists(filePath))
            return NotFound();
        
        var content = System.IO.File.ReadAllText(filePath);
        return Content(content, "text/plain");
    }

    [HttpGet("loader/loader.exe")]
    public IActionResult GetLoaderExe()
    {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "loader", "loader.exe");
        if (!System.IO.File.Exists(filePath))
            return NotFound();
        
        var fileBytes = System.IO.File.ReadAllBytes(filePath);
        return File(fileBytes, "application/octet-stream", "loader.exe");
    }
}
