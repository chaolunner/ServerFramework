using System.Collections.Generic;
using System;
using Common;

namespace ServerFramework.Servers
{
    class Game
    {
        private Room room;
        private Dictionary<int, Dictionary<int, UserInputs>> inputDict;
        private const string NextFailedError = "User Inputs Count [{0}] \n {1}";

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
            inputDict = new Dictionary<int, Dictionary<int, UserInputs>>();
        }

        public void AddUserInputs(UserInputs userInputs)
        {
            try
            {
                lock (inputDict)
                {
                    int tickId = TickId;
                    if (!inputDict.ContainsKey(tickId))
                    {
                        inputDict.Add(tickId, new Dictionary<int, UserInputs>());
                    }
                    if (!inputDict[tickId].ContainsKey(userInputs.UserId))
                    {
                        inputDict[tickId].Add(userInputs.UserId, userInputs);
                    }
                }
            }
            catch (Exception e)
            {
                ConsoleUtility.WriteLine(e, ConsoleColor.Red);
            }
        }

        public LockstepInputs Next()
        {
            int tickId = TickId;
            LockstepInputs lockstepInputs = new LockstepInputs();
            lockstepInputs.TickId = tickId;
            try
            {
                if (inputDict.ContainsKey(tickId) && inputDict[tickId].Count > 0)
                {
                    lockstepInputs.UserInputs = new UserInputs[inputDict[tickId].Count];
                    inputDict[tickId].Values.CopyTo(lockstepInputs.UserInputs, 0);
                }
            }
            catch (Exception e)
            {
                ConsoleUtility.WriteLine(string.Format(NextFailedError, inputDict[tickId].Count, e), ConsoleColor.Red);
            }
            TickId++;
            return lockstepInputs;
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
