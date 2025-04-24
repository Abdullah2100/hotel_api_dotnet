using hotel_data.dto;
using Npgsql;

namespace hotel_data;

public class RoomTypeData
{
    static string connectionUr = clsConnnectionUrl.url;
    private static string minioUrl = clsConnnectionUrl.minIoConnectionUrl + "roomtype/";

    public static RoomTypeDto? getRoomType(Guid roomTypeId)
    {
        RoomTypeDto? roomType = null;
        try
        {
            using (var con = new NpgsqlConnection(connectionUr))
            {
                con.Open();
                string query = @"select * from roomtypes where roomtypeid = @roomtypeid";

                using (var cmd = new NpgsqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@roomtypeid", roomTypeId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                //if ((bool)reader["isdeleted"] == true) continue;

                                var imageHolder = ImagesData.image(roomTypeId,minioUrl);
                                roomType = new RoomTypeDto(
                                    roomTypeId: roomTypeId,
                                    roomTypeName: (string)reader["name"],
                                    createdBy: (Guid)reader["createdby"],
                                    createdAt: (DateTime)reader["createdat"],
                                    imagePath: imageHolder == null ? "" : imageHolder.path,
                                    isDeleted: (bool)reader["isdeleted"]
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

        return roomType;
    }

    public static RoomTypeDto? getRoomType(string name)
    {
        RoomTypeDto? roomType = null;
        try
        {
            using (var con = new NpgsqlConnection(connectionUr))
            {
                con.Open();
                string query = @"select * from roomtypes where name = @name";

                using (var cmd = new NpgsqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@name", name);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                var imageHolder = ImagesData.image(
                                    (Guid)reader["roomtypeid"],
                                    minioUrl);

                                roomType = new RoomTypeDto(
                                    roomTypeId: (Guid)reader["roomtypeid"],
                                    roomTypeName: name,
                                    createdBy: (Guid)reader["createdby"],
                                    createdAt: (DateTime)reader["createdat"],
                                    imagePath: imageHolder == null ? "" : imageHolder.path,
                                    isDeleted: (bool)reader["isdeleted"]
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

        return roomType;
    }


    public static bool createRoomType(RoomTypeDto roomData)
    {
        bool isCreated = false;
        try
        {
            using (var con = new NpgsqlConnection(connectionUr))
            {
                con.Open();
                string query =
                    "SELECT * FROM fn_roomtype_insert_new(@roomtype_id_holder::UUID,@name_s::VARCHAR,@createdby_s)";
                using (var cmd = new NpgsqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@roomtype_id_holder", roomData.roomTypeID);
                    cmd.Parameters.AddWithValue("@name_s", roomData.roomTypeName);
                    cmd.Parameters.AddWithValue("@createdby_s", roomData.createdBy);

                    var result = cmd.ExecuteScalar();
                    if (result != null && bool.TryParse(result.ToString(), out bool isComplate))
                    {
                        isCreated = isComplate;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("this the error from create roomtype {0}", ex.Message);
        }

        return isCreated;
    }


    public static bool updateRoomType(RoomTypeDto roomData)
    {
        bool isCreated = false;
        try
        {
            using (var con = new NpgsqlConnection(connectionUr))
            {
                con.Open();
                string query = "SELECT * FROM fn_roomtype_update_new( " +
                               "@name_s::VARCHAR ," +
                               "@roomtypeid_s::UUID," +
                               "@createdby_s::UUID); ";
                using (var cmd = new NpgsqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@name_s", roomData.roomTypeName);
                    cmd.Parameters.AddWithValue("@roomtypeid_s", roomData.roomTypeID);
                    cmd.Parameters.AddWithValue("@createdby_s", roomData.createdBy);

                    var result = cmd.ExecuteScalar();
                    if (result != null && bool.TryParse(result.ToString(), out bool isComplate))
                    {
                        isCreated = isComplate;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("this the error from update roomtype {0}", ex.Message);
        }

        return isCreated;
    }

    public static bool isExist(Guid ID)
    {
        bool isExist = false;
        try
        {
            using (var con = new NpgsqlConnection(connectionUr))
            {
                con.Open();
                string query = "SELECT count(*)>0 FROM RoomTypes WHERE RoomTypeID = id";
                using (var cmd = new NpgsqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("id", ID);
                    var result = cmd.ExecuteScalar();
                    if (result != null && bool.TryParse(result.ToString(), out bool isComplate))
                    {
                        isExist = isComplate;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("this the error from create roomtype {0}", ex.Message);
        }

        return isExist;
    }

    public static bool isExist(string name)
    {
        bool isExist = false;
        try
        {
            using (var con = new NpgsqlConnection(connectionUr))
            {
                con.Open();
                string query = "SELECT * FROM RoomTypes WHERE name = name";
                using (var cmd = new NpgsqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("name", name);
                    using (var result = cmd.ExecuteReader())
                    {
                        if (result.HasRows)
                        {
                            if(result.Rows!=0)
                                isExist = true;
                        }
                        
                    }
                   
                }
                
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("this the error from create roomtype {0}", ex.Message);
        }

        return isExist;
    }

    public static List<RoomTypeDto> getAll(bool isNotDeletion)
    {
        List<RoomTypeDto> roomtypes = new List<RoomTypeDto>();
        try
        {
            using (var con = new NpgsqlConnection(connectionUr))
            {
                con.Open();
                string query = queryForGetDeletedOrNoteDeltedRoomtype(isNotDeletion);

               
                using (var cmd = new NpgsqlCommand(query, con))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                //if ((bool)reader["isdeleted"] == true) continue;
                                var imageHolder = ImagesData.image(
                                    (Guid)reader["roomtypeid"],
                                    minioUrl);

                                var roomtypeHolder = new RoomTypeDto(
                                    roomTypeId: (Guid)reader["roomtypeid"],
                                    roomTypeName: (string)reader["name"],
                                    createdBy: (Guid)reader["createdby"],
                                    createdAt: (DateTime)reader["createdat"],
                                    imagePath:imageHolder==null?"":imageHolder.path,
                                    isDeleted: (bool)reader["isdeleted"]
                                );
                                roomtypes.Add(roomtypeHolder);
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

        return roomtypes;
    }

    private static string queryForGetDeletedOrNoteDeltedRoomtype(bool isNotDeletion)
    {
        switch (isNotDeletion)
        {
            case true:
            {
                return @"select * from roomtypes where isdeleted= FALSE";

            }
            default: return @"select * from roomtypes";
        }
    }
    public static bool deleteOrUnDelete(
        Guid id
    )
    {
        bool isDeleted = false;
        try
        {
            using (var connection = new NpgsqlConnection(connectionUr))
            {
                connection.Open();
                string query = @"DELETE FROM  roomtypes WHERE roomtypeid =@id;";

                using (var cmd = new NpgsqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                    isDeleted = true;
                }
            }

            return isDeleted;
        }
        catch (Exception ex)
        {
            Console.WriteLine("\nthis error from roomtype deleted {0} \n", ex.Message);
        }

        return isDeleted;
    }
    
}