using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ServerClientLib.Utils;

namespace ServerClientLib
{
    public class Server
    {
        private readonly int _maxConnections;
        private readonly Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream,ProtocolType.Tcp);
        private int _connectionId = 1;
        private readonly List<Connection> _connections = new List<Connection>();
        private readonly Queue<string> _messageQueue = new Queue<string>();
        
        public Server(int maxConnections, EndPoint endPoint = null)
        {
            _maxConnections = maxConnections;
            _socket.Bind(endPoint ?? new IPEndPoint(0,5000));
            _socket.Listen(100);
            new Thread(StartListening).Start();
        }

        public event Action<Connection> ReceivedMessage;
        public event Action MaxConnectionReached;
        public event Action<Connection> NewConnection;
        public event Action<Connection> DisConnected;
        public List<Connection> GetConnections => _connections;
        public string GetMessage()
        {
            return _messageQueue.Dequeue();
        }
        public void Send(string msg,Connection connection)
        {
            connection.Send(Encoding.UTF8.GetBytes(msg));
        }
        

        private void StartListening()
        {
            while (_connections.Count <= _maxConnections)
            {
                var handler = _socket.Accept();
                var connection = new Connection(handler,_connectionId.ToString());
                _connectionId++;
                _connections.Add(connection);
                NewConnection?.Invoke(connection);
                new Thread(() => Receive(connection)).Start();
            }
            
            MaxConnectionReached?.Invoke();
        }

        private void Receive(Connection connection)
        {
            while (true)
            {
                var buffer = new byte[255];
                var received= connection.Receive(buffer);
                if (received <= 0)
                {
                    DisConnected?.Invoke(connection);
                    connection.Close();
                    if (_maxConnections == _connections.Count)
                        StartListening();
                    _connections.Remove(connection);
                    return;
                }            
                var msg = Encoding.UTF8.GetString(buffer, 0, received);
                _messageQueue.Enqueue(msg);
                ReceivedMessage?.Invoke(connection);
            }
        }
        
    }
}