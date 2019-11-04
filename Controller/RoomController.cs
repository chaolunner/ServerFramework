using System.Collections.Generic;
using ServerFramework.Servers;
using System.Text;
using Common;

namespace ServerFramework.Controller
{
    class RoomController : BaseController
    {
        public UserController UserController { get; } = ControllerManager.Default.GetController<UserController>();
        private Dictionary<int, Room> roomDict = new Dictionary<int, Room>();

        public string OnCreateRoom(Client client, string data)
        {
            int userId = UserController.GetUserId(client);
            if (userId >= 0)
            {
                if (!roomDict.ContainsKey(userId))
                {
                    Room room = new Room();
                    room.AddClient(client);
                    roomDict.Add(userId, room);
                }
                return ((int)ReturnCode.Success).ToString();
            }
            return ((int)ReturnCode.Fail).ToString();
        }

        public string OnListRoom(Client client, string data)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var kvp in roomDict)
            {
                if (kvp.Value.IsWaitingToJoin())
                {
                    stringBuilder.Append(UserController.GetUserResult(kvp.Value.GetOwner()) + VerticalBar);
                }
            }
            if (stringBuilder.Length > 0)
            {
                stringBuilder.Remove(stringBuilder.Length - 1, 1);
            }
            return stringBuilder.ToString();
        }

        public string OnJoinRoom(Client client, string data)
        {
            int userId = int.Parse(data);
            if (roomDict.ContainsKey(userId))
            {
                roomDict[userId].AddClient(client);
                return ((int)ReturnCode.Success).ToString();
            }
            return ((int)ReturnCode.Fail).ToString();
        }
    }
}
