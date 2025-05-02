using hotel_api_.RequestDto;
using hotel_api.Services;
using hotel_api.util;
using hotel_business;
using hotel_data.dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace hotel_api.controller;

[ApiController]
[Route("api/room")]
public class RoomController : ControllerBase
{
    public RoomController(IConfigurationServices config)
    {
        _config = config;
    }

    private readonly IConfigurationServices _config;


    [HttpPost("")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> createRoom
        ([FromForm] RoomRequestDto roomData)
    {
        var authorizationHeader = HttpContext.Request.Headers["Authorization"];
        var id = AuthinticationServices.GetPayloadFromToken("id",
            authorizationHeader.ToString().Replace("Bearer ", ""));
        Guid? userId = null;
        if (Guid.TryParse(id.Value, out Guid outID))
        {
            userId = outID;
        }

        if (userId == null)
        {
            return StatusCode(401, "you not have Permission");
        }

        if (roomData.roomtypeid == null)
        {
            return StatusCode(400, "لا بد من تحديد نوع الغرفة");
        }

        var roomId = Guid.NewGuid();

        List<ImageRequestDto>? imageHolderPath = null;
        if (roomData.images != null)
        {
            imageHolderPath = await MinIoServices.uploadFile(
                _config,
                roomData.images,
                MinIoServices.enBucketName.ROOM,
                roomId.ToString()
            );
        }

        clsUtil.saveImage(imageHolderPath, roomId);

        var roomHolder = new RoomBuisness(
            new RoomDto(
                roomId: roomId,
                status: null,
                pricePerNight: roomData.pricePerNight,
                roomtypeid: (Guid)roomData.roomtypeid,
                capacity: roomData.capacity,
                bedNumber: roomData.bedNumber,
                beglongTo: (Guid)userId,
                createdAt: DateTime.Now,
                location: roomData.location,
                longitude: roomData.longitude,
                latitude: roomData.latitude
            )
        );

        var result = roomHolder.save();

        if (result == false)
            return StatusCode(500, "some thing wrong");

        var roomNewInserted = roomHolder.getRoom();

        return StatusCode( 201,roomNewInserted.roomHolder );
    }


    [HttpGet("me/{pageNumber:int}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> getMyRooms
        (int pageNumber)
    {
        try
        {
            var authorizationHeader = HttpContext.Request.Headers["Authorization"];
            var id = AuthinticationServices.GetPayloadFromToken("id",
                authorizationHeader.ToString().Replace("Bearer ", ""));
            Guid? userID = null;
            if (Guid.TryParse(id.Value.ToString(), out Guid outID))
            {
                userID = outID;
            }

            if (userID == null)
            {
                return StatusCode(401, "you not have Permission");
            }

            var rooms = RoomBuisness.getAllRooms(
                pageNumber,
                25,
                userId: userID
            );
            return Ok(rooms);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Something went wrong");
        }
    }

    
    
    [HttpGet("{pageNumber:int}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult>getRooms 
        (int pageNumber)
    {
        try
        {
            var authorizationHeader = HttpContext.Request.Headers["Authorization"];
            var id = AuthinticationServices.GetPayloadFromToken("id",
                authorizationHeader.ToString().Replace("Bearer ", ""));
            Guid? userID = null;
            if (Guid.TryParse(id.Value.ToString(), out Guid outID))
            {
                userID = outID;
            }

            if (userID == null)
            {
                return StatusCode(401, "you not have Permission");
            }

            var rooms = RoomBuisness.getAllRooms(
                pageNumber,
                25
            );
            return Ok(rooms);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Something went wrong");
        }
    }


    [HttpPut()]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> updateRoom
    ([FromForm] RoomRequestUpdateDto roomData
    )
    {
        var authorizationHeader = HttpContext.Request.Headers["Authorization"];
        var id = AuthinticationServices.GetPayloadFromToken("id",
            authorizationHeader.ToString().Replace("Bearer ", ""));
        Guid? adminid = null;
        if (Guid.TryParse(id.Value.ToString(), out Guid outID))
        {
            adminid = outID;
        }

        if (adminid == null)
        {
            return StatusCode(401, "you not have Permission");
        }

        
      
            var room = RoomBuisness.getRoom(roomData.id);

        if (room == null)
            return StatusCode(400, "الغرفة غير موجودة");
        
        if(room.beglongTo!=adminid)
            return StatusCode(400, "صاحب الغرفة فقط من يمكنه تعديل البيانات");

        
        if (room.isDeleted==true||room.isBlocked==true)
            return StatusCode(400, "لا يمكن تعديل بيانات الغرفة لا بد من التاكد من وجود هذه الغرفة او تغير حالتها");


        List<ImageRequestDto>? imageHolderPath = null;
        if (roomData.images != null)
        {
            imageHolderPath = await MinIoServices.uploadFile(
                _config,
                roomData.images,
                MinIoServices.enBucketName.ROOM,
                roomData.id.ToString()
            );
        }

        if (imageHolderPath != null)
            clsUtil.saveImage(imageHolderPath, roomData.id);
        _updateRoomData(ref room, roomData);

        var result = room.save();

        if (result == false)
            return StatusCode(500, "some thing wrong");

        var newRoomUpdateData = room.getRoom();
        return StatusCode(200, newRoomUpdateData.roomHolder);
    }
    
    private void _updateRoomData(ref RoomBuisness roomData, RoomRequestUpdateDto newRoomData)
    {
        if (newRoomData.status != null && newRoomData.status.Trim().Length >0 &&roomData.status != newRoomData.status)
        {
            roomData.status = newRoomData.status;
        }
        else
        {
            roomData.status = "";
        }

        if (newRoomData.pricePerNight != null && newRoomData.pricePerNight!=0 &&newRoomData.pricePerNight != roomData.pricePerNight)
        {
            roomData.pricePerNight = (int)newRoomData.pricePerNight;
        }
        else roomData.pricePerNight = 0;

        if (newRoomData.bedNumber != null &&newRoomData.bedNumber != 0 && newRoomData.bedNumber != roomData.bedNumber)
        {
            roomData.bedNumber = (int)newRoomData.bedNumber;
        }else roomData.bedNumber = 0;

        if (newRoomData.roomtypeid != null && newRoomData.roomtypeid != roomData.roomtypeid)
        {
            roomData.roomtypeid = (Guid)newRoomData.roomtypeid;
        }
        else roomData.roomtypeid = Guid.Empty;

        if (newRoomData.capacity != null && newRoomData.capacity != 0 &&newRoomData.capacity != roomData.capacity)
        {
            roomData.capacity = (int)newRoomData.capacity;
        }
        else roomData.capacity = 0;

        if (newRoomData.longitude != null && newRoomData.longitude!=0 &&  newRoomData.longitude != roomData.longitude)
        {
            roomData.longitude = newRoomData.longitude;
        }

        if (newRoomData.latitude != null&& newRoomData.latitude!=0 && roomData.latitude != newRoomData.latitude)
        {
            roomData.latitude = newRoomData.latitude;
        }
        
        if (newRoomData.location!= null&&newRoomData.location.Trim().Length>0 && roomData.location != roomData.location)
        {
            roomData.location = newRoomData.location;
        }
        else roomData.location = null;
    }


    [HttpDelete("{roomId:guid}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> deleteOrUnDeleteRoom
    ([FromForm] RoomRequestUpdateDto roomData,
        Guid roomId
    )
    {
        var authorizationHeader = HttpContext.Request.Headers["Authorization"];
        var id = AuthinticationServices.GetPayloadFromToken("id",
            authorizationHeader.ToString().Replace("Bearer ", ""));
        Guid? userID = null;
        if (Guid.TryParse(id.Value.ToString(), out Guid outID))
        {
            userID = outID;
        }

        if (userID == null)
        {
            return StatusCode(401, "مشكلة في التحقق");
        }

        var user = UserBuissnes.getUserByID((Guid)userID!);
        
        if(user==null)
            return StatusCode(401, "المتستخدم غير موجود");


        var room = RoomBuisness.getRoom(roomId,userID);

        if (room == null)
            return StatusCode(400, "الغرفة غير موجودة");

        if(room.beglongTo!=user.ID&&user.isUser==true)
        return StatusCode(400 ,"ليس لديك الصلاحيات");

        
        var result = RoomBuisness.deleteRoom((Guid)room.ID, (Guid)userID);

        if (result == false)
            return StatusCode(500, "هناك مشكلة ما");
        return StatusCode(200, new { message = "تم الحذف بنجاح" });
    }
}