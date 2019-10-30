using MySql.Data.MySqlClient;
using ServerFramework.Model;
using System;

namespace ServerFramework.DAO
{
    class ResultDAO
    {
        private const string GetResultByUserIdCommand = "select * from result where userid = @userid";
        private const string UserIdParam = "userid";
        private const string IdParam = "id";
        private const string TotalCountParam = "id";
        private const string WinCountParam = "id";

        public Result GetResultByUserId(MySqlConnection conn, int userId)
        {
            MySqlDataReader reader = null;
            try
            {
                MySqlCommand cmd = new MySqlCommand(GetResultByUserIdCommand, conn);
                cmd.Parameters.AddWithValue(UserIdParam, userId);
                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    int id = reader.GetInt32(IdParam);
                    int totalCount = reader.GetInt32(TotalCountParam);
                    int winCount = reader.GetInt32(WinCountParam);
                    Result result = new Result(id, userId, totalCount, winCount);
                    return result;
                }
                else
                {
                    Result result = new Result(-1, userId, 0, 0);
                    return result;
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
