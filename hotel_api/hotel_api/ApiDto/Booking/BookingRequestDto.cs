using System.ComponentModel.DataAnnotations;

namespace hotel_api_.RequestDto.Booking;

public class BookingRequestDto
{
    public Guid? bookingID { get; set; }

    [Required] public Guid roomId { get; set; }
   [Required] public DateTime bookingStartDateTime { get; set; }
    [Required] public DateTime bookingEndDateTime { get; set; }
}