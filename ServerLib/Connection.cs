using System;
using System.Net.Sockets;
namespace ServerClientLib
{
    public partial class ServerLib
    {
        public class Connection
        {
            private readonly QueueThread _thread = new QueueThread();
            public Socket Handler { get; private set; }
            public string Id { get; private set; }

            public Connection(Socket handler, string id)
            {
                Handler = handler;
                Id = id;
            }

            public void PostJob(Action a)
            {
                _thread.Enqueue(a);
            }

            public void Close()
            {
                Handler.Shutdown(SocketShutdown.Both);
                Handler.Close();
                Handler.Dispose();
            }
        }
    }
}