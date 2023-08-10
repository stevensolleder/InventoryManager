using InventoryManager.DataTransferObjects;
using InventoryManager.Model;
using InventoryManager.Services;

namespace InventoryManager.Controllers;

public class CategoriesController : UniqueRessourceCRUDController<Category, CreateCategory, UpdateCategory>
{
    public CategoriesController(DatabaseContext databaseContext) : base(databaseContext, createUpdateCategory => new Category(createUpdateCategory.Name)){}
}