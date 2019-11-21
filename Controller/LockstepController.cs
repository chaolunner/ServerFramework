using System.Collections.Generic;
using ServerFramework.Servers;
using Common;

namespace ServerFramework.Controller
{
    class LockstepController : BaseController
    {
        private Dictionary<Room, Game> gameDict = new Dictionary<Room, Game>();

        public string OnInput(Client client, string data)
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
                UserInputs userInputs = JsonUtility.FromJson<UserInputs>(data);
                userInputs.UserId = userId;
                game.AddUserInputs(userInputs);
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
                LockstepInputs lockstepInputs = game.Next();
                string data = JsonUtility.ToJson(lockstepInputs);
                room.Publish(RequestCode.Lockstep, data);
            }
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
