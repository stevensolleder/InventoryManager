using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using InventoryManager_Backend;
using InventoryManager.Extensions;
using InventoryManager.Interfaces;

namespace InventoryManager.Model;

public record class DataFile:IReadOnlyIdentifiable, IReadOnlyNameable
{
    [Key] public uint Id { get; init; }
    
    private string _fileName=null!;
    [JsonIgnore, MinLength(1)] public string FileName { get=>_fileName; set=>_fileName=this.ValidateProperty(value); }

    protected string _name=null!;
    [MinLength(1)]public string Name
    {
        get => $"{_name}.{FileName.Split(".").Last()}";
        set => _name = this.ValidateProperty(value);
    } 

    public string MimeType => MimeTypes.GetMimeType(FileName);


    public DataFile(string name, string fileName)
    {
        Name = name;
        FileName = fileName;
    }
}