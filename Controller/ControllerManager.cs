using System;
using Common;
using System.Reflection;
using ServerFramework.Servers;
using System.Collections.Generic;

namespace ServerFramework.Controller
{
    class ControllerManager
    {
        private Dictionary<RequestCode, ControllerBase> controllerDict = new Dictionary<RequestCode, ControllerBase>();
        private Server server;

        public ControllerManager(Server server)
        {
            this.server = server;
            InitializeControllers();
        }

        private void InitializeControllers()
        {
            DefaultController defaultController = new DefaultController();
            controllerDict.Add(defaultController.RequestCode, defaultController);
        }

        public void HandleRequest(Client client, RequestCode requestCode, ActionCode actionCode, string data)
        {
            ControllerBase controller;
            if (controllerDict.TryGetValue(requestCode, out controller))
            {
                string methodName = Enum.GetName(typeof(ActionCode), actionCode);
                MethodInfo methodInfo = controller.GetType().GetMethod(methodName);
                if (methodInfo == null)
                {
                    Console.WriteLine("No corresponding method of " + actionCode + " can be found in " + controller.GetType() + ".");
                }
                else
                {
                    object[] parameters = new object[] { data };
                    object o = methodInfo.Invoke(controller, parameters);
                    if (o == null || string.IsNullOrEmpty(o as string))
                    {
                    }
                    else
                    {
                        server.SendResponse(client, requestCode, o as string);
                    }
                }
            }
            else
            {
                Console.WriteLine("The controller corresponding to " + requestCode + " could not be found.");
            }
        }
    }
}
