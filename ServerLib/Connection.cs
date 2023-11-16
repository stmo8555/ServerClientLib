using System;
using System.Net.Sockets;
namespace ServerClientLib
{
    public partial class Server
    {
        public class Connection
        {
            private Socket _handler;
            private readonly QueueThread _thread = new QueueThread();
            public string Id { get; private set; }

            public Connection(Socket handler, string id)
            {
                _handler = handler;
                Id = id;
            }

            public void Send(byte[] msgBytes)
            {
                _thread.Enqueue(() => _handler.Send(msgBytes));
            }

            public int Receive(byte[] buffer)
            {
                return _handler.Receive(buffer);
            }

            public void Close()
            {
                _handler.Shutdown(SocketShutdown.Both);
                _handler.Close();
                _handler.Dispose();
            }
        }
    }
}