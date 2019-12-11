namespace ServerFramework
{
    public interface ITestCase
    {
        bool Next();
        void Complete();
    }

    public interface ITestCase<T> : ITestCase where T : ITestCaseData
    {
        T Current { get; set; }
    }

    public class TestCase<T> : ITestCase<T> where T : ITestCaseData
    {
        protected T[] testCaseData;
        protected bool isLoop;
        protected int loopTimes;
        private int index;
        private int count;

        public T Current { get; set; }

        public bool Next()
        {
            if (index >= testCaseData.Length)
            {
                if (isLoop && (loopTimes <= 0 || count < loopTimes))
                {
                    index = 0;
                    count++;
                }
            }
            if (index < testCaseData.Length)
            {
                Current = testCaseData[index];
                index++;
                return true;
            }
            return false;
        }

        public virtual void Complete()
        {
            index = 0;
            count = 0;
        }
    }
}
