using ServerFramework.Controller;
using System.Collections.Generic;
using ServerFramework.Servers;
using System;
using Common;

namespace ServerFramework
{
    static class IControllerExtensions
    {
        public static void Bind(this IController controller, RequestCode requestCode, Func<Client, string, string> action)
        {
            Server.Default.Receive(requestCode, action);
        }

        public static void Publish(this IController controller, RequestCode requestCode, string data, params Client[] excludeClients)
        {
            List<Client> excludeClientList = new List<Client>();
            excludeClientList.AddRange(excludeClients);
            Server.Default.Publish(requestCode, data, excludeClientList);
        }

        public static T GetController<T>(this IController controller) where T : IController
        {
            return ControllerManager.Default.GetController<T>();
        }
    }
}
