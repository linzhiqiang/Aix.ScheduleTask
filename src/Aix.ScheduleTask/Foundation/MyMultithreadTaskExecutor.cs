using Aix.MultithreadExecutor;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aix.ScheduleTask.Foundation
{
    public class MyMultithreadTaskExecutor : MultithreadTaskExecutor
    {
        public MyMultithreadTaskExecutor(Action<MultithreadExecutorOptions> setupOptions) : base(setupOptions)
        {
        }
    }
}
