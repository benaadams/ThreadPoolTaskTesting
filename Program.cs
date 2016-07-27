using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadPoolTest2
{
    public class Program
    {
        static int[] dopSet = { 1, 2, 16, 64, 512 };
        const int Column1 = 22;

        public static void Main(string[] args)
        {
            Console.WriteLine();

            long limit = 512 * 512;
            if (args.Length > 0)
            {
                long multiplier;
                if (long.TryParse(args[0], out multiplier))
                {
                    limit *= multiplier;
                }
                else
                {
                    Console.WriteLine($"Please use an integer multiplier for ({limit} * multiplier) calls");
                    return;
                }
            }

            int batch = 512 * 512;


            Console.WriteLine($"Testing {limit:N0} calls, with GCs after {batch:N0} calls.");
            Console.WriteLine($"Operations per second on {Environment.ProcessorCount} Cores");

            MainAsync(limit, batch).Wait();
        }

        public static async Task MainAsync(long limit, int batch)
        {
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;

            var sw = Stopwatch.StartNew();

            Console.WriteLine(new string(' ', Column1 + (12 * dopSet.Length) - "Parallelism".Length) + "Parallelism");
            Console.Write(new string(' ', Column1));
            foreach (var dop in dopSet)
            {
                var title = $"{(dop == 1 ? "Serial" : $"{dop}x")}";

                Console.Write(new string(' ', 12 - title.Length) + title);
            }
            Console.WriteLine();

            await TestSetAsync("QUWI No Queues", (d, l) => QUWICallChain(d, l), batch, limit, sw);
            await TestSetAsync("SubTasks", (d, l) => SubTaskChain(d, l), batch, limit, sw);
            await TestSetAsync("QUWI Local Queues", (d, l) => QUWICallChain(d, l), batch, limit, sw);
            await TestSetAsync("Yielding Await", (d, l) => YieldingAwaitChain(d, l), batch, limit, sw);
            await TestSetAsync("Async Awaited", (d, l) => AsyncAwaitedChain(d, l), batch, limit, sw);
            await TestSetAsync("Async PassThrough", (d, l) => AsyncPassThroughChain(d, l), batch, limit, sw);
            await TestSetAsync("Completed Awaited", (d, l) => CompletedAwaitedChain(d, l), batch, limit, sw);
            await TestSetAsync("CachedTask Awaited", (d, l) => CachedTaskAwaitedChain(d, l), batch, limit, sw);
            await TestSetAsync("CachedTask CheckAwait", (d, l) => CachedTaskCheckAwaitChain(d, l), batch, limit, sw);
            await TestSetAsync("CachedTask PassThrough", (d, l) => CachedTaskPassThroughChain(d, l), batch, limit, sw);

        }

        private class QUWICounter
        {
            public SemaphoreSlim semaphore;
            public long remaining;
        }

        private class QUWIState
        {
            public QUWICounter counter;
            public int depth;
        }

        static WaitCallback QUWICallback = QUWI;

        private static void QUWI(object o)
        {
            var state = (QUWIState)o;
            if (state.depth > 0)
            {
                ThreadPool.QueueUserWorkItem(QUWICallback, new QUWIState() { counter = state.counter, depth = state.depth - 1 });
            }

            var c = Interlocked.Decrement(ref state.counter.remaining);

            if (c == 0) state.counter.semaphore.Release();
        }

        private static Task QUWICallChain(int depth, long count)
        {
            var total = count / depth;
            var semaphore = new SemaphoreSlim(0);
            var remaining = count;

            var state = new QUWIState()
            {
                counter = new QUWICounter()
                {
                    semaphore = semaphore,
                    remaining = remaining
                },
                depth = depth - 1
            };

            for (var i = 0; i < total; i++)
            {
                ThreadPool.QueueUserWorkItem(QUWICallback, state);
            }

            return semaphore.WaitAsync();
        }

        private static Task QueueUserWorkItemState(long count)
        {
            var obj = new object();
            var semaphore = new SemaphoreSlim(0);
            var remaining = count;
            for (var i = 0; i < count; i++)
            {
                ThreadPool.QueueUserWorkItem((o) => { var c = Interlocked.Decrement(ref remaining); if (c == 0) semaphore.Release(); }, obj);
            }
            return semaphore.WaitAsync();
        }

        private static async Task YieldingAwaitChain(int depth, long count)
        {
            var total = count / depth;
            for (var i = 0L; i < total; i++)
            {
                await YieldingAwait(depth - 1);
            }
        }

        private async static Task YieldingAwait(int depth)
        {
            if (depth == 0)
            {
                await Task.Yield();
                return;
            }

            await YieldingAwait(depth - 1);
        }

        private static async Task CompletedAwaitedChain(int depth, long count)
        {
            var total = count / depth;
            for (var i = 0L; i < total; i++)
            {
                await CompletedAwaited(depth - 1);
            }
        }
        private async static Task CompletedAwaited(int depth)
        {
            if (depth == 0)
            {
                await Task.CompletedTask;
                return;
            }

            await CompletedAwaited(depth - 1);
        }

        private static async Task CachedTaskAwaitedChain(int depth, long count)
        {
            var total = count / depth;
            for (var i = 0L; i < total; i++)
            {
                await CachedTaskAwaitedAsync(depth - 1);
            }
        }

        private static async Task CachedTaskAwaitedAsync(int depth)
        {
            if (depth == 0)
            {
                await Task.CompletedTask;
                return;
            }

            await CachedTaskAwaitedAsync(depth - 1);
        }

        private static async Task CachedTaskCheckAwaitChain(int depth, long count)
        {
            var total = count / depth;
            for (var i = 0L; i < total; i++)
            {
                await CachedTaskCheckAwait(depth - 1);
            }
        }

        private static Task CachedTaskCheckAwait(int depth)
        {
            if (depth == 0)
            {
                return Task.CompletedTask;
            }

            var task = CachedTaskCheckAwait(depth - 1);
            if (task.Status == TaskStatus.RanToCompletion)
            {
                return task;
            }
            return TaskAwaited(task);
        }

        private static async Task CachedTaskPassThroughChain(int depth, long count)
        {
            var total = count / depth;
            for (var i = 0L; i < total; i++)
            {
                await CachedTaskPassThrough(depth - 1);
            }
        }

        private static Task CachedTaskPassThrough(int depth)
        {
            if (depth == 0)
            {
                return Task.CompletedTask;
            }

            return CachedTaskPassThrough(depth - 1);
        }

        private async static Task AsyncAwaitedChain(int depth, long count)
        {
            var total = count / depth;
            var semaphore = new SemaphoreSlim(0);
            for (var i = 0; i < total; i++)
            {
                var t = AsyncAwaited(depth - 1, semaphore.WaitAsync());
                semaphore.Release();
                await t;
            }
        }

        private static async Task AsyncAwaited(long depth, Task task)
        {
            if (depth == 0)
            {
                await task;
                return;
            }

            await AsyncAwaited(depth - 1, task);
        }

        private async static Task AsyncPassThroughChain(int depth, long count)
        {
            var total = count / depth;
            var semaphore = new SemaphoreSlim(0);
            for (var i = 0; i < total; i++)
            {
                var t = AsyncPassThrough(depth - 1, semaphore.WaitAsync());
                semaphore.Release();
                await t;
            }
        }

        private static Task AsyncPassThrough(long depth, Task task)
        {
            if (depth == 0)
            {
                return TaskAwaited(task);
            }

            return AsyncPassThrough(depth - 1, task);
        }
        private static async Task TaskAwaited(Task task)
        {
            await task;
        }

        private static async Task SubTaskChain(int depth, long count)
        {
            var total = count / depth;
            for (var i = 0L; i < total; i++)
            {
                await SubTaskAsync(depth - 1);
            }
        }

        private static Task SubTaskAsync(int depth)
        {
            if (depth == 0)
            {
                return Task.CompletedTask;
            }

            return Task.Run(() => SubTaskAsync(depth - 1));
        }

        // Testing stuff

        private static async Task TestSetAsync(string testName, Func<int, long, Task> testAsync, int batchBeforeGC, long total, Stopwatch sw)
        {
            foreach (var depth in dopSet)
            {
                if (depth == 1)
                {
                    Console.Write(testName.Substring(0, Math.Min(Column1, testName.Length)) + new string(' ', Math.Max(0, Column1 - testName.Length)));
                }
                else
                {
                    var depthText = $"- Depth {depth,4}";
                    Console.Write(depthText + new string(' ', Column1 - depthText.Length));
                }
                for (var i = 0; i < dopSet.Length; i++)
                {
                    await TestAsync(testName, testAsync, batchBeforeGC, depth, dopSet[i], total, sw);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        private static async Task TestAsync(string testName, Func<int, long, Task> testAsync, int batchBeforeGC, int depth, int dop, long total, Stopwatch sw)
        {
            await testAsync(depth, dopSet[dopSet.Length -1] * dopSet[dopSet.Length - 1]);

            var loops = Math.Max(total / batchBeforeGC, 1);
            GC.Collect();
            sw.Reset();
            for (var i = 0; i < loops; i++)
            {
                GC.Collect();
                sw.Start();
                await ParallelTestAsync(testAsync, batchBeforeGC, depth, dop);
                sw.Stop();
            }
            sw.Stop();
            var time = $"{PerSecond(total / sw.Elapsed.TotalSeconds)}";
            Console.Write(new string(' ', 12 - time.Length) + time);
        }

        private async static Task ParallelTestAsync(Func<int, long, Task> testAsync, long count, int depth, int dop)
        {
            if (dop == 1)
            {
                await testAsync(depth, count);
                return;
            }

            var tasks = new Task[dop];
            var subCount = Math.Max(1L, count / dop);

            for (var i = 0; i < dop; i++)
            {
                tasks[i] = Task.Run(()=> testAsync(depth, subCount));
            }
            for (var i = 0; i < dop; i++)
            {
                await tasks[i];
            }
        }

        private static string PerSecond(double perSecond)
        {
            if (perSecond > 1000.0 * 1000 * 1000 * 1000)
            {
                return (perSecond / (1000.0 * 1000 * 1000 * 1000)).ToString("#0.000") + " T";
            }
            if (perSecond > 1000.0 * 1000 * 1000)
            {
                return (perSecond / (1000.0 * 1000 * 1000)).ToString("#0.000") + " B";
            }
            if (perSecond > 1000.0 * 1000)
            {
                return (perSecond / 1000000.0).ToString("#0.000") + " M";
            }
            if (perSecond > 1000.0)
            {
                return (perSecond / 1000.0).ToString("#0.000") + " k";
            }
            return (perSecond / 1000.0).ToString("#0.000") + "  ";
        }
    }
}
