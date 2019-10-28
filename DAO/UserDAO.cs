using MySql.Data.MySqlClient;
using ServerFramework.Model;
using System;

namespace ServerFramework.DAO
{
    class UserDAO
    {
        private const string ValidateCommand = "select * from user where username = @username and password = @password";
        private const string UsernameParam = "username";
        private const string PasswordParam = "password";
        private const string IdParam = "id";

        public User GetUser(MySqlConnection conn, string username, string password)
        {
            MySqlDataReader reader = null;
            try
            {
                MySqlCommand cmd = new MySqlCommand(ValidateCommand, conn);
                cmd.Parameters.AddWithValue(UsernameParam, username);
                cmd.Parameters.AddWithValue(PasswordParam, password);
                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    int id = reader.GetInt32(IdParam);
                    User user = new User(id, username, password);
                    return user;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }
            return null;
        }
    }
}
