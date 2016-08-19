using System.Threading.Tasks;

namespace Exceedra.Common
{
    public static class TaskFactoryEx
    {
        public static Task<T> FromResult<T>(this TaskFactory factory, T result)
        {
            var tcs = new TaskCompletionSource<T>();
            tcs.SetResult(result);
            return tcs.Task;
        }

        public static Task Completed(this TaskFactory factory)
        {
            var tcs = new TaskCompletionSource<int>();
            tcs.SetResult(0);
            return tcs.Task;
        }
    }
}