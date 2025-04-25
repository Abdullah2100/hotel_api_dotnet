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

        return StatusCode(201, new { message = "created seccessfully" });
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


    [HttpPut("{roomId:guid}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> updateRoom
    ([FromForm] RoomRequestUpdateDto roomData,
        Guid roomId
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

        // var isHasPermissionToCurd = AdminBuissnes.isAdminExist(adminid ?? Guid.Empty);


        // if (!isHasPermissionToCurd)
        // {
        //     return StatusCode(401, "you not have Permission");
        // }

        var room = RoomBuisness.getRoom(roomId);

        if (room == null)
            return StatusCode(400, "room not found");


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

        if (imageHolderPath != null)
            clsUtil.saveImage(imageHolderPath, roomId);
        _updateRoomData(ref room, roomData);

        var result = room.save();

        if (result == false)
            return StatusCode(500, "some thing wrong");

        return StatusCode(200, new { message = "update seccessfully" });
    }

    private void _updateRoomData(ref RoomBuisness roomData, RoomRequestUpdateDto newRoomData)
    {
        if (newRoomData.status != null && roomData.status != newRoomData.status)
        {
            roomData.status = newRoomData.status;
        }

        if (newRoomData.pricePerNight != null && newRoomData.pricePerNight != roomData.pricePerNight)
        {
            roomData.pricePerNight = (int)newRoomData.pricePerNight;
        }

        if (newRoomData.bedNumber != null && newRoomData.bedNumber != roomData.bedNumber)
        {
            roomData.bedNumber = (int)newRoomData.bedNumber;
        }

        if (newRoomData.roomtypeid != null && newRoomData.roomtypeid != roomData.roomtypeid)
        {
            roomData.roomtypeid = (Guid)newRoomData.roomtypeid;
        }

        if (newRoomData.capacity != null && newRoomData.capacity != roomData.capacity)
        {
            roomData.capacity = (int)newRoomData.capacity;
        }
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
        Guid? adminid = null;
        if (Guid.TryParse(id.Value.ToString(), out Guid outID))
        {
            adminid = outID;
        }

        if (adminid == null)
        {
            return StatusCode(401, "you not have Permission");
        }

        // var isHasPermissionToCurd = AdminBuissnes.isAdminExist(adminid ?? Guid.Empty);


        // if (!isHasPermissionToCurd)
        // {
        //     return StatusCode(401, "you not have Permission");
        // }

        var room = RoomBuisness.getRoom(roomId);

        if (room == null)
            return StatusCode(400, "room not found");


        var result = RoomBuisness.deleteRoom(room.ID, (Guid)adminid);

        if (result == false)
            return StatusCode(500, "some thing wrong");
        return StatusCode(200, new { message = "deleted seccessfully" });
    }
}