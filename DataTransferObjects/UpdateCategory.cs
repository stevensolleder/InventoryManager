using System.ComponentModel.DataAnnotations;
using InventoryManager.Extensions;
using InventoryManager.Interfaces;

namespace InventoryManager.DataTransferObjects;

public record class UpdateCategory([MinLength(1)] string Name):IReadOnlyNameable;