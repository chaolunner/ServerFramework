using System.Collections.Generic;
using ServerFramework.Extensions;
using ServerFramework.Servers;
using System.Text;
using Common;

namespace ServerFramework.Controller
{
    class RoomController : BaseController
    {
        private Dictionary<int, Room> roomDict = new Dictionary<int, Room>();
        private const string CreateRoomReturnStr = "{0},{1}";

        public string OnCreateRoom(Client client, string data)
        {
            int userId = this.GetController<UserController>().GetUserId(client);
            if (userId >= 0)
            {
                if (!roomDict.ContainsKey(userId))
                {
                    Room room = new Room(client);
                    room.OnEnd += OnEnd;
                    roomDict.Add(userId, room);
                    UpdateRoomList();
                }
                return string.Format(CreateRoomReturnStr, (int)ReturnCode.Success, this.GetController<UserController>().GetUserResult(client));
            }
            return ((int)ReturnCode.Fail).ToString();
        }

        public string OnListRoom(Client client, string data)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var kvp in roomDict)
            {
                if (kvp.Value.State == RoomState.Join)
                {
                    stringBuilder.Append(this.GetController<UserController>().GetUserResult(kvp.Value.Owner) + VerticalBar);
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
            int ownerId = int.Parse(data);
            if (ownerId >= 0 && roomDict.ContainsKey(ownerId))
            {
                Room room = roomDict[ownerId];
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append((int)ReturnCode.Success);
                stringBuilder.Append(VerticalBar + this.GetController<UserController>().GetUserResult(client) + Separator + false);
                foreach (Client guest in room.ClientList)
                {
                    stringBuilder.Append(VerticalBar + this.GetController<UserController>().GetUserResult(guest) + Separator + (guest == room.Owner));
                }
                foreach (Client guest in room.ClientList)
                {
                    guest.Publish(RequestCode.JoinRoom, stringBuilder.ToString());
                }
                room.AddClient(client);
                room.State = RoomState.Ready;
                UpdateRoomList();
                return stringBuilder.ToString();
            }
            return ((int)ReturnCode.Fail).ToString();
        }

        public string OnQuitRoom(Client client, string data)
        {
            int ownerId = int.Parse(data);
            if (ownerId >= 0 && roomDict.ContainsKey(ownerId))
            {
                roomDict[ownerId].Quit(client);
            }
            return this.GetController<UserController>().GetUserId(client).ToString();
        }

        private void OnEnd(Room room, Client client)
        {
            int userId = this.GetController<UserController>().GetUserId(client);
            if (userId >= 0)
            {
                foreach (Client guest in room.ClientList)
                {
                    guest.Publish(RequestCode.QuitRoom, userId.ToString());
                }
            }
            int ownerId = this.GetController<UserController>().GetUserId(client);
            if (ownerId >= 0 && roomDict.ContainsKey(ownerId))
            {
                roomDict.Remove(ownerId);
                room.OnEnd -= OnEnd;
            }
            UpdateRoomList();
        }

        public void UpdateRoomList()
        {
            this.Publish(RequestCode.ListRooms, OnListRoom(null, null));
        }
    }
}
