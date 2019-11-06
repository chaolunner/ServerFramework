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

        public static void Publish(this IController controller, RequestCode requestCode, string data)
        {
            Server.Default.Publish(requestCode, data);
        }

        public static T GetController<T>(this IController controller) where T : IController
        {
            return ControllerManager.Default.GetController<T>();
        }
    }
}
