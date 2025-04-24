using hotel_data.dto;
using Npgsql;

namespace hotel_data;

public class RoomData
{
    static string connectionUr = clsConnnectionUrl.url;
    private static string minioUrl = clsConnnectionUrl.minIoConnectionUrl + "room/";

    public static RoomDto? getRoom(Guid roomID)
    {
        RoomDto? room = null;
        try
        {
            using (var con = new NpgsqlConnection(connectionUr))
            {
                con.Open();
                string query = @"select * from rooms where roomid = @roomid";

                using (var cmd = new NpgsqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@roomid", roomID);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            if (reader.Read())
                            {
                                string statusHolder = (string)reader["status"];
                                room = new RoomDto(
                                    roomId:roomID,
                                    status: (string)reader["status"],
                                    pricePerNight: (decimal)reader["pricepernight"],
                                    capacity: (int)reader["capacity"],
                                    roomtypeid: (Guid)reader["roomtypeid"],
                                    bedNumber: (int)reader["bednumber"],
                                    beglongTo:(Guid)reader["belongto"],
                                    createdAt: (DateTime)reader["createdat"],
                                    isBlock: (bool)reader["isblock"],
                                    images:ImagesData.images(roomID,minioUrl),
                                    isDeleted:(bool)reader["isdeleted"]
                                    
                                );
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("this from getting user by id error {0}", ex.Message);
            return null;
        }

        return room;
    }

    public static bool isExist(Guid roomID)
    {
        bool isExist = false;
        try
        {
            using (var con = new NpgsqlConnection(connectionUr))
            {
                con.Open();
                string query = @"select * from rooms where roomid = @roomid";

                using (var cmd = new NpgsqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@roomid", roomID);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            if (reader.Read())
                            {
                                isExist = true;
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("this from getting user by id error {0}", ex.Message);
            isExist = false;
        }

        return isExist;
    }


    public static bool createRoom
    (
        RoomDto roomData
    )
    {
        bool isCration = false;
        try
        {
            using (var con = new NpgsqlConnection(connectionUr))
            {
                con.Open();
                string query = @"select * from fn_room_insert_new
                                (
                                @roomid_u::UUID,
                               @status::VARCHAR,
                               @pricePerNight_::NUMERIC,
                               @roomtypeid_ ,
                               @capacity_::INT ,
                               @bedNumber_::INT ,
                               @belongTo_
                                )";

                using (var cmd = new NpgsqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@roomid_u",roomData.roomId);
                    cmd.Parameters.AddWithValue("@status",roomData.status??"Available");
                    cmd.Parameters.AddWithValue("@pricePerNight_", roomData.pricePerNight);
                    cmd.Parameters.AddWithValue("@roomtypeid_", roomData.roomtypeid);
                    cmd.Parameters.AddWithValue("@capacity_", roomData.capacity);
                    cmd.Parameters.AddWithValue("@bedNumber_", roomData.bedNumber);
                    cmd.Parameters.AddWithValue("@belongTo_", roomData.beglongTo);
                    var reader = cmd.ExecuteScalar();
                    if (reader != null && int.TryParse(reader.ToString(),out int result))
                    {
                        isCration = result>0?true:false;
                    }
               
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("this from create room {0}", ex.Message);
           
        }

        return isCration;
    }
    
    
    public static bool updateRoom
    (
        RoomDto roomData
    )
    {
        bool isUpdate = false;
        try
        {
            using (var con = new NpgsqlConnection(connectionUr))
            {
                
                con.Open();
                string query = @"select * from fn_room_update_new
                                (
                               @roomid_::UUID,
                               @status::VARCHAR,
                               @pricePerNight_::NUMERIC,
                               @roomtypeid_::UUID ,
                               @capacity_::INT ,
                               @bedNumber_::INT
                                )";
                
                using (var cmd = new NpgsqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@roomid_", roomData.roomId);
                    cmd.Parameters.AddWithValue("@status", roomData.status);
                    cmd.Parameters.AddWithValue("@pricePerNight_", roomData.pricePerNight);
                    cmd.Parameters.AddWithValue("@roomtypeid_", roomData.roomtypeid);
                    cmd.Parameters.AddWithValue("@capacity_", roomData.capacity);
                    cmd.Parameters.AddWithValue("@bedNumber_", roomData.bedNumber);
                    var reader = cmd.ExecuteScalar();
                    if (reader != null && bool.TryParse(reader.ToString(),out bool result))
                    {
                        isUpdate = result;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("this from create room {0}", ex.Message);
           
        }

        return isUpdate;
    }

     public static List<RoomDto> getRoomByPage(int pageNumber = 1,
         int numberOfRoom = 20, Guid? userId=null 
     )
        {
            List<RoomDto> rooms = new List<RoomDto>();
            try
            {
                using (var con = new NpgsqlConnection(connectionUr))
                {
                    con.Open();
                    
                    string query="";
                    switch (userId==null)
                    {
                        case true:
                        {
                            
                           query = @"select * from  getRoomsByPage(@pagenumber,@limitnumber)";
 
                        }
                            break;
                        default:
                        {
                            query= @"select * from  getRoomsByPage(@pagenumber,@limitnumber,@belongId)";

                        }
                            break;
                        
                    }

                    using (var cmd = new NpgsqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@pagenumber", pageNumber <= 1 ? 1 : pageNumber - 1);
                        cmd.Parameters.AddWithValue("@limitnumber", numberOfRoom);
                        if (userId != null)
                            cmd.Parameters.AddWithValue("@belongId", userId);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    var roomid = (Guid)reader["roomid"];
                                var roomHolder =     new RoomDto(
                                        roomId:roomid,
                                        status: (string)reader["status"],
                                        pricePerNight: (decimal)reader["pricepernight"],
                                        capacity: (int)reader["capacity"],
                                        roomtypeid: (Guid)reader["roomtypeid"],
                                        bedNumber: (int)reader["bednumber"],
                                        beglongTo:(Guid)reader["belongto"],
                                        createdAt: (DateTime)reader["createdat"],
                                        isBlock: (bool)reader["isblock"],
                                        images:ImagesData.images(roomid,minioUrl),
                                        isDeleted:(bool)reader["isdeleted"]
                                    );

                                    rooms.Add(roomHolder);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("this from getting user by id error {0}", ex.Message);
                return null;
            }

            return rooms;
        }

    
    
  
    public static bool deleteRoom
    (
       Guid roomid,
       Guid userId
    )
    {
        bool isDeleted = false;
        try
        {
            using (var con = new NpgsqlConnection(connectionUr))
            {
                
                con.Open();
                string query = @"select * from room_delete
                                (
                               @roomid::UUID,
                               @userid::UUID 
                                )";
                
                using (var cmd = new NpgsqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@roomid", roomid);
                    cmd.Parameters.AddWithValue("@userid", userId);
                    var reader = cmd.ExecuteScalar();
                    if (reader != null && bool.TryParse(reader.ToString(),out bool result))
                    {
                        isDeleted = result;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("this from create room {0}", ex.Message);
           
        }

        return isDeleted;
    }
 
}