using MySql.Data.MySqlClient;
using System.Net.Sockets;
using System.Net;
using Common;

namespace ServerFramework.Servers
{
    public interface IClient
    {
        void Publish<T>(RequestCode requestCode, T data);
        void Response(RequestCode requestCode, byte[] dataBytes);
    }

    public class Client : IClient
    {
        public EndPoint RemoteEndPoint;

        private ISession session;
        private Message msg = new Message();
        private MessageAsyncReceive messageAsyncReceive;
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

        public Client(Socket serverSocket, EndPoint remoteEP)
        {
            RemoteEndPoint = remoteEP;
            messageAsyncReceive = new MessageAsyncReceive(msg);
            messageAsyncReceive.BeginReceive(ReceiveCallback);
            session = new KcpSession(serverSocket, messageAsyncReceive, remoteEP);
            mySqlConn = ConnHelper.Connect();
        }

        public Client(Socket clientSocket)
        {
            RemoteEndPoint = clientSocket.RemoteEndPoint;
            messageAsyncReceive = new MessageAsyncReceive(msg);
            messageAsyncReceive.BeginReceive(ReceiveCallback);
            session = new TcpSession(clientSocket, messageAsyncReceive);
            mySqlConn = ConnHelper.Connect();
        }

        private void ReceiveCallback(int count)
        {
            if (count == 0)
            {
                End();
            }
            else
            {
                msg.Process(count, Response);
            }
        }

        public void Response(RequestCode requestCode, byte[] dataBytes)
        {
            OnResponse?.Invoke(this, requestCode, dataBytes);
        }

        private void End()
        {
            if (session != null)
            {
                session.Close();
                session = null;
                ConnHelper.Disconnect(mySqlConn);
                mySqlConn = null;
                OnEnd?.Invoke(this);
            }
        }

        public void Publish<T>(RequestCode requsetCode, T data)
        {
            if (session != null && session.IsConnected)
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
                session.Send(bytes);
            }
        }

        public void OnAsyncReceive(EndPoint remoteEP, IAsyncReceive asyncReceive, int count)
        {
            if (count == 0)
            {
                End();
            }
            else if (RemoteEndPoint.Equals(remoteEP))
            {
                session.Receive(asyncReceive, count);
            }
        }
    }
}
