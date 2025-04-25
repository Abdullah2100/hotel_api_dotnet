using hotel_api_.RequestDto.Booking;
using hotel_api.Services;
using hotel_business;
using hotel_data.dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace hotel_api.controller;

[ApiController]
[Route("api/booking")]
public class BookingController : ControllerBase
{
    
    [HttpPut("")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult updateBooking
        (BookingRequestDto bookingData)
    {
        var authorizationHeader = HttpContext.Request.Headers["Authorization"];
        var id = AuthinticationServices.GetPayloadFromToken("id",
            authorizationHeader.ToString().Replace("Bearer ", ""));
        Guid? userID = null;
        if (Guid.TryParse(id.Value.ToString(), out Guid outID))
        {
            userID = outID;
        }

        if (userID == null)
        {
            return StatusCode(401, "you not have Permission");
        }

        if (bookingData.bookingID == null)
        {
            return StatusCode(401, "لا بد من ادراج رقم الحجز");
        }

        var bookingHolder = BookingBuiseness.getBooking((Guid)bookingData.bookingID);

        if (bookingHolder.userId != userID)
            return StatusCode(401, "ليس لديك الصلاحية للتعديل على هذا الحجز");


        if (bookingHolder == null)
        {
            return StatusCode(401, "لا يوجد حجز بهذا الرقم");
        }


        var isVisibleBooking =
            BookingBuiseness.isValidBooking(
                bookingData.bookingStartDateTime,
                bookingData.bookingEndDateTime,
                userID
            );

        if (!isVisibleBooking)
            return BadRequest("هناك حجز ضمن الفترة المختارة");

        var bookingFullDate = (bookingData.bookingEndDateTime - bookingData.bookingStartDateTime);

        if (bookingFullDate.Days == 0)
        {
            return BadRequest("booking at least one day is required");
        }

        var bookingDayes = Convert.ToDecimal(bookingFullDate.Days);

        var room = RoomBuisness.getRoom(bookingData.roomId);

        if (room.beglongTo == userID)
            return BadRequest("لا يمكن حجز غرفة انت صاحبها");

        var totalPriceHolder = (bookingDayes * room.pricePerNight);

        bookingHolder.bookingEnd = bookingData.bookingEndDateTime;
        bookingHolder.bookingStart = bookingData.bookingStartDateTime;
        bookingHolder.totalPrice = totalPriceHolder;


        var result = bookingHolder.save();

        if (result == false)
            return StatusCode(500, "some thing wrong");

        return StatusCode(200, new { message = "booking created seccessfully" });
    }

    [HttpDelete("{bookingId:guid}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult deleteBooking
        (Guid bookingId)
    {
        var authorizationHeader = HttpContext.Request.Headers["Authorization"];
        var id = AuthinticationServices.GetPayloadFromToken("id",
            authorizationHeader.ToString().Replace("Bearer ", ""));
        Guid? userID = null;
        if (Guid.TryParse(id.Value.ToString(), out Guid outID))
        {
            userID = outID;
        }

        if (userID == null)
        {
            return StatusCode(401, "you not have Permission");
        }

        var bookingHolder = BookingBuiseness.getBooking(bookingId);

        if (bookingHolder == null)
        {
            return StatusCode(401, "لا يوجد حجز بهذا الرقم");
        }

        if (bookingHolder.userId != userID)
            return StatusCode(401, "ليس لديك الصلاحية للتعديل على هذا الحجز");

        bookingHolder.bookingStatus = "Cancelled";


        var result = bookingHolder.cencleBooking();

        if (result == false)
            return StatusCode(500, "some thing wrong");

        return StatusCode(200, new { message = "تم الغاء الحجز بنجاح" });
    }


    [HttpPost("between{year:int}&{month:int}&{bookingID:guid}")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult getBookingDayAtYearAndMont
        (int year, int month, Guid? bookingID = null)
    {
        List<string> bookingDay = BookingBuiseness.getBookingDayesAtMonthAndYearBuissness(
            year,
            month,
            bookingID);
        return StatusCode(200, bookingDay ?? []);
    }


    [HttpGet("{pageNumber:int}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult bookings
        (int pageNumber)
    {
        var authorizationHeader = HttpContext.Request.Headers["Authorization"];
        var id = AuthinticationServices.GetPayloadFromToken("id",
            authorizationHeader.ToString().Replace("Bearer ", ""));
        Guid? userID = null;
        if (Guid.TryParse(id.Value.ToString(), out Guid outID))
        {
            userID = outID;
        }

        if (userID == null)
        {
            return StatusCode(401, "you not have Permission");
        }

        var myBookingListData = BookingBuiseness.getUserBookingList(userID.Value, pageNumber, 24);


        return Ok(myBookingListData);
    }


    [HttpGet("myRooms/{pageNumber:int}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult bookingsForMyRooms
        (int pageNumber)
    {
        var authorizationHeader = HttpContext.Request.Headers["Authorization"];
        var id = AuthinticationServices.GetPayloadFromToken("id",
            authorizationHeader.ToString().Replace("Bearer ", ""));
        Guid? userID = null;
        if (Guid.TryParse(id.Value.ToString(), out Guid outID))
        {
            userID = outID;
        }

        if (userID == null)
        {
            return StatusCode(401, "you not have Permission");
        }

        var myBookingListData = BookingBuiseness.getUserBookingList(userID.Value, pageNumber, 24, true);

        return Ok(myBookingListData);
    }
    
}