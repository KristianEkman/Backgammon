using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Backend
{
    public class Utils
    {
        public static async Task DoAfter(int ms, Action action, CancellationTokenSource cancellation)
        {
            await Task.Delay(ms);
            if (!cancellation.IsCancellationRequested)
                action.Invoke();
            return;
        }

        public static async Task RepeatEvery(int ms, Action action, CancellationTokenSource cancellation)
        {
            while (!cancellation.IsCancellationRequested)
            {
                await Task.Delay(ms);
                action.Invoke();
            }
        }
    }
}
