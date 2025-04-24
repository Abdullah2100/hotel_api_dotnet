namespace hotel_data.dto;

public class ImagesTbDto
{ 
    
    public ImagesTbDto(
        Guid? imagePathId,
        string? imagePath,
        Guid? belongTo,
        bool? isDeleted = false,
        bool? isThumnail = false
        )
    {
        this.id = imagePathId;
        this.path = imagePath;
        this.belongTo = belongTo;
        this.isDeleted = isDeleted;
        this.isThumnail = isThumnail;
    }
  
    public Guid? id  { get; set; }
    public string? path { get; set; }
    public Guid? belongTo   { get; set; }
    public bool? isDeleted { get; set; } = false;
    public bool? isThumnail { get; set; }= false;
}