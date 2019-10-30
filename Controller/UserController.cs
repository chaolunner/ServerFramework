using ServerFramework.Servers;
using ServerFramework.Model;
using ServerFramework.DAO;
using Common;

namespace ServerFramework.Controller
{
    class UserController : BaseController
    {
        private UserDAO userDAO = new UserDAO();
        private ResultDAO resultDAO = new ResultDAO();
        private const string LoginSuccessStr = "{0},{1},{2},{3}";

        public string OnLogin(Client client, string data)
        {
            string[] strs = data.Split(Separator);
            User user = userDAO.GetUser(client.MySqlConn, strs[0], strs[1]);
            if (user == null)
            {
                return ((int)ReturnCode.Fail).ToString();
            }
            else
            {
                Result result = resultDAO.GetResultByUserId(client.MySqlConn, user.Id);
                return string.Format(LoginSuccessStr, ((int)ReturnCode.Success).ToString(), user.Username, result.TotalCount, result.WinCount);
            }
        }

        public string OnRegister(Client client, string data)
        {
            string[] strs = data.Split(Separator);
            string username = strs[0];
            string password = strs[1];
            bool hasUser = userDAO.HasUser(client.MySqlConn, username);
            if (hasUser)
            {
                return ((int)ReturnCode.Fail).ToString();
            }
            userDAO.AddUser(client.MySqlConn, username, password);
            return ((int)ReturnCode.Success).ToString();
        }
    }
}
