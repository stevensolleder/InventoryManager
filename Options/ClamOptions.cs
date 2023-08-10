using System.ComponentModel.DataAnnotations;
using InventoryManager.Extensions;

namespace InventoryManager.Options;

public class ClamOptions
{
    [MinLength(2)]  public string Address { get; }
    [Range(1000, 65535)] public uint Port { get; }
    
    public ClamOptions(string address, uint port)
    {
        Address = address;
        Port = port;
        
        this.ValidateAllProperties();
    }
}