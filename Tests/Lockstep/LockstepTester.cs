using Common;

namespace ServerFramework
{
    public class LockstepTester : Tester<LockstepTestCaseData>
    {
        private float time;
        private int tickId;

        public override void Restart()
        {
            base.Restart();
            time = 0;
            tickId = 0;
        }

        public bool Simulate(ref LockstepInputs lockstepInputs)
        {
            time -= (float)lockstepInputs.DeltaTime;
            if (time <= 0 && Next())
            {
                time = Current.NextSecond;
                lockstepInputs.TickId = tickId;
                lockstepInputs.UserInputs = new UserInputs[] { Current.UserInputs };
                ConsoleUtility.WriteLine(MessagePackUtility.ToJson(MessagePackUtility.Serialize(lockstepInputs)));
                tickId++;
                return true;
            }
            return false;
        }
    }
}
