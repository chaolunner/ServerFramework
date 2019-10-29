using System.Collections.Generic;
using ServerFramework.Extensions;
using Common;

namespace ServerFramework.Controller
{
    class ControllerManager
    {
        public static readonly ControllerManager Default = new ControllerManager();

        private readonly List<IController> controllers = new List<IController>();

        public void Start()
        {
            UserController userController = new UserController();
            userController.Bind(RequestCode.Login, userController.OnLogin);
            userController.Bind(RequestCode.Register, userController.OnRegister);
            controllers.Add(userController);
        }
    }
}
