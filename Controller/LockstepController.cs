using System.Collections.Generic;
using ServerFramework.Servers;
using Newtonsoft.Json;
using System.Text;
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
                    gameDict.Add(room, new Game(room));
                }
                Game game = gameDict[room];
                Input input = JsonConvert.DeserializeObject<Input>(data);
                if (game.CurrentStep == input.Step)
                {
                    game.AddInput(userId, input);
                }
            }
            return ((int)ReturnCode.Fail).ToString();
        }

        public override void Update()
        {
            base.Update();

            foreach (var game in gameDict.Values)
            {
                int currentStep = 0;
                while (game.Next(out currentStep))
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    for (int i = 0; i < game.Count; i++)
                    {
                        int userId = this.GetController<UserController>().GetUserId(game.GetClient(i));
                        stringBuilder.Append(userId);
                        stringBuilder.Append(VerticalBar);
                        stringBuilder.Append(JsonConvert.SerializeObject(game.GetInputByUserId(userId, currentStep)));
                        stringBuilder.Append(VerticalBar);
                    }
                    if (stringBuilder.Length > 0)
                    {
                        stringBuilder.Remove(stringBuilder.Length - 1, 1);
                    }
                    this.Publish(RequestCode.Lockstep, stringBuilder.ToString());
                }
            }
        }
    }
}
