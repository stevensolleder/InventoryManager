using System.ComponentModel.DataAnnotations;
using InventoryManager.Extensions;
using Microsoft.EntityFrameworkCore;

namespace InventoryManager.Options;

public class DatabaseOptions
{
    [MinLength(5)] public string ConnectionString { get; }
    public MariaDbServerVersion MariaDbServerVersion { get; }
    
    public DatabaseOptions(string connectionString, MariaDbServerVersion mariaDbServerVersion)
    {
        ConnectionString = connectionString;
        MariaDbServerVersion = mariaDbServerVersion;

        this.ValidateAllProperties();
    }
}