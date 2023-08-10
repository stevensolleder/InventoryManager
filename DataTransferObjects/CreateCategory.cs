using System;
using System.ComponentModel.DataAnnotations;
using InventoryManager.Extensions;
using InventoryManager.Interfaces;

namespace InventoryManager.DataTransferObjects;

public record class CreateCategory([MinLength(1)] string Name):IReadOnlyNameable;