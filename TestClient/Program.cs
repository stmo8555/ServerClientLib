using System;
using System.Net.Sockets;
using System.Text;

namespace TestClient
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            using (var s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                s.Connect("127.0.0.1", 5000);
                while (true)
                {
                    Console.WriteLine("Write to Server: ");
                    var msg = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(msg))
                        continue;
                    
                    
                    var msgBytes = Encoding.UTF8.GetBytes(msg);
                    s.Send(msgBytes);
                    
                    var buffer = new byte[1_024];
                    var received = s.Receive(buffer, SocketFlags.None);
                    var response = Encoding.UTF8.GetString(buffer, 0, received);
                    Console.WriteLine(response);
                }
                
                
                
                s.Shutdown(SocketShutdown.Both);
            }
        }
    }
}