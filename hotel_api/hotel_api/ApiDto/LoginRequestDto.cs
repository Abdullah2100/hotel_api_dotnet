using System.ComponentModel.DataAnnotations;

namespace hotel_api.controller;

public class LoginRequestDto
{
    [Required] public string userNameOrEmail { get; set; } = "";
    [Required] public string password { get; set; } = "";

}