namespace hotel_api_.RequestDto;
using Microsoft.AspNetCore.Http;
public class ImageRequestDto
{
    public Guid? id { get; set; }
    
    public Guid? belongTo { get; set; }
    public bool? isDeleted { get; set; } = false;
    
    public bool? isThumnail { get; set; } = false;

    public IFormFile? data { get; set; } 
    public string? fileName { get; set; } = null;
}