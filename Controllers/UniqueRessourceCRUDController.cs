using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using InventoryManager.Interfaces;
using InventoryManager.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryManager.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public abstract class UniqueRessourceCRUDController<TResource, TCreateResource, TUpdateResource>:ControllerBase
    where TResource:class, IReadOnlyIdentifiable, INameable
    where TCreateResource:IReadOnlyNameable
    where TUpdateResource:IReadOnlyNameable
{
    private readonly DatabaseContext _databaseContext;
    private readonly DbSet<TResource> _resources;
    
    private readonly Func<TCreateResource, TResource> _createResource;

    
    protected UniqueRessourceCRUDController(DatabaseContext databaseContext, Func<TCreateResource, TResource> createResource)
    {
        _databaseContext = databaseContext;
        _resources= databaseContext.Set<TResource>();
        _createResource = createResource;
    }

    [HttpPost("")]
    public async Task<IActionResult> CreateRessource(TCreateResource createUpdateResource)
    {
        if (await _resources.AnyAsync(resource => resource.Name == createUpdateResource.Name))
            return Problem(statusCode: (int) HttpStatusCode.Conflict, detail: $"{typeof(TResource).Name} already exists");

        TResource newResource = _createResource(createUpdateResource);

        _resources.Add(newResource);
        await _databaseContext.SaveChangesAsync();

        return StatusCode(201, new Dictionary<string, uint>{{$"{typeof(TResource).Name.ToLower()}Id", newResource.Id}});
    }
    
    [HttpGet("")]
    public async Task<List<TResource>> ReadResource() => await _resources.ToListAsync();
    
    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateResource(uint id, TUpdateResource createUpdateResource)
    {
        if(await _resources.AnyAsync(resource => resource.Name == createUpdateResource.Name)) return Problem(statusCode: (int) HttpStatusCode.Conflict, detail: $"{typeof(TResource).Name} already exists");

        TResource? updateResource = await _resources.FindAsync(id);
        if(updateResource == null) return Problem(statusCode: (int) HttpStatusCode.NotFound, detail: $"{typeof(TResource).Name} not found");
        
        updateResource.Name = createUpdateResource.Name;
        await _databaseContext.SaveChangesAsync();
        
        return NoContent();
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteResource(uint id)
    {
        TResource? deleteResource = await _resources.FindAsync(id);
        if(deleteResource == null) return Problem(statusCode: (int) HttpStatusCode.NotFound, detail: $"{typeof(TResource).Name} not found");
        
        _resources.Remove(deleteResource);
        await _databaseContext.SaveChangesAsync();
        
        return NoContent();
    }
}