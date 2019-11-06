using System.Collections.Generic;

namespace ServerFramework.Servers
{
    public enum RoomState
    {
        Join,
        Ready,
        Play,
        End,
    }

    class Room
    {
        public RoomState State { get; set; } = RoomState.Join;
        public List<Client> ClientList { get; set; } = new List<Client>();
        public delegate void EndHandler(Room room, Client client);
        public event EndHandler OnEnd;

        public Room(Client client)
        {
            client.OnEnd += Quit;
            AddClient(client);
        }

        public void AddClient(Client client)
        {
            client.OnEnd += Quit;
            ClientList.Add(client);
        }

        public Client GetOwner()
        {
            if (ClientList.Count > 0)
            {
                return ClientList[0];
            }
            return null;
        }

        public void Quit(Client client)
        {
            if (client == GetOwner())
            {
                ClientList.Clear();
            }
            else if (ClientList.Contains(client))
            {
                ClientList.Remove(client);
            }
            if (ClientList.Count < 2)
            {
                State = RoomState.Join;
            }
            OnEnd?.Invoke(this, client);
            client.OnEnd -= Quit;
        }
    }
}
