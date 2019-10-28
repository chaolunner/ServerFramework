using ServerFramework.Controller;
using ServerFramework.Servers;
using System;

namespace ServerFramework
{
    class Program
    {
        static void Main(string[] args)
        {
            Server.Default.Start();

            ControllerManager.Default.Start();

            Console.ReadKey();
        }
    }
}
