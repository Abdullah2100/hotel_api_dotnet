using hotel_data.dto;

namespace hotel_api_.RequestDto;

public class RoomRequestUpdateDto
{
    public string? status {get; set;} 
    
    public decimal? pricePerNight {get; set;}
    
    public int? capacity {get; set;}
    
    public Guid? roomtypeid {get; set;}
    
    public int? bedNumber { get; set; }

    public List<ImageRequestDto>? images { get; set; } 
}