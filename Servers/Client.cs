using System;
using Common;
using System.Net;
using System.Net.Sockets;
using ServerFramework.Tool;
using MySql.Data.MySqlClient;

namespace ServerFramework.Servers
{
    class Client
    {
        private Socket clientSocket;
        private Message msg = new Message();
        private MySqlConnection mySqlConn;

        public delegate void RequestHandler(Client client, RequestCode requestCode, ActionCode actionCode, string data);
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
                    msg.Process(count, OnMessageProcessed);
                    Start();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                End();
            }
        }

        private void OnMessageProcessed(RequestCode requestCode, ActionCode actionCode, string data)
        {
            OnRequest?.Invoke(this, requestCode, actionCode, data);
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

        public void Send(RequestCode requsetCode, string data)
        {
            byte[] bytes = Message.Pack(requsetCode, data);
            clientSocket.Send(bytes);
        }
    }
}
