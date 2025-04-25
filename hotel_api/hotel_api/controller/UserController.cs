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
[Route("api/user")]
public class UserController : ControllerBase
{
    private readonly IConfigurationServices _config;

    public UserController(IConfigurationServices config)
    {
        this._config = config;
    }

 
    [HttpGet("info")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult getMyInfo()
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
            return Unauthorized("هناك مشكلة في التحقق");
        }

        var userInfo = UserBuissnes.getUserByID((Guid)userID);

        return Ok(userInfo.userDataHolder);
    }

    [AllowAnonymous]
    [HttpPost("signUp")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult userSignUp(
        UserRequestDto userRequest
    )
    {
        string? validateRequeset = clsValidation.validateInput(
            phone: userRequest.phone,
            email: userRequest.email,
            password: userRequest.password,
            username: userRequest.userName,
            name: userRequest.name
        );

        if (validateRequeset != null)
            return BadRequest(validateRequeset);


        bool isExistEmailOrPhone = PersonBuisness.isPersonExistByEmailAndPhone(userRequest.email, userRequest.phone);

        if (isExistEmailOrPhone)
            return BadRequest("الايميل او رقم الهات تم استخدامه سابقا");


        var data = UserBuissnes.getUserByUserName(userRequest.userName);

        if (data != null)
            return BadRequest("اسم المستخدم تم استخدامه");

        if (userRequest.add_by != null)
        {
            var adminData = UserBuissnes.getUserByID((Guid)userRequest.add_by);
            if (adminData.isUser == true)
            {
                return BadRequest("مدير النظام فقط من يمكنه انشاء مستخدمين");
            }
        }


        var userId = Guid.NewGuid();

        var personDataHolder = new PersonDto(
            personID: null,
            email: userRequest.email,
            name: userRequest.name,
            phone: userRequest.phone,
            address: userRequest.address ?? ""
        );


        data = new UserBuissnes(new UserDto(
            userId: userId,
            brithDay: userRequest.brithDay,
            isVip: null,
            personData: personDataHolder,
            userName: userRequest.userName,
            password: clsUtil.hashingText(userRequest.password),
            isUser: userRequest.role,
            addBy: userRequest.add_by
        ));

        var result = data.save();

        string accesstoken = "", refreshToken = "";
        if (result == false)
            return StatusCode(500, "هناك مشكلة ما");

        accesstoken = AuthinticationServices.generateToken(
            userID: userId,
            email: data.personData.email,
            config: _config,
            enTokenMode: AuthinticationServices.enTokenMode.AccessToken);
        refreshToken = AuthinticationServices.generateToken(
            userID: userId,
            email: data.personData.email,
            config: _config,
            enTokenMode: AuthinticationServices.enTokenMode.RefreshToken);

        return StatusCode(201, new { accessToken = $"{accesstoken}", refreshToken = $"{refreshToken}" });
    }

    [AllowAnonymous]
    [HttpPost("signIn")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult userSignIn(LoginRequestDto loginData)
    {
        var data = UserBuissnes.getUserByUserNameAndPassword(loginData.userNameOrEmail,
            clsUtil.hashingText(loginData.password));

        if (data == null)
            return NotFound("المستخدم غير موجود");

        string accesstoken = "", refreshToken = "";

        accesstoken = AuthinticationServices.generateToken(data.ID, data.personData.email, _config,
            AuthinticationServices.enTokenMode.AccessToken);
        refreshToken = AuthinticationServices.generateToken(data.ID, data.personData.email, _config,
            AuthinticationServices.enTokenMode.RefreshToken);


        return StatusCode(200, new { accessToken = $"{accesstoken}", refreshToken = $"{refreshToken}" });
    }


    [HttpPut("")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> updateUser(
        [FromForm] UserUpdateDto userRequestDto
    )
    {
        var authorizationHeader = HttpContext.Request.Headers["Authorization"];
        var id = AuthinticationServices.GetPayloadFromToken("id",
            authorizationHeader.ToString().Replace("Bearer ", ""));
        if (id == null)
        {
            return Unauthorized("هناك مشكلة في التحقق");
        }

        Guid? userID = null;
        if (Guid.TryParse(id.Value.ToString(), out Guid outID))
        {
            userID = outID;
        }


        //this to get the userInsert data
        var user = UserBuissnes.getUserByID((Guid)userRequestDto.id);

        if (user == null)
            return NotFound("المستخدم غير موجود");

        
        if (userRequestDto.updated_by != null)
        {
            var adminData = UserBuissnes.getUserByID(((Guid)userRequestDto.updated_by));
            if (adminData?.isUser == true)
            {
                if (user?.isUser == false)
                {
                    return BadRequest("لا يمكن تعديل المعلومات الخاصة للمدراء");
                }

                return BadRequest("فقط مدير النظام من لديه الصلاحية لحذف المستخدم");
            }
            else
            {
                if (user.isVip == false && userRequestDto.isVip == true)
                    user.isVip = true;
                user.updateBy = adminData.ID;
            }
        }

        string? validateRequeset = clsValidation.validateInput(phone: userRequestDto.phone,
            email: userRequestDto.email, password: userRequestDto.password, isNeedValidateOther: false);

        if (validateRequeset != null)
            return StatusCode(400, validateRequeset);


        bool isExistPhone = false;

        if (userRequestDto.phone != null && userRequestDto?.phone?.Length > 0 &&
            userRequestDto.phone != user.personData.phone)
            isExistPhone = PersonBuisness.isPersonExistByPhone(userRequestDto.phone);


        if (isExistPhone)
            return BadRequest("رقم الهات بالفعل مستخدم");


        var imageHolder = ImageBuissness.getImageByBelongTo(user.ID);

        string? imagePath = null;
        if (userRequestDto.imagePath != null)
        {
            imagePath = await MinIoServices.uploadFile(_config, userRequestDto.imagePath,
                MinIoServices.enBucketName.USER, imageHolder?.path ?? "");
        }


        clsUtil.saveImage(imagePath, user.ID, imageHolder);

        user.imagePath = imagePath;

        updateUserData(ref user, userRequestDto);


        var result = user.save();
        if (result == false)
            return StatusCode(500, "هناك مشكلة");

        return Ok("تم تعديل البيانات بنجاح");
    }


    private void updateUserData(
        ref UserBuissnes user,
        UserUpdateDto? userRequestData
    )
    {
        if (userRequestData == null) return;

        if (userRequestData?.name?.Length > 0 && user.personData.name != userRequestData.name)
        {
            user.personData.name = userRequestData.name;
        }

        if (userRequestData?.address?.Length > 0 && user.personData.address != userRequestData.address)
        {
            user.personData.address = userRequestData.address;
        }


        if (userRequestData?.userName?.Length > 0 && user.userName != userRequestData.userName)
            user.userName = userRequestData.userName;
        if (
            userRequestData?.currenPassword != null && userRequestData.currenPassword.Length > 0 &&
            userRequestData.password != null && userRequestData.password.Length > 0 &&
            UserBuissnes.isExistByPassword(clsUtil.hashingText(userRequestData.currenPassword))
        )
            user.password = clsUtil.hashingText(userRequestData.password);

        if (userRequestData?.phone != null &&user.personData.phone!=userRequestData.phone)
        {
            user.userDataHolder.personData.phone = userRequestData.phone;
        }
    }


    [Authorize]
    [HttpDelete("User/{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult deleteUser(
        Guid userId
    )
    {
        var authorizationHeader = HttpContext.Request.Headers["Authorization"];
        var id = AuthinticationServices.GetPayloadFromToken("id",
            authorizationHeader.ToString().Replace("Bearer ", ""));
        if (id == null)
        {
            return Unauthorized("هناك مشاكل في التحقق");
        }


        Guid? adminid = null;
        if (Guid.TryParse(id.Value.ToString(), out Guid outID))
        {
            adminid = outID;
        }


        var admin = UserBuissnes.getUserByID(adminid ?? Guid.Empty);

        if (admin == null)
        {
            return NotFound("المدير غير موجود");
        }


        var data = UserBuissnes.getUserByID(userId);

        if (data == null)
            return NotFound("المستخدم غير موجود");

        if (admin.isUser == true)
        {
            if (data.isUser == false)
            {
                return BadRequest("لا يمكن تعديل المعلومات الخاصة للمدراء");
            }

            return BadRequest("فقط مدير النظام من لديه الصلاحية لحذف المستخدم");
        }

        var result = UserBuissnes.deletedUser(userId,(Guid)adminid);
        if (result == false)
            return StatusCode(500, "هناك مشكلة ما");

        return Ok("تم حذف المستخدم بنجاح");
    }

    [Authorize]
    [HttpPost("User/{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult makeUserVip(
        Guid userId
    )
    {
        var authorizationHeader = HttpContext.Request.Headers["Authorization"];
        var id = AuthinticationServices.GetPayloadFromToken("id",
            authorizationHeader.ToString().Replace("Bearer ", ""));
        if (id == null)
        {
            return Unauthorized("هناك مشاكل في التحقق");
        }


        Guid? adminid = null;
        if (Guid.TryParse(id.Value.ToString(), out Guid outID))
        {
            adminid = outID;
        }


        var admin = UserBuissnes.getUserByID(adminid ?? Guid.Empty);


        if (admin == null)
        {
            return NotFound("المدير غير موجود");
        }


        var data = UserBuissnes.getUserByID(userId);

        if (data == null)
            return NotFound("المستخدم غير موجود");

        if (admin.isUser == true)
        {
            if (data.isUser == false)
            {
                return BadRequest("لا يمكن تعديل المعلومات الخاصة للمدراء");
            }

            return BadRequest("فقط مدير النظام من لديه الصلاحية لحذف المستخدم");
        }

        var result = UserBuissnes.makeVipUser(userId);
        if (result == false)
            return StatusCode(500, "some thing wrong");

        return Ok("تم تعديل المستخدم ل vip");
    }
}