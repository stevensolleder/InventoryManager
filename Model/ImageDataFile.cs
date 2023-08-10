using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using InventoryManager.Extensions;

namespace InventoryManager.Model;

public record class ImageDataFile:DataFile
{
    private uint _width;
    [Range(1, uint.MaxValue)] public uint Width { get=>_width; set=>_width=this.ValidateProperty(value); }
    
    private uint _height;
    [Range(1, uint.MaxValue)] public uint Height { get=>_height; set=>_height=this.ValidateProperty(value); }
    
    
    private string _previewFileName=null!;
    [JsonIgnore, MinLength(1)] public string PreviewFileName { get=>_previewFileName; set=>_previewFileName=this.ValidateProperty(value); }

    public string PreviewName => $"{_name}.preview.{FileName.Split('.').Last()}";
    
    private uint _previewWidth;
    [Range(1, uint.MaxValue)] public uint PreviewWidth { get=>_previewWidth; set=>_previewWidth=this.ValidateProperty(value); }
    
    private uint _previewHeight;
    [Range(1, uint.MaxValue)] public uint PreviewHeight { get=>_previewHeight; set=>_previewHeight=this.ValidateProperty(value); }
    
    public ImageDataFile(string name, string fileName, uint width, uint height, string previewFileName, uint previewWidth, uint previewHeight):base(name, fileName)
    {
        Width = width;
        Height = height;
        
        PreviewFileName = previewFileName;
        PreviewWidth = previewWidth;
        PreviewHeight = previewHeight;
    }
}