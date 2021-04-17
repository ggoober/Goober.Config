using System.Threading.Tasks;
using System.Threading;

namespace Goober.Config.Api.Utils
{
    internal static class AsyncUtils
    {
        public static TResult RunSync<TResult>(this Task<TResult> task)
        {
            var taskFactory = new
                TaskFactory(CancellationToken.None,
                        TaskCreationOptions.None,
                        TaskContinuationOptions.None,
                        TaskScheduler.Default);

            return taskFactory.StartNew(() => task)
                .Unwrap()
                .GetAwaiter()
                .GetResult();
        }
    }
}
