using ServerFramework.Controller;
using ServerFramework.Servers;
using System;
using Common;

namespace ServerFramework.Extensions
{
    static class IControllerExtensions
    {
        public static void Bind(this IController controller, RequestCode requestCode, Func<Client, string, string> action)
        {
            Server.Default.Receive(requestCode, action);
        }
    }
}
