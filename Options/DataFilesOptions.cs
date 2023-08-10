using System.ComponentModel.DataAnnotations;
using InventoryManager.Extensions;

namespace InventoryManager.Options;

public class DataFilesOptions
{
    [MinLength(1)] public string Path { get; }
    
    public DataFilesOptions(string path)
    {
        Path = path;

        this.ValidateAllProperties();
    }
}