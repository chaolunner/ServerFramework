using System;

namespace Common
{
    public interface IInput { }

    [Serializable]
    public struct InputData
    {
        public string Name;
        public string Data;

        public InputData(string type, string data)
        {
            Name = type;
            Data = data;
        }
    }
}
