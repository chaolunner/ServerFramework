using System.Collections.Generic;
using ServerFramework.Extensions;
using System;
using Common;

namespace ServerFramework.Controller
{
    class ControllerManager
    {
        public static readonly ControllerManager Default = new ControllerManager();

        private readonly Dictionary<Type, IController> controllerDict = new Dictionary<Type, IController>();

        public void Start()
        {
            UserController userController = new UserController();
            userController.Bind(RequestCode.Login, userController.OnLogin);
            userController.Bind(RequestCode.Register, userController.OnRegister);
            controllerDict.Add(userController.GetType(), userController);

            RoomController roomController = new RoomController();
            roomController.Bind(RequestCode.CreateRoom, roomController.OnCreateRoom);
            roomController.Bind(RequestCode.ListRooms, roomController.OnListRoom);
            controllerDict.Add(roomController.GetType(), roomController);
        }

        public T GetController<T>() where T : IController
        {
            Type type = typeof(T);
            if (controllerDict.ContainsKey(type))
            {
                return (T)controllerDict[type];
            }
            return default;
        }
    }
}
