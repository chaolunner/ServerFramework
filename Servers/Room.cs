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
        private List<Client> clientList = new List<Client>();
        private RoomState state = RoomState.Join;

        public void AddClient(Client client)
        {
            clientList.Add(client);
        }
    }
}
