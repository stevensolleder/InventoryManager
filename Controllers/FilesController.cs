using System.Net;
using System.Threading.Tasks;
using InventoryManager.Interfaces;
using Microsoft.AspNetCore.Mvc;
using InventoryManager.Model;
using InventoryManager.Services;

namespace InventoryManager.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/files")]
[ApiVersion("1.0")]
public class DataFilesController : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetDataFile([FromServices] DatabaseContext databaseContext, [FromServices] IFileService fileService, [FromRoute] uint id, [FromQuery] bool preview = false)
    {
        DataFile? dataFile = await databaseContext.DataFiles.FindAsync(id);
        if(dataFile == null) return Problem(statusCode: (int) HttpStatusCode.NotFound, detail: "File not found");
        
        (string fileName, string name) = (preview && dataFile is ImageDataFile imageDataFile) ? (imageDataFile.PreviewFileName, imageDataFile.PreviewName) : (dataFile.FileName, dataFile.Name);
        
        byte[]? content = await fileService.GetFile(fileName);
        if (content==null) return Problem(statusCode: (int) HttpStatusCode.InternalServerError, title: "File not found on server");
        
        return File(content, dataFile.MimeType, name);
    }
}