using System;
using System.Collections.Concurrent;
using System.Threading;

// Stolen from https://michaelscodingspot.com/c-job-queues/ with some tweaks.
namespace ServerClientLib.Utils
{
    public class QueueThread
    {
        private readonly ConcurrentQueue<Action> _jobs = new ConcurrentQueue<Action>();

        public QueueThread()
        {
            var thread = new Thread(new ThreadStart(OnStart));
            thread.IsBackground = true;
            thread.Start();
        }

        public void Enqueue(Action job)
        {
            _jobs.Enqueue(job);
        }

        private void OnStart()
        {
            while (true)
            {
                if (_jobs.TryDequeue(out var action)) 
                    action();
            }
        }
    }
}