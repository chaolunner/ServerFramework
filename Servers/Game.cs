using System.Collections.Generic;
using System;
using Common;

namespace ServerFramework.Servers
{
    class Game
    {
        private Room room;
        private List<LockstepInputs> lockstepInputs;
        private Dictionary<int, Dictionary<int, UserInputs>> userInputsDict;

        public int Count
        {
            get
            {
                return room.ClientList.Count;
            }
        }

        public int TickId { get; set; } = 0;

        public Game(Room room)
        {
            this.room = room;
            lockstepInputs = new List<LockstepInputs>();
            userInputsDict = new Dictionary<int, Dictionary<int, UserInputs>>();
        }

        public void AddUserInputs(UserInputs userInputs)
        {
            try
            {
                lock (userInputsDict)
                {
                    int tickId = TickId;
                    if (!userInputsDict.ContainsKey(tickId))
                    {
                        userInputsDict.Add(tickId, new Dictionary<int, UserInputs>());
                    }
                    if (!userInputsDict[tickId].ContainsKey(userInputs.UserId))
                    {
                        userInputsDict[tickId].Add(userInputs.UserId, userInputs);
                    }
                }
            }
            catch (Exception e)
            {
                ConsoleUtility.WriteLine(e, ConsoleColor.Red);
            }
        }

        public LockstepInputs Next(Fix64 deltaTime)
        {
            int tickId = TickId;
            LockstepInputs inputs = new LockstepInputs();
            inputs.TickId = tickId;
            inputs.DeltaTime = deltaTime;
            if (userInputsDict.ContainsKey(tickId) && userInputsDict[tickId].Count > 0)
            {
                inputs.UserInputs = new UserInputs[userInputsDict[tickId].Count];
                userInputsDict[tickId].Values.CopyTo(inputs.UserInputs, 0);
            }
            lockstepInputs.Add(inputs);
            TickId++;
            return inputs;
        }

        public List<LockstepInputs> GetTimeline(int index)
        {
            return lockstepInputs.GetRange(index, lockstepInputs.Count - index);
        }

        public Client GetClient(int index)
        {
            if (index >= 0 && index < Count)
            {
                return room.ClientList[index];
            }
            return null;
        }
    }
}
