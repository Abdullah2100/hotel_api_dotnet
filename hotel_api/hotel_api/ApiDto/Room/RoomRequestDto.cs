using hotel_data.dto;

namespace hotel_api_.RequestDto;

public class RoomRequestDto
{
    
    public decimal pricePerNight {get; set;}
    
    public int capacity {get; set;}

    public Guid? roomtypeid { get; set; } = null;
    
    public int bedNumber { get; set; }

    public List<ImageRequestDto>? images { get; set; }
    public string? location { get; set; }
    public decimal?  latitude { get; set; }
    public decimal? longitude { get; set; }
}