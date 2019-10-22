using MySql.Data.MySqlClient;
using ServerFramework.Tool;
using System.Net.Sockets;
using System;
using Common;

namespace ServerFramework.Servers
{
    public interface IClientRequestPublisher
    {
        void Publish(RequestCode requestCode, string data);
    }

    public interface IClientRequestResponser
    {
        void Response(RequestCode requestCode, string data);
    }

    public interface IClient : IClientRequestPublisher, IClientRequestResponser { }

    public class Client : IClient
    {
        private Socket clientSocket;
        private Message msg = new Message();
        private MySqlConnection mySqlConn;

        public delegate void RequestHandler(Client client, RequestCode requestCode, string data);
        public delegate void EndHandler(Client client);
        public event RequestHandler OnRequest;
        public event EndHandler OnEnd;

        public Client() { }
        public Client(Socket clientSocket)
        {
            this.clientSocket = clientSocket;
            mySqlConn = ConnHelper.Connect();
        }

        public void Start()
        {
            clientSocket.BeginReceive(msg.Data, msg.StartIndex, msg.RemainSize, SocketFlags.None, ReceiveCallback, null);
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                int count = clientSocket.EndReceive(ar);
                if (count == 0)
                {
                    End();
                }
                else
                {
                    msg.Process(count, Response);
                    Start();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                End();
            }
        }

        public void Response(RequestCode requestCode, string data)
        {
            OnRequest?.Invoke(this, requestCode, data);
        }

        private void End()
        {
            if (clientSocket != null)
            {
                ConnHelper.Disconnect(mySqlConn);
                mySqlConn = null;
                clientSocket.Close();
                clientSocket = null;
                OnEnd?.Invoke(this);
            }
        }

        public void Publish(RequestCode requsetCode, string data)
        {
            byte[] bytes = Message.Pack(requsetCode, data);
            clientSocket.Send(bytes);
        }
    }
}
