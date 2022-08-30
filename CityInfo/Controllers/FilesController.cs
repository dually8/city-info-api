using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace CityInfo.API.Controllers;

[Route("api/files")]
[ApiController]
public class FilesController : Controller
{
    private readonly FileExtensionContentTypeProvider _provider;

    public FilesController(
        FileExtensionContentTypeProvider provider
    )
    {
        _provider = provider ?? throw new System.ArgumentNullException((nameof(provider)));
    }

    [HttpGet("{fileId}")]
    public ActionResult GetFile(string fileId)
    {
        var pathToFile = "TestFile.txt";

        if (!System.IO.File.Exists(pathToFile))
        {
            return NotFound();
        }

        if (!_provider.TryGetContentType(pathToFile, out var contentType))
        {
            contentType = "application/octet-stream"; // default binary type
        }

        var bytes = System.IO.File.ReadAllBytes(pathToFile);
        return File(bytes, contentType, Path.GetFileName(pathToFile));
    }
}