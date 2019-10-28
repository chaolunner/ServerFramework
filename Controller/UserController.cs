using ServerFramework.Servers;
using ServerFramework.Model;
using ServerFramework.DAO;
using Common;

namespace ServerFramework.Controller
{
    class UserController : BaseController
    {
        private UserDAO userDAO = new UserDAO();

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
                return ((int)ReturnCode.Success).ToString();
            }
        }
    }
}
