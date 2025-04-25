using System.ComponentModel.DataAnnotations;

namespace hotel_api_.RequestDto;

public class RoomTypeRequestUpdateDto
{
    [Required]public Guid Id { get; set; }
     [StringLength(50)] public string name { get; set; } = "";
    public IFormFile? image { get; set; } = null;
}