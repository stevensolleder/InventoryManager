using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using InventoryManager.Extensions;
using InventoryManager.Interfaces;

namespace InventoryManager.Model;

public record class Item:IReadOnlyIdentifiable, INameable
{
    [Key] public uint Id { get; init; }

    private string _name=null!;
    [MinLength(1)] public string Name { get => _name; set => _name = this.ValidateProperty(value); }

    public string Description { get; set; } = "";
    
    public string Link { get; set; } = "";
    
    public Room? Room { get; set; }
    
    public List<Category> Categories { get; } = new ();
    
    public List<ImageDataFile> Images { get; } = new ();
    
    public List<DataFile> AdditionalFiles { get; } = new ();

    public Item(string name) => Name = name;
}