using hotel_data.dto;
using Npgsql;

namespace hotel_data;

public class BookingData
{
    static string connectionUr = clsConnnectionUrl.url;

    public static BookingDto? getBooking(Guid bookingID)
    {
        BookingDto? bookingData = null;
        try
        {
            using (var con = new NpgsqlConnection(connectionUr))
            {
                con.Open();

                // Option 1: Using positional parameters
                string query =
                    @"SELECT * FROM bookings WHERE bookingid = @bookingID";

                using (var cmd = new NpgsqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@bookingID", bookingID);

                         using (var reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        if (reader.Read())
                        {
                            var BookingDto = new BookingDto(
                                bookingId: bookingID,
                                roomId: (Guid)reader["roomid"],
                                userId: (Guid)reader["belongto"],
                                bookingStart: (DateTime)reader["booking_start"],
                                bookingEnd: (DateTime)reader["booking_end"],
                                bookingStatus: (string)reader["bookingstatus"],
                                totalPrice: (decimal)reader["totalprice"],
                                
                                servicePayment: reader["servicepayment"] == DBNull.Value
                                    ? null
                                    : (decimal)reader["servicepayment"],
                                
                                maintenancePayment: reader["maintenancepayment"] == DBNull.Value
                                    ? null
                                    : (decimal)reader["maintenancepayment"],
                                
                                paymentStatus: (string)reader["paymentstatus"],
                                
                                createdAt: (DateTime)reader["createdat"],
                                
                                cancelledAt: reader["cancelledat"] == DBNull.Value
                                    ? null
                                    : (DateTime)reader["cancelledat"],
                                
                                cancellationReason: reader["cancellationreason"] == DBNull.Value
                                    ? null
                                    : (string)reader["cancellationreason"],
                                
                                actualCheckOut: reader["actualcheckout"] == DBNull.Value
                                    ? null
                                    : (DateTime)reader["actualcheckout"]
                            );
                            bookingData = BookingDto;
                        }
                    }
                }
           
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error update booking: {0}", ex);
        }

        return bookingData;
    }
    

    public static Guid? createBooking(BookingDto bookingData)
    {
        Guid? bookingID=null ;
        try
        {
            using (var con = new NpgsqlConnection(connectionUr))
            {
                con.Open();

                // Option 1: Using positional parameters
                string query =
                    @"SELECT * FROM fn_bookin_insert(@roomid_ , @totalprice_, @userid_,@startbookindate_,@endbookingdate_)";

                using (var cmd = new NpgsqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@roomid_", bookingData.roomId);
                    cmd.Parameters.AddWithValue("@totalprice_", bookingData.totalPrice);
                    cmd.Parameters.AddWithValue("@userid_", bookingData.userId);
                    cmd.Parameters.AddWithValue("@startbookindate_", bookingData.bookingStart);
                    cmd.Parameters.AddWithValue("@endbookingdate_", bookingData.bookingEnd);

                    var result = cmd.ExecuteScalar();
                    if (result != null && Guid.TryParse(result?.ToString(), out Guid bookingIDResult))
                    {
                    bookingID = bookingIDResult;        
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error creating booking: {0}", ex);
        }

        return bookingID;
    }

    public static bool updateBooking(BookingDto bookingData)
    {
        bool isUpdated = false;
        try
        {
            using (var con = new NpgsqlConnection(connectionUr))
            {
                con.Open();

                // Option 1: Using positional parameters
                string query =
                    @"SELECT * FROM fn_bookin_update(
                        @booking_id,
                        @totalprice_, 
                        @userid_ ,
                        @startbookindate_,
                        @endbookingdate_,
                    );";

                using (var cmd = new NpgsqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@booking_id", bookingData.bookingId);
                    cmd.Parameters.AddWithValue("@totalprice_", bookingData.totalPrice);
                    cmd.Parameters.AddWithValue("@userid_", bookingData.userId);
                    cmd.Parameters.AddWithValue("@startbookindate_", bookingData.bookingStart);
                    cmd.Parameters.AddWithValue("@endbookingdate_", bookingData.bookingEnd);

                    var result = cmd.ExecuteScalar();
                    isUpdated = result != null && (bool)result;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error update booking: {0}", ex);
        }

        return isUpdated;
    }

    public static bool cencelBooking(BookingDto bookingData)
    {
        bool isUpdated = false;
        try
        {
            using (var con = new NpgsqlConnection(connectionUr))
            {
                con.Open();

                // Option 1: Using positional parameters
                string query =
                    @"SELECT * FROM fn_bookin_update(@booking_id ,   
                      @totalprice_,  
                      @userid_ ,
                      @bookingstatus_,  
                      @startbookindate_ ,  
                      @endbookingdate_)";

                using (var cmd = new NpgsqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@booking_id", bookingData.bookingId);
                    cmd.Parameters.AddWithValue("@totalprice_",DBNull.Value);
                    cmd.Parameters.AddWithValue("@userid_",bookingData.userId);
                    cmd.Parameters.AddWithValue("@bookingstatus_",bookingData.bookingStatus);
                    cmd.Parameters.AddWithValue("@startbookindate_",DBNull.Value);
                    cmd.Parameters.AddWithValue("@endbookingdate_",DBNull.Value);

                    var result = cmd.ExecuteScalar();
                    if (bool.TryParse(result.ToString(), out bool resultInt))
                    {
                        isUpdated = resultInt; 
                    }
                    
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error update booking: {0}", ex);
        }

        return isUpdated;
    }

    public static bool isValideBookingDate
    ( 
        DateTime startBookingDate, 
        DateTime endBookingDate, 
        Guid? guid
        
        )
    {
        bool isVisibleBooking = false;
        try
        {
            using (var con = new NpgsqlConnection(connectionUr))
            {
                con.Open();
                string query = @"SELECT * FROM fn_isValid_booking(
                                   @startBooking,
                                  @endBooking ,
                                  @belongTo_
                                    );";

                using (var cmd = new NpgsqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@startBooking", startBookingDate);
                    cmd.Parameters.AddWithValue("@endBooking", endBookingDate);
                    cmd.Parameters.AddWithValue("@belongTo_", guid!=null?guid:DBNull.Value);
                    var result = cmd.ExecuteScalar();

                    if (result != null && bool.TryParse(result?.ToString(), out bool isVisibleBookingResult))
                    {
                        isVisibleBooking = isVisibleBookingResult;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("this from check if the booking is visible error {0}", ex);
        }

        return isVisibleBooking;
    }

    public static List<string>? getBookingDayesAtMonthAndYearData(int year,
        int month, Guid? bookingID)
    {
        List<string>? bookingsDayAtYearAndMonth = null;
        try
        {
            using (var con = new NpgsqlConnection(connectionUr))
            {
                con.Open();
                string query = @"SELECT * FROM fun_get_list_of_booking_at_specific_month_and_year(
                                   @year_,
                                  @month_ , 
                                  @bookingID  );";

                using (var cmd = new NpgsqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@year_", year);
                    cmd.Parameters.AddWithValue("@month_", month);
                    cmd.Parameters.AddWithValue("@bookingID", bookingID!=null?bookingID:DBNull.Value);
                    var result = cmd.ExecuteScalar();

                    if (result != null && result.ToString().Length > 0)
                    {
                        bookingsDayAtYearAndMonth = Convert.ToString(result)?.Split(',').ToList();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("this from check if the booking is visible error {0}", ex);
        }

        return bookingsDayAtYearAndMonth;
    }

    public static List<BookingDto> getUserBookingData(Guid userId, int pageNumber, int limitSize = 25)
    {
        List<BookingDto> bookingList = new List<BookingDto>();
        try
        {
            using (var con = new NpgsqlConnection(connectionUr))
        {
            con.Open();
            string query = "SELECT * FROM fun_getBookingPagination(@belongId ,@pageNumber , @limitNumber );";
            using (var cmd = new NpgsqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@belongId", userId);
                cmd.Parameters.AddWithValue("@pageNumber", pageNumber>=1?1:pageNumber);
                cmd.Parameters.AddWithValue("@limitNumber", limitSize);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var BookingDto = new BookingDto(
                                bookingId: (Guid)reader["bookingid"],
                                roomId: (Guid)reader["roomid"],
                                userId: (Guid)reader["belongto"],
                                bookingStart: (DateTime)reader["booking_start"],
                                bookingEnd: (DateTime)reader["booking_end"],
                                bookingStatus: (string)reader["bookingstatus"],
                                totalPrice: (decimal)reader["totalprice"],
                                
                                servicePayment: reader["servicepayment"] == DBNull.Value
                                    ? null
                                    : (decimal)reader["servicepayment"],
                                
                                maintenancePayment: reader["maintenancepayment"] == DBNull.Value
                                    ? null
                                    : (decimal)reader["maintenancepayment"],
                                
                                paymentStatus: (string)reader["paymentstatus"],
                                
                                createdAt: (DateTime)reader["createdat"],
                                
                                cancelledAt: reader["cancelledat"] == DBNull.Value
                                    ? null
                                    : (DateTime)reader["cancelledat"],
                                
                                cancellationReason: reader["cancellationreason"] == DBNull.Value
                                    ? null
                                    : (string)reader["cancellationreason"],
                                
                                actualCheckOut: reader["actualcheckout"] == DBNull.Value
                                    ? null
                                    : (DateTime)reader["actualcheckout"]
                            );
                            bookingList.Add(BookingDto);
                        }
                    }
                }
            }
        } 
        }
        catch (Exception ex)
        {
            Console.WriteLine("this from check from get bookingList {0}", ex);
        } 
        
        return bookingList;
    }
    
    public static List<BookingDto> getBookingBelongToUserRoomData(Guid userId, int pageNumber, int limitSize = 25)
    {
        List<BookingDto> bookingList = new List<BookingDto>();
        try
        {
            using (var con = new NpgsqlConnection(connectionUr))
        {
            con.Open();
            string query = "SELECT * FROM fun_getBookingBelongToUserPagination(@belongId ,@pageNumber , @limitNumber );";
            using (var cmd = new NpgsqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@belongId", userId);
                cmd.Parameters.AddWithValue("@pageNumber", pageNumber>=1?1:pageNumber);
                cmd.Parameters.AddWithValue("@limitNumber", limitSize);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var BookingDto = new BookingDto(
                                bookingId: (Guid)reader["bookingid"],
                                roomId: (Guid)reader["roomid"],
                                userId: (Guid)reader["belongto"],
                                bookingStart: (DateTime)reader["booking_start"],
                                bookingEnd: (DateTime)reader["booking_end"],
                                bookingStatus: (string)reader["bookingstatus"],
                                totalPrice: (decimal)reader["totalprice"],
                                
                                servicePayment: reader["servicepayment"] == DBNull.Value
                                    ? null
                                    : (decimal)reader["servicepayment"],
                                
                                maintenancePayment: reader["maintenancepayment"] == DBNull.Value
                                    ? null
                                    : (decimal)reader["maintenancepayment"],
                                
                                paymentStatus: (string)reader["paymentstatus"],
                                
                                createdAt: (DateTime)reader["createdat"],
                                
                                cancelledAt: reader["cancelledat"] == DBNull.Value
                                    ? null
                                    : (DateTime)reader["cancelledat"],
                                
                                cancellationReason: reader["cancellationreason"] == DBNull.Value
                                    ? null
                                    : (string)reader["cancellationreason"],
                                
                                actualCheckOut: reader["actualcheckout"] == DBNull.Value
                                    ? null
                                    : (DateTime)reader["actualcheckout"]
                            );
                            bookingList.Add(BookingDto);
                        }
                    }
                }
            }
        } 
        }
        catch (Exception ex)
        {
            Console.WriteLine("this from check from get bookingList {0}", ex);
        } 
        
        return bookingList;
    }
    
}