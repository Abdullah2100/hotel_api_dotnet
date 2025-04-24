using System.ComponentModel.DataAnnotations;
using hotel_data.dto;

namespace hotel_api_.RequestDto.Booking;

public class BookingRequestUpdateDto
{
    [Required]  public Guid? bookingId { get; set; }
    [Required]  public Guid roomId { get; set; }
    [Required] public Guid userId { get; set; }
    public DateTime? bookingStart { get; set; }
    public DateTime? bookingEnd { get; set; }
    [Required] public string bookingStatus { get; set; }
    [Required] public DateTime? createdAt { get; set; }
    public DateTime? cancelledAt { get; set; }
    public string? cancellationReason { get; set; }
}