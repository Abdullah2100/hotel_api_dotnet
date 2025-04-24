namespace hotel_api_.RequestDto;

public class UserUpdateDto
{
    public Guid? id { get; set; } = null;

    
    public string name { get; set; } = "";

     public string? email { get; set; } = null;

    
    public string? phone { get; set; } = null;

    public string? address { get; set; } = null;

    public string userName { get; set; } = "";

    
    public string? password { get; set; } = "";
    public string? currenPassword { get; set; } = null;


    public bool? isVip { get; set; } = false;
    
    public Guid? updated_by { get; set; } = null;


    public IFormFile? imagePath { get; set; } 
 
}