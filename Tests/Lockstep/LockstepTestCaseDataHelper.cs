using System.Collections.Generic;
using Common;

namespace ServerFramework
{
    public static class LockstepTestCaseDataHelper
    {
        public static LockstepTestCaseData NewLockstepTestCaseData(float nextSecond, int userId, IInput[] inputs)
        {
            var inputData = new byte[inputs.Length][];
            for (int i = 0; i < inputData.Length; i++)
            {
                inputData[i] = MessagePackUtility.Serialize(inputs[i]);
            }
            return new LockstepTestCaseData()
            {
                NextSecond = nextSecond,
                UserInputs = new UserInputs()
                {
                    UserId = userId,
                    InputData = inputData,
                },
            };
        }

        public static LockstepTestCaseData NewLockstepTestCaseData(float nextSecond = 0.02f, int userId = 1, float vertical = 0, float horizontal = 0, bool jump = false)
        {
            var inputs = new IInput[(jump ? 2 : 1)];
            inputs[0] = new AxisInput() { Vertical = (Fix64)vertical, Horizontal = (Fix64)horizontal };
            if (jump) { inputs[1] = new KeyInput() { KeyCodes = new List<int>() { 32 } }; }
            return NewLockstepTestCaseData(nextSecond, userId, inputs);
        }
    }
}
