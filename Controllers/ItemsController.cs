using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using InventoryManager.DataTransferObjects;
using InventoryManager.Interfaces;
using InventoryManager.Model;
using InventoryManager.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryManager.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/items")]
[ApiVersion("1.0")]
public class ItemsController : ControllerBase
{
    private readonly DatabaseContext _databaseContext;
    private readonly IFileService _fileService;
    
    public ItemsController(DatabaseContext databaseContext, IFileService fileService)
    {
        _databaseContext = databaseContext;
        _fileService = fileService;
    }

    
    [HttpPost("")]
    public async Task<ActionResult<CreatedItem>> CreateItem() 
    {
        Item newItem=new Item("New Item");
        
        _databaseContext.Items.Add(newItem);
        await _databaseContext.SaveChangesAsync();

        return Ok(new CreatedItem(newItem.Id));
    }
    
    
    [HttpGet("")]
    public async Task<IActionResult> GetItems()
    {
        return Ok(
            await _databaseContext.Items
                .Include(item => item.Room)
                .Include(item => item.Categories)
                .Include(item => item.Images)
                .Include(item => item.AdditionalFiles)
                .ToListAsync()
        );
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetItem(uint id)
    {
        Item? searchedItem=await _databaseContext.Items
                                        .Include(item => item.Room)
                                        .Include(item => item.Categories)
                                        .Include(item => item.Images)
                                        .Include(item => item.AdditionalFiles)
                                        .FirstOrDefaultAsync(item => item.Id == id);
        
        if (searchedItem == null) return Problem(statusCode: (int) HttpStatusCode.NotFound, title: "Item not found");
        
        return Ok(searchedItem);
    }

    
    [HttpPatch("{id}/name")]
    public async Task<IActionResult> UpdateItemName(uint id, UpdateItemName updateItem)
    {
        Item? searchedItem=await _databaseContext.Items.FindAsync(id);
        if(searchedItem==null) return Problem(statusCode: (int) HttpStatusCode.NotFound, title:"Item not found");

        searchedItem.Name = updateItem.Name;
        await _databaseContext.SaveChangesAsync();

        return NoContent();
    }
    
    
    [HttpPatch("{id}/description")]
    public async Task<IActionResult> UpdateItemDescription(uint id, UpdateItemDescription updateItem)
    {
        Item? searchedItem=await _databaseContext.Items.FindAsync(id);
        if(searchedItem==null) return Problem(statusCode: (int) HttpStatusCode.NotFound, title:"Item not found");

        searchedItem.Description = updateItem.Description;
        await _databaseContext.SaveChangesAsync();

        return NoContent();
    }
    
    
    [HttpPatch("{id}/link")]
    public async Task<IActionResult> UpdateItemLink(uint id, UpdateItemLink updateItem)
    {
        Item? searchedItem=await _databaseContext.Items.FindAsync(id);
        if(searchedItem==null) return Problem(statusCode: (int) HttpStatusCode.NotFound, title:"Item not found");

        searchedItem.Link = updateItem.Link;
        await _databaseContext.SaveChangesAsync();

        return NoContent();
    }
    
    
    [HttpPatch("{id}/room")]
    public async Task<IActionResult> UpdateItemRoom(uint id, UpdateItemRoom updateItem)
    {
        Item? searchedItem=await _databaseContext.Items.FindAsync(id);
        if(searchedItem==null) return Problem(statusCode: (int) HttpStatusCode.NotFound, title:"Item not found");

        Room? searchedRoom=await _databaseContext.Rooms.FindAsync(updateItem.RoomId);
        if(searchedRoom==null) return Problem(statusCode: (int) HttpStatusCode.NotFound, title:"Room not found");

        searchedItem.Room = searchedRoom;
        await _databaseContext.SaveChangesAsync();

        return NoContent();
    }

    
    [HttpPost("{id}/categories")]
    public async Task<IActionResult> AddCategory(uint id, [FromBody] AppendCategory addCategory)
    {
        Item? searchedItem=await _databaseContext.Items.FindAsync(id);
        if(searchedItem==null) return Problem(statusCode: (int) HttpStatusCode.NotFound, title:"Item not found");
        
        Category? searchedCategory=await _databaseContext.Categories.FindAsync(addCategory.CategoryId);
        if(searchedCategory==null) return Problem(statusCode: (int) HttpStatusCode.NotFound, title:"Category not found");
        
        searchedItem.Categories.Add(searchedCategory);
        await _databaseContext.SaveChangesAsync();
        
        return NoContent();
    }
    
    [HttpDelete("{itemId}/categories/{categoryId}")]
    public async Task<IActionResult> DeleteCategory(uint itemId, uint categoryId)
    {
        Item? searchedItem=await _databaseContext.Items.FindAsync(itemId);
        if(searchedItem==null) return Problem(statusCode: (int) HttpStatusCode.NotFound, title:"Item not found");
        
        Category? searchedCategory=await _databaseContext.Categories.FindAsync(categoryId);
        if(searchedCategory==null) return Problem(statusCode: (int) HttpStatusCode.NotFound, title:"Category not found");
        
        searchedItem.Categories.Remove(searchedCategory);
        await _databaseContext.SaveChangesAsync();

        return NoContent();
    }
    

    [HttpPost("{id}/images")]
    public async Task<IActionResult> AddImage(uint id, [FromForm] IFormFile image)
    {
        return await AddDataFileToItem(id, new List<string> {"image/jpeg", "image/png"}, image,
            async () => (Original: await _fileService.SaveJpgFile(image), Preview: await _fileService.SaveReducedJpgFile(image)),
            (searchedItem, writeActionResult) =>
            {
                ImageDataFile newImageDataFile=new ImageDataFile(image.FileName.Replace($".{image.FileName.Split(".").Last()}", ""), writeActionResult.Original.FileName, writeActionResult.Original.Width, writeActionResult.Original.Height, writeActionResult.Preview.FileName, writeActionResult.Preview.Width, writeActionResult.Preview.Height);
                searchedItem.Images.Add(newImageDataFile);
                
                return newImageDataFile;
            }
        );
    }
    
    [HttpPatch("{itemId}/images/{additionalFilesId}/name")]
    public async Task<IActionResult> UpdateImageName(uint itemId, uint additionalFilesId, [FromBody] UpdateFileName updateFileName)
    {
        Item? searchedItem=await _databaseContext.Items.Include(item => item.Images).FirstOrDefaultAsync(item => item.Id == itemId);;
        if (searchedItem == null) return Problem(statusCode: (int) HttpStatusCode.NotFound, title: "Item not found");
        
        DataFile? searchedDataFile = searchedItem.Images.Find(file => file.Id == additionalFilesId);
        if(searchedDataFile==null) return Problem(statusCode: (int) HttpStatusCode.NotFound, title: "Image not found");
        
        searchedDataFile.Name= updateFileName.Name;
        await _databaseContext.SaveChangesAsync();
        
        return NoContent();
    }

    [HttpDelete("{itemId}/images/{imageId}")]
    public async Task<IActionResult> DeleteImage(uint itemId, uint imageId)
    {
        Item? searchedItem=await _databaseContext.Items
            .Include(item => item.Images)
            .FirstOrDefaultAsync(item => item.Id == itemId);
            
        if(searchedItem==null) return Problem(statusCode: (int) HttpStatusCode.NotFound, title:"Item not found");

        ImageDataFile? searchedImageDataFile = searchedItem.Images.Find(image => image.Id == imageId);
        if(searchedImageDataFile==null) return Problem(statusCode: (int) HttpStatusCode.NotFound, title:"Image not found");
        
        _fileService.DeleteFile(searchedImageDataFile.FileName);
        _fileService.DeleteFile(searchedImageDataFile.PreviewFileName);
        _databaseContext.ImageDataFiles.Remove(searchedImageDataFile);
        await _databaseContext.SaveChangesAsync();
        
        return NoContent();
    }

    
    [HttpPost("{id}/additionalFiles")]
    public async Task<IActionResult> AddAdditionalFile(uint id, [FromForm] IFormFile additionalFile) =>
        await AddDataFileToItem(id, new List<string> {"application/pdf", "text/plain", "application/rtf", "application/msword", "text/html"}, additionalFile,
            async () => await _fileService.SaveFile(additionalFile),
            (searchedItem, firstWriteActionResult) =>
            {
                DataFile newDataFile=new DataFile(additionalFile.FileName.Replace($".{additionalFile.FileName.Split(".").Last()}", ""), firstWriteActionResult);
                searchedItem.AdditionalFiles.Add(newDataFile);
                return newDataFile;
            }
        );
    
    [HttpPatch("{itemId}/additionalFiles/{additionalFilesId}/name")]
    public async Task<IActionResult> UpdateAdditionalFileName(uint itemId, uint additionalFilesId, [FromBody] UpdateFileName updateFileName)
    {
        Item? searchedItem=await _databaseContext.Items.Include(item => item.AdditionalFiles).FirstOrDefaultAsync(item => item.Id == itemId);
        if (searchedItem == null) return Problem(statusCode: (int) HttpStatusCode.NotFound, title: "Item not found");
        
        DataFile? searchedDataFile = searchedItem.AdditionalFiles.Find(file => file.Id == additionalFilesId);
        if(searchedDataFile==null) return Problem(statusCode: (int) HttpStatusCode.NotFound, title: "Additional file not found");
        
        searchedDataFile.Name= updateFileName.Name;
        await _databaseContext.SaveChangesAsync();
        
        return NoContent();
    }
    
    [HttpDelete("{itemId}/additionalFiles/{additionalFilesId}")]
    public async Task<IActionResult> DeleteAdditionalFile(uint itemId, uint additionalFilesId)
    {
        Item? searchedItem=await _databaseContext.Items.Include(item => item.AdditionalFiles).FirstOrDefaultAsync(item => item.Id == itemId);
        if(searchedItem==null) return Problem(statusCode: (int) HttpStatusCode.NotFound, title: "Item not found");;

        DataFile? searchedAdditionalFiles = searchedItem.AdditionalFiles.Find(file => file.Id == additionalFilesId);
        if(searchedAdditionalFiles==null) return Problem(statusCode: (int) HttpStatusCode.NotFound, title: "Additional file not found");;
        
        _fileService.DeleteFile(searchedAdditionalFiles.FileName);
        _databaseContext.DataFiles.Remove(searchedAdditionalFiles);
        await _databaseContext.SaveChangesAsync();
        
        return NoContent();
    }
    
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteItem(uint id)
    {
        Item? searchedItem = await _databaseContext.Items
                                        .Include(item => item.Images)
                                        .Include(item => item.AdditionalFiles)
                                        .Where(item => item.Id == id)
                                        .FirstOrDefaultAsync();
        if(searchedItem==null) return Problem(statusCode: (int) HttpStatusCode.NotFound, title: "Item not found");
        
        foreach(ImageDataFile imageDataFile in searchedItem.Images)
        {
            _fileService.DeleteFile(imageDataFile.FileName);
            _fileService.DeleteFile(imageDataFile.PreviewFileName);
        }
        foreach(DataFile dataFile in searchedItem.AdditionalFiles) _fileService.DeleteFile(dataFile.FileName);

        _databaseContext.Items.Remove(searchedItem);
        await _databaseContext.SaveChangesAsync();
        
        return NoContent();
    }
    
    
    
    private async Task<IActionResult> AddDataFileToItem<TWriteActionResult>(uint id, List<string> allowedMimeTypes, IFormFile file, Func<Task<TWriteActionResult>> writeAction, Func<Item, TWriteActionResult, DataFile> storeAction)
    {
        switch (await _fileService.CheckFileForVirus(file))
        {
            case VirusScanResult.Clean: break;
            case VirusScanResult.Infected: return Problem(statusCode: (int) HttpStatusCode.UnsupportedMediaType, title: "File contains virus");
            case VirusScanResult.Error: return Problem(statusCode: (int) HttpStatusCode.InternalServerError, title: "Error while scanning file for virus");
        }
        
        if(file.Length is 0 or > 100*1000000) return Problem(statusCode: (int) HttpStatusCode.RequestEntityTooLarge, title:"File size not allowed");
        
        string mimeType = _fileService.GetFileMimeType(file);
        if(!allowedMimeTypes.Contains(mimeType)) return Problem(statusCode: (int) HttpStatusCode.UnsupportedMediaType, title:"File type not allowed");
        
        Item? searchedItem=await _databaseContext.Items.FindAsync(id);
        if(searchedItem==null) return Problem(statusCode: (int) HttpStatusCode.NotFound, title:"Item not found");
        
        TWriteActionResult writeActionResult = await writeAction();

        DataFile newDataFile=storeAction(searchedItem, writeActionResult);
        
        _databaseContext.Items.Update(searchedItem);
        await _databaseContext.SaveChangesAsync();
        
        return Ok(new CreatedFile(newDataFile.Id));
    }
}