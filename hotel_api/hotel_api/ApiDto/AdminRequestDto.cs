using System.ComponentModel.DataAnnotations;

namespace hotel_api_.RequestDto;

public class AdminRequestDto
{
    [Required]
    public string userName { get; set; } = "";

    [Required]
    [MaxLength(16)]
    [MinLength(8)]
    public string? password { get; set; } = "";

    [Required]
    [MaxLength(50)]
    public string name { get; set; } = "";

    [Required] public string email { get; set; } = "";

    [Required]
    [MaxLength(10)]
    [MinLength(10)]
    public string phone { get; set; } = "";

    [Required] public string address { get; set; } = "";
}