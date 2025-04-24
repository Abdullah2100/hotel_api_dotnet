namespace hotel_data.dto;

public class RoomTypeDto
{
    public RoomTypeDto(
        Guid? roomTypeId,
        string roomTypeName,
        Guid createdBy,
        DateTime createdAt,
        bool isDeleted = false,
        string? imagePath = null
        )
    {
        roomTypeID = roomTypeId;
        this.roomTypeName = roomTypeName;
        this.createdBy = createdBy;
        this.createdAt = createdAt;
        this.imagePath = imagePath;
        this.isDeleted = isDeleted;
    }

    public Guid? roomTypeID { get; set; }
    public string roomTypeName { get; set; }
    public string? imagePath { get; set; }
    public Guid createdBy { get; set; }
    public DateTime createdAt { get; set; }
    public bool isDeleted { get; set; }
}