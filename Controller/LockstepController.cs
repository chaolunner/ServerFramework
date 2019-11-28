using System.Collections.Generic;
using ServerFramework.Servers;
using System;
using Common;

namespace ServerFramework.Controller
{
    class LockstepController : BaseController
    {
        private DateTime currentTime;
        private Dictionary<Room, Game> gameDict = new Dictionary<Room, Game>();

        public string OnInput(Client client, byte[] dataBytes)
        {
            int userId = this.GetController<UserController>().GetUserId(client);
            Room room = this.GetController<RoomController>().GetRoomByUserId(userId);
            if (room != null)
            {
                if (!gameDict.ContainsKey(room))
                {
                    room.OnEnd += RemoveRoom;
                    gameDict.Add(room, new Game(room));
                }
                Game game = gameDict[room];
                UserInputs userInputs = MessagePackUtility.Deserialize<UserInputs>(dataBytes);
                userInputs.UserId = userId;
                game.AddUserInputs(userInputs);
                return ((int)ReturnCode.Success).ToString();
            }
            return ((int)ReturnCode.Fail).ToString();
        }

        public string OnTimeline(Client client, string data)
        {
            int userId = this.GetController<UserController>().GetUserId(client);
            Room room = this.GetController<RoomController>().GetRoomByUserId(userId);
            if (room != null)
            {
                Game game = gameDict[room];
                foreach (var lockstepInputs in game.GetTimeline(int.Parse(data)))
                {
                    client.Publish(RequestCode.Lockstep, MessagePackUtility.Serialize(lockstepInputs));
                }
                return ((int)ReturnCode.Success).ToString();
            }
            return ((int)ReturnCode.Fail).ToString();
        }

        public override void Update()
        {
            base.Update();

            foreach (var kvp in gameDict)
            {
                Room room = kvp.Key;
                Game game = kvp.Value;
                LockstepInputs lockstepInputs = game.Next((Fix64)(DateTime.Now - currentTime).TotalSeconds);
                room.Publish(RequestCode.Lockstep, MessagePackUtility.Serialize(lockstepInputs));
            }
            currentTime = DateTime.Now;
        }

        private void RemoveRoom(Room room, Client client)
        {
            if (gameDict.ContainsKey(room))
            {
                gameDict.Remove(room);
            }
            room.OnEnd -= RemoveRoom;
        }
    }
}
