using System.Collections.Generic;
using ServerFramework.Servers;
using System.Text;
using Common;

namespace ServerFramework.Controller
{
    class RoomController : BaseController
    {
        private List<Room> roomList = new List<Room>();

        public string OnCreateRoom(Client client, string data)
        {
            Room room = new Room();
            room.AddClient(client);
            roomList.Add(room);
            return ((int)ReturnCode.Success).ToString();
        }

        public string OnListRoom(Client client, string data)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (Room room in roomList)
            {
                if (room.IsWaitingToJoin())
                {
                    stringBuilder.Append(ControllerManager.Default.GetController<UserController>().GetUserResult(room.GetOwner()) + VerticalBar);
                }
            }
            if (stringBuilder.Length > 0)
            {
                stringBuilder.Remove(stringBuilder.Length - 1, 1);
            }
            return stringBuilder.ToString();
        }
    }
}
