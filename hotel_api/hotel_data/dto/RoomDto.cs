namespace hotel_data.dto;

public class RoomDto
{
    public RoomDto(
        Guid roomId,
        string? status,
        decimal pricePerNight,
        int capacity,
        Guid roomtypeid,
        int bedNumber,
        Guid beglongTo,
        DateTime createdAt,
        bool isBlock = false,
        bool isDeleted = false,
        string? location = null,
        double? latitude = null,
        double? longitude = null,
        List<ImagesTbDto>? images = null
    )
    {
        this.roomId = roomId;
        this.status = status;
        this.pricePerNight = pricePerNight;
        this.capacity = capacity;
        this.roomtypeid = roomtypeid;
        this.bedNumber = bedNumber;
        this.beglongTo = beglongTo;
        this.createdAt = createdAt;
        this.isBlock = isBlock;
        this.isDeleted = isDeleted;
        this.images = images;
        this.location = location;
        this.latitude = latitude;
        this.longitude = longitude;
        var userData = UserData.getUser(beglongTo);
        
        this.user = userData;

        this.roomTypeData = RoomTypeData.getRoomType(roomtypeid);
        this.images = images;
    }

    public Guid roomId { get; set; }
    public string? status { get; set; } = "Available";
    public decimal pricePerNight { get; set; }
    public int capacity { get; set; }
    public Guid roomtypeid { get; set; }
    public DateTime createdAt { get; set; } = DateTime.UtcNow;
    public int bedNumber { get; set; }
    public Guid beglongTo { get; set; }

    public bool isBlock { get; set; } = false;
    public bool isDeleted { get; set; } = false;

    public UserDto? user { get; set; } = null;
    public RoomTypeDto? roomTypeData { get; set; } = null;

    public List<ImagesTbDto>? images { get; set; }
    public string? location { get; set; }
    public double? latitude { get; set; }
    public double? longitude { get; set; }
}