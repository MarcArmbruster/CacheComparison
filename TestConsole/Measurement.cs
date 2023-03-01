namespace TestConsole
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    internal class Measurement
    {
        private Dictionary<long, string> log = new Dictionary<long, string>();
        private Stopwatch stopwatch = new Stopwatch();
        private Action<string> outputChannel = s => { };
        internal long TotalMilliseconds { get; private set; }
        internal static Measurement Create(Action<string> outputChannel)
        {
            var instance = new Measurement();
            if (outputChannel != null)
            {
                instance.outputChannel = outputChannel;
            }
                
            return instance;
        }

        internal Measurement Start()
        {
            log.Add(0, "START");
            outputChannel.Invoke("START");
            this.stopwatch.Start();
            return this;
        }

        internal Measurement Invoke(string info, Action action)
        {
            action.Invoke();
            var lastTimeStamp = log.Last().Key;
            var now = stopwatch.ElapsedMilliseconds;
            var duration = now - lastTimeStamp;
            log.Add(now, info);
            outputChannel.Invoke($"Total:{now}ms; Step:{duration}ms: {info}");
            return this;
        }

        internal Dictionary<long, string> Stop()
        {
            this.stopwatch.Stop();
            var total = this.stopwatch.ElapsedMilliseconds;
            this.TotalMilliseconds = total;
            outputChannel.Invoke($"{total}ms: DONE");
            return this.log;
        }
    }
}
