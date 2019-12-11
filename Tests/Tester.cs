using System.Collections.Generic;

namespace ServerFramework
{
    public interface ITester
    {
        void AddTestCase(ITestCase testCase);
        void Restart();
        bool Next();
        void Complete();
    }

    public interface ITester<T> : ITester where T : ITestCaseData
    {
        T Current { get; set; }
        void AddTestCase(ITestCase<T> testCase);
    }

    public class Tester<T> : ITester<T> where T : ITestCaseData
    {
        protected bool isLoop = true;
        private List<ITestCase<T>> testCases = new List<ITestCase<T>>();
        private int index;

        public void AddTestCase(ITestCase testCase)
        {
            testCases.Add((ITestCase<T>)testCase);
        }

        public void AddTestCase(ITestCase<T> testCase)
        {
            testCases.Add(testCase);
        }

        public virtual void Restart()
        {
            for (int i = 0; i < testCases.Count; i++)
            {
                testCases[i].Complete();
            }
            index = 0;
        }

        public T Current { get; set; }

        public bool Next()
        {
            if (index >= testCases.Count)
            {
                if (isLoop)
                {
                    index = 0;
                }
                else
                {
                    Complete();
                }
            }
            while (index < testCases.Count)
            {
                if (testCases[index].Next())
                {
                    Current = testCases[index].Current;
                    return true;
                }
                else
                {
                    index++;
                    testCases[index].Complete();
                }
            }
            return false;
        }

        public virtual void Complete()
        {
        }
    }
}
