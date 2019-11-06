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
                    stringBuilder.Append(this.GetController<UserController>().GetUserResult(kvp.Value.GetOwner()) + VerticalBar);
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
                Room room = roomDict[userId];
                Client owner = room.GetOwner();
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append((int)ReturnCode.Success);
                stringBuilder.Append(VerticalBar + this.GetController<UserController>().GetUserResult(client) + Separator + false);
                foreach (Client guest in room.ClientList)
                {
                    stringBuilder.Append(VerticalBar + this.GetController<UserController>().GetUserResult(guest) + Separator + (guest == owner));
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
            int userId = this.GetController<UserController>().GetUserId(client);
            if (userId >= 0 && roomDict.ContainsKey(userId))
            {
                roomDict[userId].Quit(client);
            }
            return userId.ToString();
        }

        private void OnEnd(Room room, Client client)
        {
            foreach (Client guest in room.ClientList)
            {
                int guestId = this.GetController<UserController>().GetUserId(guest);
                if (guestId >= 0)
                {
                    guest.Publish(RequestCode.QuitRoom, guestId.ToString());
                }
            }
            Client roomOwner = room.GetOwner() ?? client;
            int roomOwnerId = this.GetController<UserController>().GetUserId(roomOwner);
            if (roomOwnerId >= 0 && roomDict.ContainsKey(roomOwnerId))
            {
                roomDict.Remove(roomOwnerId);
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
