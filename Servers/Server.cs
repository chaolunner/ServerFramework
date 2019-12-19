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
        void Start();
        void Publish<T>(RequestCode requestCode, T data, List<Client> excludeClients);
        void Receive<T, R>(RequestCode requestCode, Func<Client, T, R> func);
        void Response(Client client, RequestCode requestCode, byte[] dataBytes);
    }

    class Server : IServer, IDisposable
    {
        public static readonly IServer Default = new Server();

        private bool isDisposed = false;
        private IPEndPoint ipEndPoint;
        private EndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
        private IAsyncReceive udpAsyncReceive = new AsyncReceive();
        private Socket serverSocket;
        private event Action<EndPoint, IAsyncReceive, int> OnAsyncReceive;
        private readonly Dictionary<EndPoint, Client> clientDict = new Dictionary<EndPoint, Client>();
        private readonly Dictionary<RequestCode, List<object>> notifiers = new Dictionary<RequestCode, List<object>>();

        private const string IP = "ip";
        private const string PORT = "port";
        private const string MODE = "mode";
        private const string TCP = "Tcp";
        private const string UDP = "Udp";

        public Server()
        {
            ipEndPoint = new IPEndPoint(IPAddress.Parse(ConfigUtility.GetAppConfig(IP)), int.Parse(ConfigUtility.GetAppConfig(PORT)));
        }

        public void Start()
        {
            string mode = ConfigUtility.GetAppConfig(MODE);
            if (mode == TCP)
            {
                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                serverSocket.Bind(ipEndPoint);
                serverSocket.Listen(0);
                serverSocket.BeginAccept(AcceptCallback, null);
            }
            else if (mode == UDP)
            {
                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                serverSocket.Bind(ipEndPoint);
                serverSocket.BeginReceiveFrom(udpAsyncReceive.Buffer, udpAsyncReceive.Offset, udpAsyncReceive.Size, SocketFlags.None, ref remoteEP, ReceiveFromCallback, null);
            }
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            Socket clientSocket = serverSocket.EndAccept(ar);
            Client client = new Client(clientSocket);
            AddClient(client);
            serverSocket.BeginAccept(AcceptCallback, null);
        }

        private void ReceiveFromCallback(IAsyncResult ar)
        {
            try
            {
                int count = serverSocket.EndReceiveFrom(ar, ref remoteEP);
                if (count > 0 && !clientDict.ContainsKey(remoteEP))
                {
                    Client client = new Client(serverSocket, remoteEP);
                    AddClient(client);
                }
                OnAsyncReceive?.Invoke(remoteEP, udpAsyncReceive, count);
            }
            catch (Exception e)
            {
                OnAsyncReceive?.Invoke(remoteEP, udpAsyncReceive, 0);
                ConsoleUtility.WriteLine(e);
            }
            finally
            {
                remoteEP = new IPEndPoint(IPAddress.Any, 0);
                serverSocket.BeginReceiveFrom(udpAsyncReceive.Buffer, udpAsyncReceive.Offset, udpAsyncReceive.Size, SocketFlags.None, ref remoteEP, ReceiveFromCallback, null);
            }
        }

        private void AddClient(Client client)
        {
            client.OnResponse += Response;
            client.OnEnd += RemoveClient;
            OnAsyncReceive += client.OnAsyncReceive;
            clientDict.Add(client.RemoteEndPoint, client);
        }

        private void RemoveClient(Client client)
        {
            lock (clientDict)
            {
                client.OnResponse -= Response;
                client.OnEnd -= RemoveClient;
                OnAsyncReceive -= client.OnAsyncReceive;
                clientDict.Remove(client.RemoteEndPoint);
            }
        }

        public void Publish<T>(RequestCode requestCode, T data, List<Client> excludeClients)
        {
            foreach (Client client in clientDict.Values)
            {
                if (excludeClients.Contains(client)) { continue; }
                client.Publish(requestCode, data);
            }
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
