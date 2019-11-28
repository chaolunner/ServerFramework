using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Net;
using System;
using Common;

namespace ServerFramework.Servers
{
    public interface IServer
    {
        void SetIpAndPort(string ipStr, int port);
        void Start();
        void Publish(RequestCode requestCode, byte[] dataBytes, List<Client> excludeClients);
        void Publish(RequestCode requestCode, string data, List<Client> excludeClients);
        void Receive<T, R>(RequestCode requestCode, Func<Client, T, R> func);
        void Response(Client client, RequestCode requestCode, byte[] dataBytes);
    }

    class Server : IServer, IDisposable
    {
        public static readonly IServer Default = new Server();

        private bool isDisposed = false;
        private IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9663);
        private Socket serverSocket;
        private readonly List<Client> clientList = new List<Client>();
        private readonly Dictionary<RequestCode, List<object>> notifiers = new Dictionary<RequestCode, List<object>>();

        public Server() { }
        public Server(string ipStr, int port)
        {
            SetIpAndPort(ipStr, port);
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
            client.OnResponse += Response;
            client.OnEnd += RemoveClient;
            client.Start();
            clientList.Add(client);
            serverSocket.BeginAccept(AcceptCallback, null);
        }

        private void RemoveClient(Client client)
        {
            lock (clientList)
            {
                client.OnResponse -= Response;
                client.OnEnd -= RemoveClient;
                clientList.Remove(client);
            }
        }

        public void Publish(RequestCode requestCode, byte[] dataBytes, List<Client> excludeClients)
        {
            foreach (Client client in clientList)
            {
                if (excludeClients.Contains(client)) { continue; }
                client.Publish(requestCode, dataBytes);
            }
        }

        public void Publish(RequestCode requestCode, string data, List<Client> excludeClients)
        {
            Publish(requestCode, Encoding.UTF8.GetBytes(data), excludeClients);
        }

        public void Receive<T, R>(RequestCode requestCode, Func<Client, T, R> func)
        {
            if (!notifiers.ContainsKey(requestCode))
            {
                notifiers.Add(requestCode, new List<object>());
            }
            notifiers[requestCode].Add(func);
        }

        public void Response(Client client, RequestCode requestCode, byte[] dataBytes)
        {
            if (notifiers.ContainsKey(requestCode))
            {
                foreach (var func in notifiers[requestCode])
                {
                    Type[] types = func.GetType().GetGenericArguments();
                    if (types[1] == typeof(byte[]) && types[2] == types[1])
                    {
                        client.Publish(requestCode, (func as Func<Client, byte[], byte[]>)?.Invoke(client, dataBytes));
                    }
                    else if (types[1] == typeof(string) && types[2] == typeof(byte[]))
                    {
                        client.Publish(requestCode, (func as Func<Client, string, byte[]>)?.Invoke(client, Encoding.UTF8.GetString(dataBytes)));
                    }
                    else if (types[1] == typeof(byte[]) && types[2] == typeof(string))
                    {
                        client.Publish(requestCode, (func as Func<Client, byte[], string>)?.Invoke(client, dataBytes));
                    }
                    else if (types[1] == typeof(string) && types[2] == types[1])
                    {
                        client.Publish(requestCode, (func as Func<Client, string, string>)?.Invoke(client, Encoding.UTF8.GetString(dataBytes)));
                    }
                }
            }
            else
            {
                ConsoleUtility.WriteLine("The notifier corresponding to " + requestCode + " could not be found.");
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
