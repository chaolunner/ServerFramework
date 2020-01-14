﻿using System.Collections.Generic;
using System;
using Common;

namespace ServerFramework.Servers
{
    class Game
    {
        private Room room;
        private List<LockstepInputs> lockstepInputs;
        private Dictionary<int, List<UserInputs>> userInputsDict;

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
            userInputsDict = new Dictionary<int, List<UserInputs>>();
        }

        public void AddUserInputs(UserInputs userInputs)
        {
            try
            {
                lock (userInputsDict)
                {
                    int tickId = TickId;
                    if (tickId - userInputs.TickId < 5)
                    {
                        if (!userInputsDict.ContainsKey(tickId))
                        {
                            userInputsDict.Add(tickId, new List<UserInputs>());
                        }
                        userInputsDict[tickId].Add(userInputs);
                    }
                }
            }
            catch (Exception e)
            {
                ConsoleUtility.WriteLine(e, ConsoleColor.Red);
            }
        }

        public LockstepInputs MoveNext(Fix64 deltaTime)
        {
            int tickId = TickId;
            var inputs = new LockstepInputs();
            var inputsDict = new Dictionary<int, Dictionary<int, List<UserInputs>>>();

            inputs.TickId = tickId;
            inputs.DeltaTime = deltaTime;

            if (userInputsDict.ContainsKey(tickId) && userInputsDict[tickId].Count > 0)
            {
                for (int i = 0; i < userInputsDict[tickId].Count; i++)
                {
                    var userInputs = userInputsDict[tickId][i];
                    var userId = userInputs.UserId;

                    if (!inputsDict.ContainsKey(userId))
                    {
                        inputsDict.Add(userId, new Dictionary<int, List<UserInputs>>());
                    }
                    if (!inputsDict[userId].ContainsKey(userInputs.Index))
                    {
                        inputsDict[userId].Add(userInputs.Index, new List<UserInputs>());
                    }
                    inputsDict[userId][userInputs.Index].Add(userInputs);
                }
            }

            var index = 0;
            var e = inputsDict.GetEnumerator();
            var userInputsList = new List<List<UserInputs>>();
            while (true)
            {
                userInputsList.Add(new List<UserInputs>());
                while (e.MoveNext())
                {
                    var count = e.Current.Value.Count;
                    if (index < count)
                    {
                        var keys = new int[count];
                        e.Current.Value.Keys.CopyTo(keys, 0);
                        userInputsList[index].AddRange(e.Current.Value[keys[index]]);
                    }
                }
                if (userInputsList[index].Count <= 0)
                {
                    userInputsList.RemoveAt(index);
                    break;
                }
                index++;
            }

            inputs.UserInputs = new UserInputs[userInputsList.Count][];
            for (int i = 0; i < userInputsList.Count; i++)
            {
                userInputsList[i].CopyTo(inputs.UserInputs[i]);
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
