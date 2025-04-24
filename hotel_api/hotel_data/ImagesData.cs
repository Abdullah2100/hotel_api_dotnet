using hotel_data.dto;
using Npgsql;

namespace hotel_data;

public class ImagesData
{
    static string connectionUr = clsConnnectionUrl.url;

    public static bool createImages(ImagesTbDto image)
    {
        bool isCreated = false;
        try
        {
            using (var con = new NpgsqlConnection(connectionUr))
            {
                con.Open();
                string query = @"INSERT INTO images(name,belongto,isthumnail) 
                                 VALUES (@name,@belongto,@isthumnail)";
                using (var cmd = new NpgsqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@name", image.path);
                    cmd.Parameters.AddWithValue("@belongto", image.belongTo);
                    if (image.isThumnail != null)
                    {
                    cmd.Parameters.AddWithValue("@isthumnail", image.isThumnail);
                    }else cmd.Parameters.AddWithValue("@isthumnail", DBNull.Value);
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
            Console.WriteLine("this the error from create images {0}", ex.Message);
        }

        return isCreated;
    }


    public static bool updateImages(ImagesTbDto image)
    {
        bool isCreated = false;
        try
        {
            if (image.id == null) return false;
            using (var con = new NpgsqlConnection(connectionUr))
            {
                con.Open();
                string query = "UPDATE images SET name =@image_path WHERE imageid = @ID";
                using (var cmd = new NpgsqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@image_path", image.path);
                    cmd.Parameters.AddWithValue("@ID", image.id);
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

    public static ImagesTbDto? image(Guid? belongto)
    {
        if (belongto == null) return null;
        ImagesTbDto? image = null;
        try
        {
            using (var con = new NpgsqlConnection(connectionUr))
            {
                con.Open();
                string query = "SELECT  * FROM images where belongto = @belongto LIMIT 1";
                using (var cmd = new NpgsqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@belongto", belongto);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            if (reader.Read())
                            {
                                image = new ImagesTbDto(
                                    imagePathId: (Guid)reader["imageid"],
                                    imagePath: (string)reader["name"],
                                    belongTo: (Guid)reader["belongto"]
                                    ,isThumnail:(bool)reader["isthumnail"]
                                );
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("this the error from get image by path {0}", ex.Message);
        }

        return image;
    }

    public static ImagesTbDto? image(Guid id,string? minioUrl=null)
    {
        ImagesTbDto? image = null;
        try
        {
            using (var con = new NpgsqlConnection(connectionUr))
            {
                con.Open();
                string query = "SELECT * FROM images where belongto = @id";
                using (var cmd = new NpgsqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            if (reader.Read())
                            {
                                string imageHolder = minioUrl!=null ? minioUrl+((string)reader["name"]) : "";
                                image = new ImagesTbDto(
                                    imagePathId: (Guid)reader["imageid"],
                                    imagePath:imageHolder,
                                    belongTo: (Guid)reader["belongto"],
                                     isThumnail:reader["isthumnail"]==DBNull.Value?false: (bool)reader["isthumnail"]

                                );
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("this the error from get image by path {0}", ex.Message);
        }

        return image;
    }
    public static List<ImagesTbDto> images(Guid? belongto,string? minioEndPoint=null)
    {
        if (belongto == null) return null;
        List<ImagesTbDto>? images = new List<ImagesTbDto>();
        try
        {
            using (var con = new NpgsqlConnection(connectionUr))
            {
                con.Open();
                string query = "SELECT * FROM images WHERE belongto = @belongto";
                using (var cmd = new NpgsqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@belongto", belongto);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                var imageHolder = new ImagesTbDto(
                                    imagePathId: (Guid)reader["imageid"],
                                    imagePath:(minioEndPoint??"")+ (string)reader["name"],
                                    belongTo: (Guid)reader["belongto"]
                                    , isThumnail:reader["isthumnail"]==DBNull.Value?false: (bool)reader["isthumnail"]

                                );
                                images.Add(imageHolder);
                               
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("this the error from get image by path {0}", ex.Message);
        }

        return images;
    }


    public static bool isExist(string path)
    {
        bool isExist = false;
        try
        {
            using (var con = new NpgsqlConnection(connectionUr))
            {
                con.Open();
                string query = "SELECT count(*)>0 FROM images  WHERE  path= path";
                using (var cmd = new NpgsqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("path", path);
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
            Console.WriteLine("this the error from get image by path {0}", ex.Message);
        }

        return isExist;
    }

    public static bool deleteImage(Guid belongTo)
    {
        bool isDeleted = false;
        try
        {
            using (var con = new NpgsqlConnection(connectionUr))
            {
                con.Open();
                string query = "DELETE FROM images  WHERE belongto= @belongTo";
                using (var cmd = new NpgsqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@belongTo", belongTo);
                    cmd.ExecuteNonQuery();
                    isDeleted = true;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("this the error from get image by path {0}", ex.Message);
        }

        return isDeleted;
    }
    
    public static bool deleteImageById(Guid id)
    {
        bool isDeleted = false;
        try
        {
            using (var con = new NpgsqlConnection(connectionUr))
            {
                con.Open();
                string query = "DELETE FROM images  WHERE imageid= @id";
                using (var cmd = new NpgsqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                    isDeleted = true;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("this the error from get image by path {0}", ex.Message);
        }

        return isDeleted;
    }

}