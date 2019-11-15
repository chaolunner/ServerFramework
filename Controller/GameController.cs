using ServerFramework.Servers;
using Common;

namespace ServerFramework.Controller
{
    class GameController : BaseController
    {
        public string OnStartGame(Client client, string data)
        {
            this.Publish(RequestCode.StartGame, data, client);
            return data;
        }
    }
}
