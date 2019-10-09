using System;
using Common;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using ServerFramework.Controller;

namespace ServerFramework.Servers
{
    class Server
    {
        private IPEndPoint ipEndPoint;
        private Socket serverSocket;
        private List<Client> clientList;
        private ControllerManager controllerManager;

        public Server() { Initialize(); }
        public Server(string ipStr, int port)
        {
            Initialize();
            SetIpAndPort(ipStr, port);
        }

        private void Initialize()
        {
            clientList = new List<Client>();
            controllerManager = new ControllerManager(this);
        }

        public void SetIpAndPort(string ipStr, int port)
        {
            ipEndPoint = new IPEndPoint(IPAddress.Parse(ipStr), port);
        }

        public void Start()
        {
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(ipEndPoint);
            serverSocket.Listen(0);
            serverSocket.BeginAccept(AcceptCallback, null);
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            Socket clientSocket = serverSocket.EndAccept(ar);
            Client client = new Client(clientSocket);
            client.OnRequest += HandleRequest;
            client.OnEnd += RemoveClient;
            client.Start();
            clientList.Add(client);
        }

        private void RemoveClient(Client client)
        {
            lock (clientList)
            {
                client.OnRequest -= HandleRequest;
                client.OnEnd -= RemoveClient;
                clientList.Remove(client);
            }
        }

        public void HandleRequest(Client client, RequestCode requestCode, ActionCode actionCode, string data)
        {
            controllerManager.HandleRequest(client, requestCode, actionCode, data);
        }

        public void SendResponse(Client client, RequestCode requsetCode, string data)
        {
            client.Send(requsetCode, data);
        }
    }
}
