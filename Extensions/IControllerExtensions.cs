using ServerFramework.Controller;
using System.Collections.Generic;
using ServerFramework.Servers;
using System.Text;
using System;
using Common;

namespace ServerFramework
{
    static class IControllerExtensions
    {
        public static void Bind<T, R>(this IController controller, RequestCode requestCode, Func<Client, T, R> action)
        {
            Server.Default.Receive(requestCode, action);
        }

        public static void Publish(this IController controller, RequestCode requestCode, byte[] dataBytes, params Client[] excludeClients)
        {
            List<Client> excludeClientList = new List<Client>();
            excludeClientList.AddRange(excludeClients);
            Server.Default.Publish(requestCode, dataBytes, excludeClientList);
        }

        public static void Publish(this IController controller, RequestCode requestCode, string data, params Client[] excludeClients)
        {
            Publish(controller, requestCode, Encoding.UTF8.GetBytes(data), excludeClients);
        }

        public static T GetController<T>(this IController controller) where T : IController
        {
            return ControllerManager.Default.GetController<T>();
        }
    }
}
