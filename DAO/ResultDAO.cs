using MySql.Data.MySqlClient;
using ServerFramework.Model;
using System;
using Common;

namespace ServerFramework.DAO
{
    class ResultDAO
    {
        private const string GetResultByUserIdCommand = "select * from result where userid = @userid";
        private const string UserIdParam = "userid";
        private const string IdParam = "id";
        private const string TotalCountParam = "totalCount";
        private const string WinCountParam = "winCount";

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
    }
}
