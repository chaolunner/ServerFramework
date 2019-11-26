using System;

namespace Common
{
    [Serializable]
    public struct LockstepInputs
    {
        public int TickId;
        public Fix64 DeltaTime;
        public UserInputs[] UserInputs;
    }
}
