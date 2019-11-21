using System.Reflection;

namespace Common
{
    public static class AssemblyHelper
    {
        private const string CommonStr = "Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";

        public static Assembly Assembly { get; } = Assembly.Load(CommonStr);
    }
}
