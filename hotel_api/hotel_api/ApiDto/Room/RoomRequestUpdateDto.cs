using System.ComponentModel.DataAnnotations;
using hotel_data.dto;

namespace hotel_api_.RequestDto;

public class RoomRequestUpdateDto
{
    [Required]public Guid id {get; set;} 
    
    public string? status {get; set;} 
    
    public decimal? pricePerNight {get; set;}
    
    public int? capacity {get; set;}
    
    public Guid? roomtypeid {get; set;}
    
    public int? bedNumber { get; set; }

    public string? location { get; set; }
    public decimal?  latitude { get; set; }
    public decimal? longitude { get; set; }
    public List<ImageRequestDto>? images { get; set; } 
}