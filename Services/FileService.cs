using System;
using System.IO;
using System.Threading.Tasks;
using HeyRed.Mime;
using InventoryManager.Interfaces;
using InventoryManager.Model;
using Microsoft.AspNetCore.Http;
using nClam;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using InventoryManager.Options;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp.Processing;

namespace InventoryManager.Services;

public class FileService : IFileService
{
    private readonly DataFilesOptions _fileOptions;
    private readonly ClamOptions _clamOptions;
    
    public FileService(DataFilesOptions dataFileOptions, ClamOptions clamOptions)
    {
        _fileOptions = dataFileOptions;
        _clamOptions = clamOptions;
    }
    
    
    public async Task<string> SaveFile(IFormFile formFile)
    {
        return await StoreRandomNamedNonExecutableFile(
            async (targetFileStream, fileName) =>
            {
                await formFile.CopyToAsync(targetFileStream);
                return fileName;
            }, 
            MimeGuesser.GuessExtension(formFile.OpenReadStream())
        );
    }
    
    public async Task<ImageStoringResult> SaveJpgFile(IFormFile formFile)
    {
        return await StoreRandomNamedNonExecutableFile(
            async (targetFileStream, fileName) =>
            {
                using Image image = await Image.LoadAsync(formFile.OpenReadStream());
                
                await image.SaveAsync(targetFileStream, new JpegEncoder());

                return new ImageStoringResult(fileName, Convert.ToUInt32(image.Width), Convert.ToUInt32(image.Height));
            },
            "jpg"
        );
    }

    public async Task<ImageStoringResult> SaveReducedJpgFile(IFormFile imageFormFile)
    {
        return await StoreRandomNamedNonExecutableFile(
            async (targetFileStream, fileName) =>
            {
                using Image originalImage = await Image.LoadAsync(imageFormFile.OpenReadStream());

                int previewImageWidth = originalImage.Width / 6;
                int previewImageHeight = originalImage.Height / 6;
                originalImage.Mutate(mutatingImage => mutatingImage.Resize(new Size(previewImageWidth, previewImageHeight)));
                
                await originalImage.SaveAsync(targetFileStream, new JpegEncoder());

                return new ImageStoringResult(fileName, Convert.ToUInt32(previewImageWidth), Convert.ToUInt32(previewImageHeight));
            },
            "jpg"
        );
    }
    
    public async Task<byte[]?> GetFile(string fileName)
    {
        string filePath=Path.Combine(_fileOptions.Path, fileName);
        return File.Exists(filePath) ? await File.ReadAllBytesAsync(filePath) : null;
    }
    
    public void DeleteFile(string fileName)
    {
        string filePath = Path.Combine(_fileOptions.Path, fileName);
        if (File.Exists(filePath)) File.Delete(filePath);
    }
    
    public string GetFileMimeType(IFormFile formFile) => MimeGuesser.GuessMimeType(formFile.OpenReadStream());
    
    public async Task<VirusScanResult> CheckFileForVirus(IFormFile formFile)
    {
        ClamClient clam = new ClamClient(_clamOptions.Address, Convert.ToInt32(_clamOptions.Port));  
        ClamScanResult scanResult = await clam.SendAndScanFileAsync(formFile.OpenReadStream());
        
        return scanResult.Result switch
        {
            ClamScanResults.Clean => VirusScanResult.Clean,
            ClamScanResults.VirusDetected => VirusScanResult.Infected,
            ClamScanResults.Unknown => VirusScanResult.Unknown,
            _ => VirusScanResult.Error
        };
    }
    

    private async Task<TStoreResult> StoreRandomNamedNonExecutableFile<TStoreResult>(Func<FileStream, string, Task<TStoreResult>> fileWriteAction, string fileEnding)
    {
        string newFileName = $"{Guid.NewGuid().ToString().Replace("-", String.Empty)}.{fileEnding}";
        string newFilePath = Path.Combine(_fileOptions.Path, newFileName);

        using FileStream newFileStream = File.Create(newFilePath);

        File.SetUnixFileMode(newFilePath, UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.GroupRead | UnixFileMode.OtherRead);

        return await fileWriteAction(newFileStream, newFileName);
    }
}