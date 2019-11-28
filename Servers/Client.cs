using MySql.Data.MySqlClient;
using System.Net.Sockets;
using System;
using Common;

namespace ServerFramework.Servers
{
    public interface IClient
    {
        void Start();
        void Publish<T>(RequestCode requestCode, T data);
        void Response(RequestCode requestCode, byte[] dataBytes);
    }

    public class Client : IClient
    {
        private Socket clientSocket;
        private Message msg = new Message();
        private MySqlConnection mySqlConn;

        public delegate void RequestHandler(Client client, RequestCode requestCode, byte[] dataBytes);
        public delegate void EndHandler(Client client);
        public event RequestHandler OnResponse;
        public event EndHandler OnEnd;

        public MySqlConnection MySqlConn
        {
            get
            {
                return mySqlConn;
            }
        }

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
                ConsoleUtility.WriteLine(e, ConsoleColor.Red);
                End();
            }
        }

        public void Response(RequestCode requestCode, byte[] dataBytes)
        {
            OnResponse?.Invoke(this, requestCode, dataBytes);
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

        public void Publish<T>(RequestCode requsetCode, T data)
        {
            if (clientSocket != null && clientSocket.Connected)
            {
                byte[] bytes = null;
                if (typeof(T) == typeof(byte[]))
                {
                    bytes = Message.Pack(requsetCode, data as byte[]);
                }
                else
                {
                    bytes = Message.Pack(requsetCode, data.ToString());
                }
                clientSocket.Send(bytes);
            }
        }
    }
}
