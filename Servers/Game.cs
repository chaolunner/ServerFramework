using System.Collections.Generic;
using System;
using Common;

namespace ServerFramework.Servers
{
    class Game
    {
        private Room room;
        private DateTime currentTime;
        private Dictionary<int, Dictionary<int, Input>> inputDict;

        public int Count
        {
            get
            {
                return room.ClientList.Count;
            }
        }

        public int CurrentStep { get; set; } = 0;

        public float Timeout { get; set; } = 0.1f;

        public Game(Room room)
        {
            this.room = room;
            currentTime = DateTime.Now;
            inputDict = new Dictionary<int, Dictionary<int, Input>>();
        }

        public void AddInput(int userId, Input input)
        {
            if (!inputDict.ContainsKey(CurrentStep))
            {
                inputDict.Add(CurrentStep, new Dictionary<int, Input>());
            }
            if (inputDict[CurrentStep].ContainsKey(userId))
            {
                // Waiting for input from other players.
            }
            else
            {
                inputDict[CurrentStep].Add(userId, input);
            }
        }

        public bool Next(out int currentStep)
        {
            // Get input from all players, or get input from half of the players and time out.
            var result = inputDict.ContainsKey(CurrentStep) && (inputDict[CurrentStep].Count == Count || (inputDict[CurrentStep].Count / (float)Count >= 0.5f && (DateTime.Now - currentTime).TotalSeconds >= Timeout));
            currentStep = CurrentStep;
            if (result)
            {
                currentTime = DateTime.Now;
                CurrentStep++;
            }
            return result;
        }

        public Client GetClient(int index)
        {
            if (index >= 0 && index < Count)
            {
                return room.ClientList[index];
            }
            return null;
        }

        public Input GetInputByUserId(int userId, int currentStep)
        {
            if (inputDict.ContainsKey(currentStep))
            {
                if (inputDict[currentStep].ContainsKey(userId))
                {
                    return inputDict[currentStep][userId];
                }
            }
            return Input.None;
        }
    }
}
