using hotel_data;
using hotel_data.dto;

namespace hotel_business;

public class BookingBuiseness
{
    enMode mode = enMode.add;
    public Guid? ID { get; set; }
    public Guid roomId { get; set; }
    public Guid userId { get; set; }
    public DateTime bookingStart { get; set; }
    public DateTime bookingEnd { get; set; }
    public string bookingStatus { get; set; }
    public decimal totalPrice { get; set; }
    public decimal? servicePayment { get; set; }
    public decimal? maintenancePayment { get; set; }
    public string? paymentStatus { get; set; }
    public DateTime? createdAt { get; set; }
    public DateTime? cancelledAt { get; set; }
    public string? cancellationReason { get; set; }
    public DateTime? actualCheckOut { get; set; }

    public BookingDto booking
    {
        get
        {
            return new BookingDto(
                bookingId: this.ID,
                roomId: this.roomId,
                userId: this.userId,
                bookingStart: this.bookingStart,
                bookingEnd: this.bookingEnd,
                bookingStatus: this.bookingStatus,
                totalPrice: this.totalPrice,
                servicePayment: this.servicePayment,
                maintenancePayment: this.maintenancePayment,
                paymentStatus: this.paymentStatus,
                createdAt: this.createdAt,
                cancelledAt: this.cancelledAt,
                cancellationReason: this.cancellationReason,
                actualCheckOut: this.actualCheckOut);
        }
    }


    public BookingBuiseness(
        BookingDto booking,
        enMode mode = enMode.add
    )
    {
        this.ID = booking.bookingId;
        this.roomId = booking.roomId;
        this.userId = booking.userId;
        this.bookingStart = booking.bookingStart;
        this.bookingEnd = booking.bookingEnd;
        this.bookingStatus = booking.bookingStatus;
        this.totalPrice = booking.totalPrice;
        this.servicePayment = booking.servicePayment;
        this.maintenancePayment = booking.maintenancePayment;
        this.paymentStatus = booking.paymentStatus;
        this.createdAt = booking.createdAt ?? DateTime.Now;
        this.cancelledAt = booking.cancelledAt ?? null;
        this.cancellationReason = booking.cancellationReason ?? null;
        this.actualCheckOut = booking.actualCheckOut ?? null;
        this.mode = mode;
    }

    private bool _createBooking()
    {
        this.ID= BookingData.createBooking(this.booking);
        return (ID != null);
    }
    
    private bool _updateBooking()
    {
        return BookingData.updateBooking(this.booking);
    }
    
    public bool save()
    {
        switch (mode)
        {
            case enMode.add: return _createBooking();
            case enMode.update: return _updateBooking();
            default: return false;
        }
    }

    public static bool isValidBooking(DateTime startBookingDate,
        DateTime endBookingDate, Guid? guid=null)
    {
        return BookingData.isValideBookingDate(
            startBookingDate: startBookingDate,
            endBookingDate: endBookingDate,
            guid
            );
    }
      public static List<string> getBookingDayesAtMonthAndYearBuissness(
          int year,
          int month, 
          Guid? bookingID
          )
        {
            var bookingData =  BookingData.getBookingDayesAtMonthAndYearData(
                year: year, 
                month: month,
                bookingID:bookingID);
            switch (bookingData==null)
            {
                case false: return bookingData;
                default: return [];
            }
        }


        public static List<BookingDto> getUserBookingList(Guid userId, int pageNumber, int limitSize = 25,bool isBelngToMe = false)
        {
            switch (isBelngToMe)
            {
                case true: return BookingData.getBookingBelongToUserRoomData(userId, pageNumber, limitSize);
                default: return BookingData.getUserBookingData(userId, pageNumber, limitSize);
            }
        }

        public  BookingBuiseness getBooking()
        {
            var result = BookingData.getBooking((Guid)this.ID!);
            if (result != null)
            {
                return new  BookingBuiseness(result, enMode.update);
            }
            return null ;
        }


        public static BookingBuiseness? getBooking(Guid bookingId)
        {
            var result = BookingData.getBooking(bookingId);
            if (result != null)
            {
                return new  BookingBuiseness(result, enMode.update);
            }
            return null ;
        }

        public  bool cencleBooking()
        {
            return BookingData.cencelBooking(booking);
        }
}