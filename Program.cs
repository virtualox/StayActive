using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace StayActive
{
    class Program
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

        private const uint MOUSEEVENTF_MOVE = 0x0001;

        static void Main(string[] args)
        {
            Console.WriteLine("StayActive app is running. Press Ctrl+C to stop.");

            var cancellationTokenSource = new CancellationTokenSource();
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                eventArgs.Cancel = true;
                cancellationTokenSource.Cancel();
            };

            while (!cancellationTokenSource.Token.IsCancellationRequested)
            {
                mouse_event(MOUSEEVENTF_MOVE, 1, 1, 0, 0);
                SleepWithCancellation(TimeSpan.FromSeconds(1), cancellationTokenSource.Token);
                mouse_event(MOUSEEVENTF_MOVE, uint.MaxValue, uint.MaxValue, 0, 0);
                SleepWithCancellation(TimeSpan.FromMinutes(4), cancellationTokenSource.Token);
            }
        }

        private static void SleepWithCancellation(TimeSpan timeout, CancellationToken cancellationToken)
        {
            var sleepTask = Task.Delay(timeout, cancellationToken);
            try
            {
                sleepTask.Wait();
            }
            catch (AggregateException) when (cancellationToken.IsCancellationRequested)
            {
                // Task.Delay throws an AggregateException when canceled, which we can safely ignore
            }
        }
    }
}
