using System.ComponentModel.DataAnnotations;
using hotel_api_.AnotationValidation;

namespace hotel_api_.RequestDto;

public class RoomTypeRequestDto
{
    [Required]
    public Guid  belongTo { get; set; }
    
    [Required]
    [IsImageFile(ErrorMessage = "Only image files are supported image is [png,jpg,jpeg,svg")]
    public IFormFile? file { get; set; }
}