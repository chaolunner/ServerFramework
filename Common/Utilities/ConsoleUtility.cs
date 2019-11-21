using System;

namespace Common
{
    public static class ConsoleUtility
    {
        public static void WriteLine(Exception e, ConsoleColor color = ConsoleColor.White)
        {
            WriteLine(e.ToString());
        }

        public static void WriteLine(string msg, ConsoleColor color = ConsoleColor.White)
        {
#if UNITY
            if (UnityEngine.Application.isEditor)
            {
                if (color == ConsoleColor.Red)
                {
                    UnityEngine.Debug.LogError(msg);
                }
                else if (color == ConsoleColor.Yellow)
                {
                    UnityEngine.Debug.LogWarning(msg);
                }
                else
                {
                    UnityEngine.Debug.Log(msg);
                }
            }
#else
            ConsoleColor foregroundColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(msg);
            Console.ForegroundColor = foregroundColor;
#endif
        }
    }
}
