using System.Collections.Generic;
using System;

namespace ServerFramework
{
    public class TestManager
    {
        public static TestManager Default = new TestManager();

        private Dictionary<Type, ITester> testerDict = new Dictionary<Type, ITester>();

        public TestManager()
        {
            var lockstepTester = Create<LockstepTester>();
            lockstepTester.AddTestCase(new LockstepCase1(times: 500));
            lockstepTester.AddTestCase(new LockstepCase1(loop: false, nextSecond: 10, vertical: 1));
            lockstepTester.AddTestCase(new LockstepCase1());
        }

        public T Create<T>(params ITestCase[] testCases) where T : ITester
        {
            var testerType = typeof(T);
            if (!testerDict.ContainsKey(testerType))
            {
                testerDict.Add(testerType, (T)Activator.CreateInstance(testerType));
            }
            for (int i = 0; i < testCases.Length; i++)
            {
                ((T)testerDict[testerType]).AddTestCase(testCases[i]);
            }
            return (T)testerDict[testerType];
        }

        public T Get<T>() where T : ITester
        {
            var testerType = typeof(T);
            if (testerDict.ContainsKey(testerType))
            {
                return (T)testerDict[testerType];
            }
            return default;
        }
    }
}
