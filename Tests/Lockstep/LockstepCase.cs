namespace ServerFramework
{
    public class LockstepCase1 : TestCase<LockstepTestCaseData>
    {
        public LockstepCase1()
        {
            isLoop = true;
            loopTimes = 500;
            testCaseData = new LockstepTestCaseData[]
            {
                LockstepTestCaseDataHelper.NewLockstepTestCaseData(),
            };
        }
    }

    public class LockstepCase2 : TestCase<LockstepTestCaseData>
    {
        public LockstepCase2()
        {
            testCaseData = new LockstepTestCaseData[]
            {
                LockstepTestCaseDataHelper.NewLockstepTestCaseData(10f, 1, 1),
            };
        }
    }

    public class LockstepCase3 : TestCase<LockstepTestCaseData>
    {
        public LockstepCase3()
        {
            isLoop = true;
            testCaseData = new LockstepTestCaseData[]
            {
                LockstepTestCaseDataHelper.NewLockstepTestCaseData(),
            };
        }
    }
}
