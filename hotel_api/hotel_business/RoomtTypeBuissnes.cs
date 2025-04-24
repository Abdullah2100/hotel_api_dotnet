using hotel_data;
using hotel_data.dto;

namespace hotel_business;

public class RoomtTypeBuissnes
{
   public enMode mode { get; set; }  
    public Guid? ID { get; set; }
    public string name { get; set; }
    public string? image { get; set; }
    public Guid createdBy { get; set; }
    public DateTime createdAt { get; set; }

    public RoomTypeDto roomType
    {
        get
        {
            return new RoomTypeDto(
                roomTypeId: ID, 
                roomTypeName: name,
                createdAt: createdAt,
                createdBy: createdBy,
                imagePath: null
                );
        }
    }

    public RoomtTypeBuissnes(RoomTypeDto roomType,enMode mode =enMode.add)
    {
        this.ID = roomType.roomTypeID;
        this.name = roomType.roomTypeName;
        this.createdBy = roomType.createdBy;
        this.createdAt = roomType.createdAt;
        this.image = roomType.imagePath??"";
        this.mode = mode;
    }

    public static RoomtTypeBuissnes? getRoomType(string name)
    {
        var roomType = RoomTypeData.getRoomType(name);
        return roomType!=null ? new RoomtTypeBuissnes(roomType,enMode.update) : null;
    }

    public static RoomtTypeBuissnes? getRoomType(Guid id)
    {
        var roomType = RoomTypeData.getRoomType(id);
        return roomType!=null ? new RoomtTypeBuissnes(roomType,enMode.update) : null;
    }
    private bool _createRoomType()
    {
        return RoomTypeData.createRoomType(roomType);
    }

    private bool _updateRoomType()
    {
        return RoomTypeData.updateRoomType(roomType);
    }
    
    public bool save()
    {
        switch (mode)
        {
            case enMode.add:
            {
                if (_createRoomType())
                {
                    return true;
                }
                return false;
            }
            case enMode.update:
            {
                if (_updateRoomType())
                {
                    return true;
                }
                return false;
            }
            default: return false;
        }
    }

    public static bool isExist(Guid id)
    {
        return RoomTypeData.isExist(id);
    }
    public static bool isExist(string  name)
    {
        return RoomTypeData.isExist(name);
    }

    public static List<RoomTypeDto> getRoomTypes(bool isNotDeletion)
    {
        return RoomTypeData.getAll(isNotDeletion);
    }


    public static bool deleteOrUnDeleteRoomType(Guid id)
    {
        return      RoomTypeData.deleteOrUnDelete(id);
    }
}