using System.ComponentModel.DataAnnotations;
using hotel_api.util;

namespace hotel_api_.AnotationValidation;

public class IsImageFile : ValidationAttribute
{
    public override string FormatErrorMessage(string name)
    {
        return base.FormatErrorMessage(name);
    }

    public override bool IsValid(object? value)
    {
        if (value == null)
            return false; 
        IFormFile? fileHolder = value as IFormFile;

        if (fileHolder == null) return false;
        
        string fileExtetnion = clsUtil.getFileExtention(fileHolder.FileName);
        
        return  (fileExtetnion == "png" || fileExtetnion == "jpg" || fileExtetnion == "jpeg" || fileExtetnion=="svg")?
            true:false;
    }
}