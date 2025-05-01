using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using hotel_data.dto;
using Npgsql;

namespace hotel_data
{
    public class PersonData
    {
        public static string connectionUrl = clsConnnectionUrl.url;

        public static bool createPerson(
            PersonDto personData
        )
        {
            bool isCreated = false;
            try
            {
                using (var connection = new NpgsqlConnection(connectionUrl))
                {
                    connection.Open();

                    string query = @"
                                INSERT INTO persons ( name , email, phone, address)
                                VALUES (@name,@email,@phone,@address);
                                RETURNING personid;";

                    using (var cmd = new NpgsqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@name", personData.name);
                        cmd.Parameters.AddWithValue("@phone", personData.phone);
                        cmd.Parameters.AddWithValue("@email", personData.email);
                        cmd.Parameters.AddWithValue("@address", personData.address);

                        cmd.ExecuteScalar();
                        isCreated = true;
                    }
                }

                return isCreated;
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nthis error from person create {0} \n", ex.Message);
            }

            return isCreated;
        }

        public static bool updatePerson(
            PersonDto personData
        )
        {
            bool isUpdate = false;
            try
            {
                using (var connection = new NpgsqlConnection(connectionUrl))
                {
                    connection.Open();

                    string query = @"
                                UPDATE INTO persons 
                                SET name = @name, phone = @phone,address=@address)
                                WHERE personid =@id;";

                    using (var cmd = new NpgsqlCommand(query, connection))
                    {
                        if (personData != null)
                        {
                            cmd.Parameters.AddWithValue("@id", personData.personID!);

                            cmd.Parameters.AddWithValue("@name", personData.name);
                            cmd.Parameters.AddWithValue("@phone", personData.phone);
                            cmd.Parameters.AddWithValue("@address", personData.address);
                        }

                        cmd.ExecuteNonQuery();
                        isUpdate = true;
                    }
                }

                return isUpdate;
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nthis error from person create {0} \n", ex.Message);
            }

            return isUpdate;
        }


        public static bool deletePerson(
            Guid id
        )
        {
            bool isDelelted = false;
            try
            {
                using (var connection = new NpgsqlConnection(connectionUrl))
                {
                    connection.Open();

                    string query = @" DELETE FROM  persons WHERE personid =@id;";

                    using (var cmd = new NpgsqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", id);

                        cmd.ExecuteNonQuery();
                        isDelelted = true;
                    }
                }

                return isDelelted;
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nthis error from person deleted {0} \n", ex.Message);
            }

            return isDelelted;
        }


        public static PersonDto? getPerson(
            Guid id
        )
        {
            PersonDto? person = null;
            try
            {
                using (var connection = new NpgsqlConnection(connectionUrl))
                {
                    connection.Open();

                    string query = @" SELECT * FROM  persons WHERE personid =@id;";

                    using (var cmd = new NpgsqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", id);

                        using (var result = cmd.ExecuteReader())
                        {
                            if (result.Read())
                            {
                                person = new PersonDto
                                (
                                    personID: id,
                                    name: (string)result["name"],
                                    email: (string)result["email"],
                                    phone: result["phone"] == DBNull.Value ? "" : (string)result["phone"],
                                    address: (string)result["address"]
                                );
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nthis error from person getPerson by id {0} \n", ex.Message);
            }

            return person;
        }


        public static bool isExist(
            Guid id
        )
        {
            bool isExist = false;
            try
            {
                using (var connection = new NpgsqlConnection(connectionUrl))
                {
                    connection.Open();

                    string query = @" SELECT * FROM  persons WHERE personid =@id;";

                    using (var cmd = new NpgsqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", id);

                        using (var result = cmd.ExecuteReader())
                        {
                            if (result.Read())
                                isExist = true;
                        }
                    }
                }

                return isExist;
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nthis error from person deleted {0} \n", ex.Message);
            }

            return isExist;
        }

        public static bool isExist(
            string phone
        )
        {
            bool isExist = false;
            try
            {
                using (var connection = new NpgsqlConnection(connectionUrl))
                {
                    connection.Open();

                    string query = @" SELECT * FROM  persons WHERE phone =@phone;";

                    using (var cmd = new NpgsqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@phone", phone);

                        using (var result = cmd.ExecuteReader())
                        {
                            if (result.HasRows)
                                isExist = true;
                        }
                    }
                }

                return isExist;
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nthis error from person deleted {0} \n", ex.Message);
            }

            return isExist;
        }


        public static bool isExist(
            string email,
            string phone
        )
        {
            bool isExist = false;
            try
            {
                using (var connection = new NpgsqlConnection(connectionUrl))
                {
                    connection.Open();

                    string query = @" SELECT * FROM isExistByEmailAndPhone(@email ,@phone);";

                    using (var cmd = new NpgsqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@email", email);
                        cmd.Parameters.AddWithValue("@phone", phone);

                        var result = cmd.ExecuteScalar();
                        if (result != null && bool.TryParse(result.ToString(), out bool isExist_))
                        {
                            isExist = isExist_;
                        }
                        // {
                        //     if (result.Read())
                        //         isExist = true;
                        // }
                    }
                }

                return isExist;
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nthis error from person deleted {0} \n", ex.Message);
            }

            return isExist;
        }
    }
}