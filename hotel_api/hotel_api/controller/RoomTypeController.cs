using hotel_api_.RequestDto;
using hotel_api.Services;
using hotel_api.util;
using hotel_business;
using hotel_data.dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace hotel_api.controller;

[Authorize]
[ApiController]
[Route("api/roomType")]
public class RoomTypeController: ControllerBase
{
    public RoomTypeController(IConfigurationServices config)
    {
        _config = config;
    }

    private readonly IConfigurationServices _config;
    
      
        [HttpPost()]
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

            var adminData = UserBuissnes.getUserByID((Guid)adminid);
            if (adminData.isUser == true)
            {
                return BadRequest("مدير النظام فقط من يمكنه انشاء نوع غرف");
            }

            if (roomTypeData.name.Length > 50)
                return StatusCode(400, "الاسم يجب الا يتجاوز ال 50 حرف");


            bool isExistName = RoomtTypeBuissnes.isExist(roomTypeData.name);

            if (isExistName)
                return StatusCode(400, "نوع الغرفة موجود بالفعل");


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
                return StatusCode(500, " هناك مشكالة ما");

            return StatusCode(201, new { message = "تم الانشاء بنجاح" });
        }


        [HttpGet("{isNotDeletion:bool}")]
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
                    return StatusCode(401, "تسجيل غير مصرح");
                }


                var roomtypes = RoomtTypeBuissnes.getRoomTypes(isNotDeletion ?? false);

                return Ok(roomtypes);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(500, "هناك خطب ما");
            }
        }


        [HttpPut()]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> updateRoomTypes(
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
                return StatusCode(401, "ليس لديك الصلاحية");
            }

            var adminData = UserBuissnes.getUserByID((Guid)adminid);
            
            if (adminData==null)
            {
                return BadRequest("المستخدم غير موجو");
            } 
          
            if (adminData.isUser == true)
            {
                return BadRequest("مدير النظام فقط من يمكنه انشاء نوع غرف");
            } 

            if (roomTypeData.name.Length > 50 || roomTypeData.Id == null)
                return StatusCode(400, "roomtype name must be under 50 characters");


            var roomtypeHolder = RoomtTypeBuissnes.getRoomType(roomTypeData.Id);

            if (roomtypeHolder == null)
                return StatusCode(400, "نوع الغرفة غير موجود");

            var imageHolder = ImageBuissness.getImageByBelongTo(roomTypeData.Id);

            string? imageHolderPath = null;
            if (roomTypeData.image != null)
            {
                imageHolderPath = await MinIoServices.uploadFile(_config, roomTypeData.image,
                    MinIoServices.enBucketName.RoomType, imageHolder.path);
            }


            clsUtil.saveImage(imageHolderPath, roomTypeData.Id, imageHolder);

            updateRoomTypeData(ref roomtypeHolder, roomTypeData, (Guid)adminid);
            var result = roomtypeHolder.save();

            if (result == false)
                return StatusCode(500, "هناك مشكله ما");

            return StatusCode(201, new { message = "تم التعديل بنجاح" });
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


        [HttpDelete("{roomtypeid:guid}")]
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
                return StatusCode(401, "دخول غير مصرح");
            }
            
            var adminData = UserBuissnes.getUserByID((Guid)adminid);

            if (adminData==null)
            {
                return BadRequest("المستخدم غير موجو");
            } 
          
            if (adminData.isUser == true)
            {
                return BadRequest("مدير النظام فقط من يمكنه انشاء نوع غرف");
            } 
            var data = RoomtTypeBuissnes.getRoomType(roomtypeid);

            if (data == null)
                return StatusCode(409, "نوع الغرفة غير موجود");


            var result = RoomtTypeBuissnes.deleteOrUnDeleteRoomType(roomtypeid,adminid:(Guid)adminid);
            if (result == false)
                return StatusCode(500, "هناك مشكلة ما");

            return Ok("تم التعديل بنجاح");
        }
 
}