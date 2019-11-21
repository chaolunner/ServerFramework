using System;

namespace Common
{
    [Serializable]
    public struct LockstepInputs
    {
        public int TickId;
        public UserInputs[] UserInputs;
    }
}
