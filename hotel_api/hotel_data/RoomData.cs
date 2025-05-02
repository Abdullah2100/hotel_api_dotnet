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
                string query = @"SELECT  * FROM getRoomsByID(@roomId_ )";

                using (var cmd = new NpgsqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@roomId_", roomID);
                      using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            if (reader.Read())
                            {
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
                                    isDeleted:(bool)reader["isdeleted"],
                                    location:(string)reader["place"],
                                    longitude:(decimal)reader["longitude"],
                                    latitude:(decimal)reader["latitude"]
                                
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


    public static Guid? createRoom
    (
        RoomDto roomData
    )
    {
        Guid? roomId = null;
        try
        {
            using (var con = new NpgsqlConnection(connectionUr))
            {
                con.Open();
                string query = @"select * from fn_room_insert_new
                                (
                                @roomid_::UUID,
                               @status::VARCHAR,
                               @pricePerNight_::NUMERIC,
                               @roomtypeid_ ,
                               @capacity_::INT ,
                               @bedNumber_::INT ,
                               @belongTo_ ,
                               ST_SetSRID(ST_MakePoint(@latitude ,@longitude),4326),
                               @place
                                )";

                using (var cmd = new NpgsqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@roomid_",roomData.roomId);
                    cmd.Parameters.AddWithValue("@status",roomData.status??"Available");
                    cmd.Parameters.AddWithValue("@pricePerNight_", roomData.pricePerNight);
                    cmd.Parameters.AddWithValue("@roomtypeid_", roomData.roomtypeid);
                    cmd.Parameters.AddWithValue("@capacity_", roomData.capacity);
                    cmd.Parameters.AddWithValue("@bedNumber_", roomData.bedNumber);
                    cmd.Parameters.AddWithValue("@belongTo_", roomData.beglongTo);
                    
                    if (roomData.longitude != null)
                    {
                        cmd.Parameters.AddWithValue("@longitude", roomData.longitude);
                    }
                    else cmd.Parameters.AddWithValue("@longitude", DBNull.Value); 
                    
                    if (roomData.latitude != null)
                    {
                        cmd.Parameters.AddWithValue("@latitude", roomData.latitude);
                    }
                    else cmd.Parameters.AddWithValue("@latitude", DBNull.Value); 
                    
                    if (roomData.location != null)
                    {
                        cmd.Parameters.AddWithValue("@place", roomData.location);
                    }
                    else cmd.Parameters.AddWithValue("@place", DBNull.Value); 
                    
                    var reader = cmd.ExecuteScalar();
                    if (reader != null && Guid.TryParse(reader.ToString(),out Guid result))
                    {
                        roomId = result;
                    }
               
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("this from create room {0}", ex.Message);
           
        }

        return roomId;
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
                               @bedNumber_::INT,
                                @belongTo_,
                                ST_SetSRID(ST_MakePoint(@longitude,@latitude),4326) ,
                                @place_ 
                                )";
                
                using (var cmd = new NpgsqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@roomid_", roomData.roomId);
                    if(roomData.status==null||roomData.status=="") cmd.Parameters.AddWithValue("@status", DBNull.Value);
                    else cmd.Parameters.AddWithValue("@status", roomData.status);
                    
                   if(roomData.pricePerNight==0) cmd.Parameters.AddWithValue("@pricePerNight_", DBNull.Value);
                   else  cmd.Parameters.AddWithValue("@pricePerNight_", roomData.pricePerNight);
                   
                    
                    if(roomData.roomtypeid==Guid.Empty)cmd.Parameters.AddWithValue("@roomtypeid_", DBNull.Value);
                    else  cmd.Parameters.AddWithValue("@roomtypeid_", roomData.roomtypeid);
                    
                    if(roomData.capacity==0) cmd.Parameters.AddWithValue("@capacity_", DBNull.Value);
                     else cmd.Parameters.AddWithValue("@capacity_", roomData.capacity);
                     
                    if(roomData.bedNumber==0)cmd.Parameters.AddWithValue("@bedNumber_",DBNull.Value );
                    else  cmd.Parameters.AddWithValue("@bedNumber_", roomData.bedNumber);
                    
                    cmd.Parameters.AddWithValue("@belongTo_", roomData.beglongTo);
                    
                    if(roomData.latitude==null)cmd.Parameters.AddWithValue("@latitude",DBNull.Value );
                    else  cmd.Parameters.AddWithValue("@latitude", roomData.latitude);
                    
                    if(roomData.longitude==null)cmd.Parameters.AddWithValue("@longitude",DBNull.Value );
                    else  cmd.Parameters.AddWithValue("@longitude", roomData.longitude);
                    
                    if(roomData.location==null)cmd.Parameters.AddWithValue("@place_",DBNull.Value );
                    else  cmd.Parameters.AddWithValue("@place_", roomData.location);
                    
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
                    
                    string  query= @"select * from  getRoomsByPage(@pagenumber,@limitnumber,@belongId)";


                    using (var cmd = new NpgsqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@pagenumber", pageNumber <= 1 ? 1 : pageNumber - 1);
                        cmd.Parameters.AddWithValue("@limitnumber", numberOfRoom);
                      
                        if (userId != null)
                            cmd.Parameters.AddWithValue("@belongId", userId);
                         else    cmd.Parameters.AddWithValue("@belongId", DBNull.Value);

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
                                        isDeleted:(bool)reader["isdeleted"],
                                        latitude:(decimal)reader["latitude"],
                                        longitude:(decimal)reader["longitude"],
                                        location:(string)reader["place"]
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

   
        public static List<RoomDto> getRoomByPage(
            Guid userId
            ,int pageNumber = 1,
         int numberOfRoom = 20
     )
        {
            List<RoomDto> rooms = new List<RoomDto>();
            try
            {
                using (var con = new NpgsqlConnection(connectionUr))
                {
                    con.Open();
                    
                    string  query= @"select * from  getRoomsByPage_A(@pagenumber,@limitnumber,@belongId)";


                    using (var cmd = new NpgsqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@pagenumber", pageNumber <= 1 ? 1 : pageNumber - 1);
                        cmd.Parameters.AddWithValue("@limitnumber", numberOfRoom);
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
                                        isDeleted:(bool)reader["isdeleted"],
                                        latitude:(decimal)reader["latitude"],
                                        longitude:(decimal)reader["longitude"],
                                        location:(string)reader["place"]
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
                string query = @"select * from  blockRoom
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