﻿using System.Collections.Generic;
using ServerFramework.Servers;
using System;
using Common;

namespace ServerFramework.Controller
{
    class LockstepController : BaseController
    {
        private DateTime currentTime;
        private Dictionary<Room, Game> gameDict = new Dictionary<Room, Game>();
        private const int InvalidTime = 5;
        private const string SyncTimelineTips = "Synchronizing timeline... \n Client id [{0}], current server tick id [{1}], current client tick id [{2}].";

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
                if (game.TickId - userInputs.TickId < InvalidTime)
                {
                    game.AddUserInputs(userInputs);
                }
                else
                {
                    var timeline = game.GetTimeline(userId, userInputs.TickId);
                    if (timeline != null)
                    {
                        for (int i = 0; i < timeline.Count; i++)
                        {
                            client.Publish(RequestCode.Lockstep, MessagePackUtility.Serialize(timeline[i]));
                        }
                        ConsoleUtility.WriteLine(string.Format(SyncTimelineTips, userId, game.TickId, userInputs.TickId), ConsoleColor.White);
                    }
                }
                return ((int)ReturnCode.Success).ToString();
            }
            return ((int)ReturnCode.Fail).ToString();
        }

        public string OnAnalysis(Client client, string data)
        {
            int userId = this.GetController<UserController>().GetUserId(client);
            Room room = this.GetController<RoomController>().GetRoomByUserId(userId);
            if (room != null)
            {
                room.Publish(RequestCode.LockstepAnalysis, data, client);
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

                LockstepTester lockstepTester = TestManager.Default.Get<LockstepTester>();
                if (lockstepTester != null)
                {
                    var lockstepInputs = new LockstepInputs();
                    if (lockstepTester.Simulate(ref lockstepInputs))
                    {
                        room.Publish(RequestCode.Lockstep, MessagePackUtility.Serialize(lockstepInputs));
                    }
                }
                else
                {
                    var lockstepInputs = game.MoveNext((Fix64)(DateTime.Now - currentTime).TotalSeconds);
                    room.Publish(RequestCode.Lockstep, MessagePackUtility.Serialize(lockstepInputs));
                }
            }
            currentTime = DateTime.Now;
        }

        private void RemoveRoom(Room room, Client client)
        {
            LockstepTester lockstepTester = TestManager.Default.Get<LockstepTester>();
            if (lockstepTester != null)
            {
                lockstepTester.Restart();
            }
            if (gameDict.ContainsKey(room))
            {
                gameDict.Remove(room);
            }
            room.OnEnd -= RemoveRoom;
        }
    }
}
