using MySql.Data.MySqlClient;
using ServerFramework.Model;
using System;
using Common;

namespace ServerFramework.DAO
{
    class UserDAO
    {
        private const string GetUserCommand = "select * from user where username = @username and password = @password";
        private const string HasUserCommand = "select * from user where username = @username";
        private const string AddUserCommand = "insert into user set username = @username, password = @password";
        private const string UsernameParam = "username";
        private const string PasswordParam = "password";
        private const string IdParam = "id";

        public User GetUser(MySqlConnection conn, string username, string password)
        {
            MySqlDataReader reader = null;
            try
            {
                MySqlCommand cmd = new MySqlCommand(GetUserCommand, conn);
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
                ConsoleUtility.WriteLine(e, ConsoleColor.Red);
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

        public bool HasUser(MySqlConnection conn, string username)
        {
            MySqlDataReader reader = null;
            try
            {
                MySqlCommand cmd = new MySqlCommand(HasUserCommand, conn);
                cmd.Parameters.AddWithValue(UsernameParam, username);
                reader = cmd.ExecuteReader();
                return reader.HasRows;
            }
            catch (Exception e)
            {
                ConsoleUtility.WriteLine(e, ConsoleColor.Red);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }
            return false;
        }

        public void AddUser(MySqlConnection conn, string username, string password)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand(AddUserCommand, conn);
                cmd.Parameters.AddWithValue(UsernameParam, username);
                cmd.Parameters.AddWithValue(PasswordParam, password);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                ConsoleUtility.WriteLine(e, ConsoleColor.Red);
            }
        }
    }
}
