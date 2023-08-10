using System.ComponentModel.DataAnnotations;
using InventoryManager.Interfaces;

namespace InventoryManager.DataTransferObjects;


public record class UpdateFileName([MinLength(1)] string Name):IReadOnlyNameable;