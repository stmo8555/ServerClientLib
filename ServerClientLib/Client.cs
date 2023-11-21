using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ServerClientLib.Utils;

namespace ServerClientLib
{
    public class Client
    {
        private readonly Connection _connection;
        private readonly Queue<string> _messageQueue = new Queue<string>();
        
        public Client (EndPoint endPoint = null)
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream,ProtocolType.Tcp);
            socket.Connect(endPoint ?? new IPEndPoint(0,5000));
            _connection = new Connection(socket, "Client");
            new Thread(() => Receive(_connection)).Start();
        }

        public event Action<Connection> ReceivedMessage;
        public Connection GetConnection => _connection;
        public string GetMessage()
        {
            return _messageQueue.Dequeue();
        }
        public void Send(string msg,Connection connection)
        {
            connection.Send(Encoding.UTF8.GetBytes(msg));
        }

        private void Receive(Connection connection)
        {
            while (true)
            {
                var buffer = new byte[255];
                var received= connection.Receive(buffer);
                var msg = Encoding.UTF8.GetString(buffer, 0, received);
                _messageQueue.Enqueue(msg);
                ReceivedMessage?.Invoke(connection);
            }
        }
    }
}