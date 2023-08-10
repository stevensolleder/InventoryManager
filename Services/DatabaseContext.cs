using InventoryManager.Model;
using Microsoft.EntityFrameworkCore;

namespace InventoryManager.Services;

public class DatabaseContext:DbContext
{
    //public DbSet<ApplicationUser> ApplicationUsers { get; private set; } = null!;
    //public DbSet<StandardRole> StandardRoles { get; private set; } = null!;
    public DbSet<Item> Items { get; private set; } = null!;
    public DbSet<DataFile> DataFiles { get; private set; } = null!;
    public DbSet<ImageDataFile> ImageDataFiles { get; private set; } = null!;
    public DbSet<Room> Rooms { get; private set; } = null!;
    public DbSet<Category> Categories { get; private set; } = null!;
    
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options){}
}