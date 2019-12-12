namespace ServerFramework
{
    public class LockstepCase1 : TestCase<LockstepTestCaseData>
    {
        public LockstepCase1(bool loop = true, int times = 0, float nextSecond = 0.02f, int userId = 1, float vertical = 0, float horizontal = 0, bool jump = false)
        {
            isLoop = loop;
            loopTimes = times;
            testCaseData = new LockstepTestCaseData[]
            {
                LockstepTestCaseDataHelper.NewLockstepTestCaseData(nextSecond, userId, vertical, horizontal, jump),
            };
        }
    }
}
