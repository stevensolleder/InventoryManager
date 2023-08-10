using System.ComponentModel.DataAnnotations;
using InventoryManager.Extensions;
using InventoryManager.Interfaces;

namespace InventoryManager.Model;

public record class Room:IReadOnlyIdentifiable, INameable
{
    [Key] public uint Id { get; init; }
    
    private string _name=null!;
    [MinLength(1)] public string Name { get=>_name; set=>_name=this.ValidateProperty(value); }
    
    public Room(string name) => Name = name;
}