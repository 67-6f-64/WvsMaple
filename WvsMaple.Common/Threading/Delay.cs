using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Common.Threading
{
    public class Delay
    {

        public static async Task Execute(int delay, ThreadStart action)
        {
            await Task.Delay((int)delay).ContinueWith((x) => action());
        }

        public static async Task Execute(double delay, ThreadStart action)
        {
            await Task.Delay((int)delay).ContinueWith((x) => action());
        }

        private Timer t;

        public Delay(int delay, ThreadStart action)
        {
            t = new Timer(delay);

            t.Elapsed += new ElapsedEventHandler(delegate(object sender, ElapsedEventArgs e)
            {
                if (t != null)
                {
                    //t.Stop();
                    action();
                    //t.Dispose();
                }

                //t = null;
            });
        }

        public void Execute()
        {
            t.Start();
        }

        public void Cancel()
        {
            if (t != null)
            {
                t.Stop();
                t.Dispose();
            }

            t = null;
        }
    }
}
