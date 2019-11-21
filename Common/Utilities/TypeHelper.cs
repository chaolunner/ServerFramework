using System.Collections.Generic;
using System;

namespace Common
{
    public static class TypeHelper
    {
        private static Dictionary<string, Type> typeDict = new Dictionary<string, Type>();

        public static Type GetType(string name)
        {
            if (!typeDict.ContainsKey(name))
            {
                typeDict.Add(name, AssemblyHelper.Assembly.GetType(name));
            }
            return typeDict[name];
        }
    }
}
