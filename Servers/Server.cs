using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System;
using Common;

namespace ServerFramework.Servers
{
    public interface IServerRequestReceiver
    {
        void Receive(RequestCode requestCode, Func<Client, string, string> action);
    }

    public interface IServerRequestResponser
    {
        void Response(Client client, RequestCode requestCode, string data);
    }

    public interface IServer : IServerRequestReceiver, IServerRequestResponser { }

    class Server : IServer, IDisposable
    {
        public static readonly IServer Default = new Server();

        private bool isDisposed = false;
        private IPEndPoint ipEndPoint;
        private Socket serverSocket;
        private List<Client> clientList;
        private readonly Dictionary<RequestCode, List<Func<Client, string, string>>> notifiers = new Dictionary<RequestCode, List<Func<Client, string, string>>>();

        public Server() { Initialize(); }
        public Server(string ipStr, int port)
        {
            Initialize();
            SetIpAndPort(ipStr, port);
        }

        private void Initialize()
        {
            clientList = new List<Client>();
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
            client.OnRequest += Response;
            client.OnEnd += RemoveClient;
            client.Start();
            clientList.Add(client);
        }

        private void RemoveClient(Client client)
        {
            lock (clientList)
            {
                client.OnRequest -= Response;
                client.OnEnd -= RemoveClient;
                clientList.Remove(client);
            }
        }

        public void Receive(RequestCode requestCode, Func<Client, string, string> func)
        {
            if (!notifiers.ContainsKey(requestCode))
            {
                notifiers.Add(requestCode, new List<Func<Client, string, string>>());
            }
            notifiers[requestCode].Add(func);
        }

        public void Response(Client client, RequestCode requestCode, string data)
        {
            if (notifiers.ContainsKey(requestCode))
            {
                foreach (var func in notifiers[requestCode])
                {
                    var result = func?.Invoke(client, data);
                    if (result == null || string.IsNullOrEmpty(result))
                    {
                    }
                    else
                    {
                        client.Publish(requestCode, result);
                    }
                }
            }
            else
            {
                Console.WriteLine("The controller corresponding to " + requestCode + " could not be found.");
            }
        }

        public void Dispose()
        {
            lock (notifiers)
            {
                if (!isDisposed)
                {
                    isDisposed = true;
                    notifiers.Clear();
                }
            }
        }
    }
}
