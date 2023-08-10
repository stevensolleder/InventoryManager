using System.Threading.Tasks;
using InventoryManager.Model;
using Microsoft.AspNetCore.Http;

namespace InventoryManager.Interfaces;

public interface IFileService
{
    Task<string> SaveFile(IFormFile formFile);
    Task<ImageStoringResult> SaveJpgFile(IFormFile formFile);
    Task<ImageStoringResult> SaveReducedJpgFile(IFormFile imageFormFile);
    Task<byte[]?> GetFile(string fileName);
    void DeleteFile(string fileName);
    string GetFileMimeType(IFormFile formFile);
    Task<VirusScanResult> CheckFileForVirus(IFormFile formFile);
}