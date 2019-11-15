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
            userController.Bind(RequestCode.Login, userController.OnLogin);
            userController.Bind(RequestCode.Register, userController.OnRegister);
            controllerDict.Add(userController.GetType(), userController);

            RoomController roomController = new RoomController();
            roomController.Bind(RequestCode.CreateRoom, roomController.OnCreateRoom);
            roomController.Bind(RequestCode.ListRooms, roomController.OnListRoom);
            roomController.Bind(RequestCode.JoinRoom, roomController.OnJoinRoom);
            roomController.Bind(RequestCode.QuitRoom, roomController.OnQuitRoom);
            controllerDict.Add(roomController.GetType(), roomController);

            GameController gameController = new GameController();
            gameController.Bind(RequestCode.StartGame, gameController.OnStartGame);
            controllerDict.Add(gameController.GetType(), gameController);

            LockstepController lockstepController = new LockstepController();
            lockstepController.Bind(RequestCode.Input, lockstepController.OnInput);
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
                    Console.WriteLine(e);
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
