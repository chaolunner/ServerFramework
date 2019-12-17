using MySql.Data.MySqlClient;
using System.Net.Sockets;
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
        private ISession session;
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
            session = new TcpSession(clientSocket, new AsyncReceive(msg, ReceiveCallback));
            mySqlConn = ConnHelper.Connect();
        }

        public void Start()
        {
            if (session != null)
            {
                session.Receive();
            }
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
                ConnHelper.Disconnect(mySqlConn);
                mySqlConn = null;
                session.Close();
                session = null;
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
    }
}
