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
        public delegate void EndHandler(Room room);
        public event EndHandler OnEnd;

        public Room(Client client)
        {
            client.OnEnd += Quit;
            AddClient(client);
        }

        public void AddClient(Client client)
        {
            ClientList.Add(client);
        }

        public Client GetOwner()
        {
            return ClientList[0];
        }

        public void Quit(Client client)
        {
            State = RoomState.Join;
            OnEnd?.Invoke(this);
            ClientList.Clear();
            client.OnEnd -= Quit;
        }
    }
}
