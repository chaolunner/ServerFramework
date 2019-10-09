using System;
using MySql.Data.MySqlClient;

namespace ServerFramework.Tool
{
    class ConnHelper
    {
        public const string CONNECTIONSTRING = "datasource=127.0.0.1;port=3306;database=serverframework;user=root;pwd=1234";

        public static MySqlConnection Connect()
        {
            MySqlConnection conn = new MySqlConnection(CONNECTIONSTRING);
            try
            {
                conn.Open();
                return conn;
            }
            catch (Exception e)
            {
                Console.WriteLine("An exception occurred while connecting to MySQL database: " + e);
                return null;
            }
        }

        public static void Disconnect(MySqlConnection conn)
        {
            if (conn != null)
            {
                conn.Close();
            }
        }
    }
}
