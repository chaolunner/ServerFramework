using System.Collections.Generic;
using ServerFramework.Servers;
using System.Text;
using Common;

namespace ServerFramework.Controller
{
    class RoomController : BaseController
    {
        private Dictionary<int, Room> roomDict = new Dictionary<int, Room>();
        private Dictionary<Room, List<int>> userDict = new Dictionary<Room, List<int>>();
        private const string CreateRoomReturnStr = "{0},{1}";

        private void AddRoomOrUser(int userId, Room room)
        {
            if (!roomDict.ContainsKey(userId))
            {
                roomDict.Add(userId, room);
            }
            if (!userDict.ContainsKey(room))
            {
                userDict.Add(room, new List<int>(userId));
            }
            else if (!userDict[room].Contains(userId))
            {
                userDict[room].Add(userId);
            }
        }

        private void RemoveRoom(Room room)
        {
            if (userDict.ContainsKey(room))
            {
                foreach (var userId in userDict[room])
                {
                    RemoveUser(userId);
                }
                userDict.Remove(room);
            }
        }

        private void RemoveUser(int userId)
        {
            if (roomDict.ContainsKey(userId))
            {
                roomDict.Remove(userId);
            }
        }

        public string OnCreateRoom(Client client, string data)
        {
            int userId = this.GetController<UserController>().GetUserId(client);
            if (userId >= 0)
            {
                if (!roomDict.ContainsKey(userId))
                {
                    Room room = new Room(client);
                    room.OnEnd += OnEnd;
                    AddRoomOrUser(userId, room);
                    UpdateRoomList();
                }
                return string.Format(CreateRoomReturnStr, (int)ReturnCode.Success, this.GetController<UserController>().GetUserResult(client));
            }
            return ((int)ReturnCode.Fail).ToString();
        }

        public string OnListRoom(Client client, string data)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var room in userDict.Keys)
            {
                if (room.State == RoomState.Join)
                {
                    stringBuilder.Append(this.GetController<UserController>().GetUserResult(room.Owner) + VerticalBar);
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
            Room room = GetRoomByUserId(ownerId);
            if (room != null)
            {
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
                AddRoomOrUser(this.GetController<UserController>().GetUserId(client), room);
                UpdateRoomList();
                return stringBuilder.ToString();
            }
            return ((int)ReturnCode.Fail).ToString();
        }

        public string OnQuitRoom(Client client, string data)
        {
            int userId = int.Parse(data);
            Room room = GetRoomByUserId(userId);
            if (room != null) { room.Quit(client); }
            return data;
        }

        private void OnEnd(Room room, Client client)
        {
            int userId = this.GetController<UserController>().GetUserId(client);
            if (userId >= 0)
            {
                RemoveUser(userId);
                foreach (Client guest in room.ClientList)
                {
                    guest.Publish(RequestCode.QuitRoom, userId.ToString());
                }
            }
            if (room.Owner == client)
            {
                RemoveRoom(room);
                room.OnEnd -= OnEnd;
            }
            UpdateRoomList();
        }

        public void UpdateRoomList()
        {
            this.Publish(RequestCode.ListRooms, OnListRoom(null, null));
        }

        public Room GetRoomByUserId(int userId)
        {
            if (roomDict.ContainsKey(userId))
            {
                return roomDict[userId];
            }
            return null;
        }
    }
}
