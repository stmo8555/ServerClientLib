using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ServerClientLib
{
    public partial class ServerLib
    {
        private readonly int _maxConnections;
        private readonly Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream,ProtocolType.Tcp);
        private int _connectionId = 0;
        private readonly List<Connection> _connections = new List<Connection>();
        private Queue<string> _messageQueue = new Queue<string>();
        
        public ServerLib(EndPoint endPoint = null, int maxConnections = -1)
        {
            _maxConnections = maxConnections;
            _socket.Bind(endPoint ?? new IPEndPoint(0,5000));
            _socket.Listen(100);
            new Thread(StartListening).Start();
        }
        public delegate void Notify();
        public event Notify RecievedMessage;
        public List<Connection> GetConnection => _connections;
        public string GetMessage()
        {
            return _messageQueue.Dequeue();
        }
        public void Send(string msg,Connection connection)
        {
            connection.Handler.Send(Encoding.UTF8.GetBytes(msg));
        }
        

        private void StartListening()
        {
            Func<bool> condition = () => _connections.Count <= _maxConnections;
            if (_maxConnections == -1)
                condition = () => true;
            
            while (condition())
            {
                var handler = _socket.Accept();
                var connection = new Connection(handler, "p" + _connectionId++);
                _connections.Add(connection);
                new Thread(() => Receive(connection)).Start();
            }   
        }

        private void Receive(Connection connection)
        {
            while (true)
            {
                var buffer = new byte[20];
                var received= connection.Handler.Receive(buffer);
                var msg = Encoding.UTF8.GetString(buffer, 0, received);
                _messageQueue.Enqueue(msg);
                RecievedMessage?.Invoke();
            }
        }
        
        
    }
}