using hotel_data;
using hotel_data.dto;

namespace hotel_business;

public class RoomBuisness
{

    enMode mode = enMode.add;

    public Guid ID { get; set; }
    public string status { get; set; } = "Available";
    public decimal pricePerNight { get; set; }
    public int capacity { get; set; }
    public Guid roomtypeid { get; set; }
    public DateTime createdAt { get; set; } = DateTime.UtcNow;
    public int bedNumber { get; set; }
    public Guid beglongTo { get; set; }
    public string? location  {get; set; }
    public double? latitude { get; set; }
    public double? longitude { get; set; }

    public RoomDto roomHolder
    {
        get
        {
            return new RoomDto(
                roomId: this.ID,
                status: this.status,
                pricePerNight: this.pricePerNight,
                capacity: this.capacity,
                bedNumber: this.bedNumber,
                roomtypeid: this.roomtypeid,
                createdAt: this.createdAt,
                beglongTo: this.beglongTo,
                longitude: this.longitude,
                location:this.location,
                latitude: this.latitude
            );
        }
    }

    public RoomBuisness(
        RoomDto roomData,
        enMode mode = enMode.add
    )
    {
        this.ID = roomData.roomId;
        this.status = roomData.status;
        this.pricePerNight = roomData.pricePerNight;
        this.capacity = roomData.capacity;
        this.roomtypeid = roomData.roomtypeid;
        this.bedNumber = roomData.capacity;
        this.beglongTo = roomData.beglongTo;
        this.createdAt = roomData.createdAt;
        this.mode = mode;
    }

    private bool _create()
    {
        bool isCreated = RoomData.createRoom(roomHolder);
        return isCreated;
    }

    private bool _update()
    {
        bool isUpdate = RoomData.updateRoom(roomHolder);
        return isUpdate;
    }

    public bool save()
    {
        switch (mode)
        {
            case enMode.add:
            {
                if (_create()) return true;
                return false;
            }
            case enMode.update:
            {
                if (_update()) return true;
                return false;
            }
            default: return false;
        }
    }


    public static List<RoomDto> getAllRooms(
        int pagenumber,
        int limitPerPage,
        Guid? userId=null
        )
    {
        return RoomData.getRoomByPage(
            pagenumber, 
            limitPerPage,
            userId
            );
    }

    public static RoomBuisness? getRoom(Guid roomId)
    {
        var result = RoomData.getRoom(roomId);
        if (result != null) return new RoomBuisness(result, enMode.update);
        return null;
    }

    public static bool deleteRoom(Guid roomId, Guid userId)
    {
        return RoomData.deleteRoom(roomId, userId);
    }
}