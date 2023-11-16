using System;
using System.Runtime.InteropServices;
using System.Threading;
using ServerClientLib;

namespace TestServer
{
    internal class Program
    {
        private static int i = 0;
        public static void Main(string[] args)
        {
            var s = new Server(100);
            s.ReceivedMessage += () =>
            {
                Console.WriteLine(s.GetMessage());
                
                s.Send("Post" + i++, s.GetConnection[0]);
            };
            while (true)
            {
                Thread.Sleep(1);
                if (s.GetConnection.Count > 0)
                    s.Send("stress Test", s.GetConnection[0]);
            }
        }
    }
}