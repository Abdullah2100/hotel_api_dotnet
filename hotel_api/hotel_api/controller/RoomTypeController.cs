using hotel_api_.RequestDto;
using hotel_api.Services;
using hotel_api.util;
using hotel_business;
using hotel_data.dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace hotel_api.controller;

[ApiController]
[Route("api/roomType")]
public class RoomTypeController: ControllerBase
{
    public RoomTypeController(IConfigurationServices config)
    {
        _config = config;
    }

    private readonly IConfigurationServices _config;
    
       [Authorize]
        [HttpPost("roomtype")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> createNewRoomType(
            [FromForm] RoomTypeRequestUpdateDto roomTypeData
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

            // var isHasPermissionToCreateRoomType = AdminBuissnes.isAdminExist(adminid ?? Guid.Empty);


            // if (!isHasPermissionToCreateRoomType)
            // {
            //     return StatusCode(401, "you not have Permission");
            // }


            if (roomTypeData.name.Length > 50)
                return StatusCode(400, "roomtype name must be under 50 characters");


            bool isExistName = RoomtTypeBuissnes.isExist(roomTypeData.name);

            if (isExistName)
                return StatusCode(400, "roomtype is already exist");


            var roomtypeid = Guid.NewGuid();


            string? imageHolderPath = null;
            if (roomTypeData.image != null)
            {
                imageHolderPath = await MinIoServices.uploadFile(_config, roomTypeData.image,
                    MinIoServices.enBucketName.RoomType);
            }

            clsUtil.saveImage(imageHolderPath, roomtypeid);

            var roomTypeHolder = new RoomtTypeBuissnes(
                new RoomTypeDto(
                    roomTypeId: roomtypeid,
                    roomTypeName: roomTypeData.name,
                    createdBy: (Guid)adminid,
                    createdAt: DateTime.Now
                )
            );

            var result = roomTypeHolder.save();

            if (result == false)
                return StatusCode(500, "some thing wrong");

            return StatusCode(201, new { message = "created seccessfully" });
        }


        [Authorize]
        [HttpGet("roomtype{isNotDeletion:bool}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult getRoomTypes(bool? isNotDeletion)
        {
            try
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

                // var isHasPermission = AdminBuissnes.isAdminExist(adminid ?? Guid.Empty);
                //
                //
                // if (!isHasPermission)
                // {
                //     return StatusCode(401, "you not have Permission");
                // }

                var roomtypes = RoomtTypeBuissnes.getRoomTypes(isNotDeletion ?? false);

                return Ok(roomtypes);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(500, "some thing wrong");
            }
        }


        [Authorize]
        [HttpPut("roomtype/{roomtypeid:guid}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> updateRoomTypes([FromForm] RoomTypeRequestUpdateDto roomTypeData,
            Guid roomtypeid)
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

            // var isHasPermissionToCreateUser = AdminBuissnes.isAdminExist(adminid ?? Guid.Empty);
            //
            //
            // if (!isHasPermissionToCreateUser)
            // {
            //     return StatusCode(401, "you not have Permission");
            // }


            if (roomTypeData.name.Length > 50 || roomtypeid == null)
                return StatusCode(400, "roomtype name must be under 50 characters");


            var roomtypeHolder = RoomtTypeBuissnes.getRoomType(roomtypeid);

            if (roomtypeHolder == null)
                return StatusCode(400, "roomtype is already exist");

            var imageHolder = ImageBuissness.getImageByBelongTo((Guid)roomtypeid);

            string? imageHolderPath = null;
            if (roomTypeData.image != null)
            {
                imageHolderPath = await MinIoServices.uploadFile(_config, roomTypeData.image,
                    MinIoServices.enBucketName.RoomType, imageHolder.path);
            }


            clsUtil.saveImage(imageHolderPath, roomtypeid, imageHolder);

            updateRoomTypeData(ref roomtypeHolder, roomTypeData, (Guid)adminid);
            var result = roomtypeHolder.save();

            if (result == false)
                return StatusCode(500, "some thing wrong");

            return StatusCode(201, new { message = "created seccessfully" });
        }


        private void updateRoomTypeData(ref RoomtTypeBuissnes data, RoomTypeRequestUpdateDto holder, Guid createdBy)
        {
            if (data.name != holder.name)
            {
                data.name = holder.name;
            }

            if (createdBy != null && data.createdBy != createdBy)
            {
                data.createdBy = createdBy;
            }
        }


        [Authorize]
        [HttpDelete("roomtype/{roomtypeid:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult deleteOrUnDeleteRoomtype(
            Guid roomtypeid
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

            // var isHasPermissionToCreateUser = AdminBuissnes.isAdminExist(adminid ?? Guid.Empty);
            //
            //
            // if (!isHasPermissionToCreateUser)
            // {
            //     return StatusCode(401, "you not have Permission");
            // }


            var data = RoomtTypeBuissnes.getRoomType(roomtypeid);

            if (data == null)
                return StatusCode(409, "user notFound exist");


            var result = RoomtTypeBuissnes.deleteOrUnDeleteRoomType(roomtypeid);
            if (result == false)
                return StatusCode(500, "some thing wrong");

            return StatusCode(201, new { message = "created seccessfully" });
        }
 
}