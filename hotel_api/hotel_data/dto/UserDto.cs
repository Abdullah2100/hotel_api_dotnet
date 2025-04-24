using System;

namespace hotel_data.dto;

public class UserDto
{
    public Guid userId  { get; set; }

  
    public Guid? addBy  { get; set; }
    public Guid? updatedBy  { get; set; }

    public DateTime? brithDay   { get; set; }

    public bool? isVip   { get; set; } = false;

    public PersonDto personData { get; set; }

    public string userName { get; set; }

    public string password { get; set; }
    public bool isDeleted { get; set; }
    public string? imagePath { get; set; } = null;

    public bool? isUser { get; set; } = true;

    public UserDto(
        Guid userId,
        PersonDto personData, 
        string userName,
        string password,
        DateTime? brithDay= null, 
        bool? isVip = null,
        Guid? addBy=null,
        bool isDeleted=false,
        string? imagePath=null,
        bool?isUser=null,
        Guid? updatedBy=null
      )
    {
        this.userId = userId;

        this.brithDay = brithDay;
        this.isVip = isVip;
        this.personData = personData;
        this.userName = userName;
        this.password = password;
        this.addBy = addBy;
        this.isDeleted = isDeleted;
        this.imagePath = imagePath;
        this.isUser = isUser??false;
        this.updatedBy = updatedBy;
    }
}