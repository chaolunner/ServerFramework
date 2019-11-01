using System.Collections.Generic;
using ServerFramework.Servers;
using Common;

namespace ServerFramework.Controller
{
    class RoomController : BaseController
    {
        private List<Room> roomList = new List<Room>();

        public string CreateRoom(Client client, string data)
        {
            Room room = new Room();
            room.AddClient(client);
            roomList.Add(room);
            return ((int)ReturnCode.Success).ToString();
        }
    }
}
