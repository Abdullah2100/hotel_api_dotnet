namespace hotel_data.dto;

public class BookingDto
{
    public BookingDto(
        Guid? bookingId, 
        Guid roomId, 
        Guid userId, 
        DateTime bookingStart, 
        DateTime bookingEnd, 
        string? bookingStatus, 
        decimal totalPrice, 
        decimal? servicePayment, 
        decimal? maintenancePayment, 
        string paymentStatus, 
        DateTime? createdAt, 
        DateTime? cancelledAt, 
        string? cancellationReason, 
        DateTime? actualCheckOut
         )
    {
        this.bookingId = bookingId;
        this.roomId = roomId;
        this.userId = userId;
        this.bookingStart = bookingStart;
        this.bookingEnd = bookingEnd;
        this.bookingStatus = bookingStatus;
        this.totalPrice = totalPrice;
        this.servicePayment = servicePayment;
        this.maintenancePayment = maintenancePayment;
        this.paymentStatus = paymentStatus;
        this.createdAt = createdAt;
        this.cancelledAt = cancelledAt;
        this.cancellationReason = cancellationReason;
        this.actualCheckOut = actualCheckOut;
        this.room= RoomData.getRoom(roomId);
        this.user = UserData.getUser(userId);
        
    }

    public Guid? bookingId { get; set; }
    public Guid roomId { get; set; }
    public Guid userId { get; set; }
    public DateTime bookingStart { get; set; }
    public DateTime bookingEnd { get; set; }
    public string? bookingStatus { get; set; }
    public decimal totalPrice { get; set; }
    public decimal? servicePayment { get; set; }
    public decimal? maintenancePayment { get; set; }
    public string? paymentStatus { get; set; }
    public DateTime? createdAt { get; set; }
    public DateTime? cancelledAt { get; set; }
    public string? cancellationReason { get; set; }
    public DateTime? actualCheckOut { get; set; }
    public RoomDto? room { get; set; }
    public UserDto? user { get; set; }
}