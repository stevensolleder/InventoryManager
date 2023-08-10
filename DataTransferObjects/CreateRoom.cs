using System.ComponentModel.DataAnnotations;
using InventoryManager.Extensions;
using InventoryManager.Interfaces;

namespace InventoryManager.DataTransferObjects;

public record class CreateRoom([MinLength(1)] string Name):IReadOnlyNameable;