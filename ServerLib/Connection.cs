using System.Net.Sockets;

namespace ServerClientLib
{
    public partial class ServerLib
    {
        private class Connection
        {
            public Socket Handler { get; private set; }
            public string Id { get; private set; }

            public Connection(Socket handler, string id)
            {
                Handler = handler;
                Id = id;
            }

            ~Connection()
            {
                Handler.Disconnect(false);
                Handler.Dispose();
            }
        }
    }
}