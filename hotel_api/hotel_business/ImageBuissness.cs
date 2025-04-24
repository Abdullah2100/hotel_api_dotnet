using hotel_data;
using hotel_data.dto;

namespace hotel_business;

public class ImageBuissness
{
    public enum enMode {add,update}
    public enMode mode { get; set; }  
    public Guid? ID  { get; set; }
    public string path { get; set; }
    public Guid? belongTo   { get; set; }    
    public bool? isThumnail   { get; set; }    
    public ImagesTbDto imageHolder{get{return new ImagesTbDto(
        imagePathId:ID,
        imagePath:path,
        belongTo:belongTo,
        isThumnail:isThumnail
        );}}
    public ImageBuissness(
        ImagesTbDto image,
        enMode mode = enMode.add
        )
    {
        this.ID = image.id;
        this.path = image.path;
        this.belongTo = image.belongTo;
        this.isThumnail = image.isThumnail;
        this.mode = mode;   
    }

    private bool _createImage()
    {
        return ImagesData.createImages(imageHolder);
    }

    private bool _updateImage()
    {
        return ImagesData.updateImages(imageHolder);
    }
    public bool save()
    {
        switch (mode)
        {
            case enMode.add:
            {
                return _createImage() ? true : false; 
            }
            case enMode.update: return _updateImage()? true : false;
            
            default: return false;
        }
    }

    
    public static ImageBuissness? getImageById(Guid id)
    { 
        ImagesTbDto? result =ImagesData.image(id:id);
        if (result != null)
        {
            return new ImageBuissness(result, enMode.update);
        }

        return null;
    }
    public static ImageBuissness? getImageByBelongTo(Guid belongTo)
    { 
        ImagesTbDto? result =ImagesData.image(belongto:belongTo);
        if (result != null)
        {
            return new ImageBuissness(result, enMode.update);
        }

        return null;
    }
    public static List<ImageBuissness> getImages(Guid belongTo)
    {
        var  result = ImagesData.images(belongto:belongTo);
        List<ImageBuissness> images = new List<ImageBuissness>();
        foreach (var image in result)
        {
            var imageHolder = new ImageBuissness(image, enMode.update);
            images.Add(imageHolder);
        }

        return images;
    }

    public static bool deleteImageByBelgonTo(Guid belongTo)
    {
        return ImagesData.deleteImage(belongTo:belongTo);
    }
    
    public static bool deleteImage(Guid id)
    {
        return ImagesData.deleteImageById(id);
    }
}