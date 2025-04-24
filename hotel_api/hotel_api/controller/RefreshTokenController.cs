using hotel_api.Services;
using hotel_api.util;
using hotel_business;
using Microsoft.AspNetCore.Mvc;

namespace hotel_api.controller;

[ApiController]
[Route("api/refreshToken")]
public class RefreshTokenController : Controller
{
    private readonly IConfigurationServices _config;

    public RefreshTokenController(IConfigurationServices config)
    {
        this._config = config;
    }


    [HttpPost("refresh")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult generateRefreshToken(string tokenHolder)
    {
        Console.Write($"{DateTime.Now}");
        if (!clsTokenUtil.isValidToken(tokenHolder, _config))
            return Unauthorized("Invalid token");

        var issuAt = AuthinticationServices.GetPayloadFromToken("exp", tokenHolder);
        var expire = AuthinticationServices.GetPayloadFromToken("lat", tokenHolder);
        var email = AuthinticationServices.GetPayloadFromToken("email", tokenHolder);
        var id = AuthinticationServices.GetPayloadFromToken("id", tokenHolder);

        if (issuAt == null || expire == null || email == null || id == null)
            return Unauthorized("Invalid token");

        if (!clsTokenUtil.isRefreshToken(issuAt.Value, expire.Value))
        {
            return Unauthorized("Invalid token");
        }


        var guid_id = Guid.Parse(id.Value);


        if (!UserBuissnes.isExistByEmailAndID(email.Value, guid_id))
        {
            return Unauthorized("unAuthorize person ");
        }

        string accesstoken = "", refreshToken = "";

        accesstoken = AuthinticationServices.generateToken(guid_id, email.Value, _config,
            AuthinticationServices.enTokenMode.AccessToken);
        refreshToken = AuthinticationServices.generateToken(guid_id, email.Value, _config,
            AuthinticationServices.enTokenMode.RefreshToken);

        return Ok(new { accessToken = $"{accesstoken}", refreshToken = $"{refreshToken}" });
    }
}