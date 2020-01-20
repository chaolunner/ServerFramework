using System.Collections.Generic;
using Common;

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
        public Client Owner { get; set; }
        public RoomState State { get; set; } = RoomState.Join;
        public List<Client> ClientList { get; set; } = new List<Client>();
        public delegate void EndHandler(Room room, Client client);
        public event EndHandler OnEnd;

        public Room(Client client)
        {
            Owner = client;
            AddClient(client);
        }

        public void AddClient(Client client)
        {
            client.OnEnd += Quit;
            ClientList.Add(client);
        }

        public void Publish<T>(RequestCode requestCode, T data, params Client[] excludeClients)
        {
            var excludeList = new List<Client>();
            excludeList.AddRange(excludeClients);
            for (int i = 0; i < ClientList.Count; i++)
            {
                if (excludeList.Contains(ClientList[i])) { continue; }
                ClientList[i].Publish(requestCode, data);
            }
        }

        public void Quit(Client client)
        {
            if (ClientList.Contains(client))
            {
                ClientList.Remove(client);
            }
            if (ClientList.Count < 2)
            {
                State = RoomState.Join;
            }
            OnEnd?.Invoke(this, client);
            if (client == Owner)
            {
                ClientList.Clear();
            }
            client.OnEnd -= Quit;
        }
    }
}
