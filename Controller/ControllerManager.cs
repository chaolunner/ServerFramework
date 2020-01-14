using System.Collections.Generic;
using System.Threading;
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
            userController.Bind<string, string>(RequestCode.Login, userController.OnLogin);
            userController.Bind<string, string>(RequestCode.Register, userController.OnRegister);
            controllerDict.Add(userController.GetType(), userController);

            RoomController roomController = new RoomController();
            roomController.Bind<string, string>(RequestCode.CreateRoom, roomController.OnCreateRoom);
            roomController.Bind<string, string>(RequestCode.ListRooms, roomController.OnListRoom);
            roomController.Bind<string, string>(RequestCode.JoinRoom, roomController.OnJoinRoom);
            roomController.Bind<string, string>(RequestCode.QuitRoom, roomController.OnQuitRoom);
            controllerDict.Add(roomController.GetType(), roomController);

            LockstepController lockstepController = new LockstepController();
            lockstepController.Bind<byte[], string>(RequestCode.Input, lockstepController.OnInput);
            controllerDict.Add(lockstepController.GetType(), lockstepController);

            while (true)
            {
                try
                {
                    foreach (var kvp in controllerDict)
                    {
                        kvp.Value.Update();
                    }
                    Thread.Sleep(20);
                }
                catch (Exception e)
                {
                    ConsoleUtility.WriteLine(e, ConsoleColor.Red);
                }
            }
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
