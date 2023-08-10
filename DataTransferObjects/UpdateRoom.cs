using System.ComponentModel.DataAnnotations;
using InventoryManager.Interfaces;

namespace InventoryManager.DataTransferObjects;

public record class UpdateRoom([MinLength(1)] string Name):IReadOnlyNameable;