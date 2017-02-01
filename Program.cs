using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadPoolTest2
{
    public class Program
    {
        static int[] dopSet = { 1, 2, 16, 64, 512 };
        static bool[] booleans = { true, false };
        const int Column1 = 28;
        static TaskFactory nonTpFactory = new TaskFactory(
            CancellationToken.None,
            TaskCreationOptions.HideScheduler, TaskContinuationOptions.HideScheduler,
            new DedicatedThreadsTaskScheduler(dopSet[dopSet.Length - 1]));

        public static void Main(string[] args)
        {
            Console.WriteLine();

            long limit = 512 * 512;
            long multiplier = 10;
            if (args.Length > 0)
            {
                if (!long.TryParse(args[0], out multiplier))
                {
                    Console.WriteLine($"Please use an integer multiplier for ({limit} * multiplier) calls");
                    return;
                }
            }
            limit *= multiplier;

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

            await TestSetAsync("QUWI No Queues", (d, l) => QUWICallChainRepeat(d, l), batch, limit, sw);
            await TestSetAsync("SubTask Chain Return", (d, l) => SubTaskChainReturnRepeat(d, l), batch, limit, sw);
            await TestSetAsync("SubTask Chain Awaited", (d, l) => SubTaskChainAwaitedRepeat(d, l), batch, limit, sw);
            await TestSetAsync("SubTask Fanout Awaited", (d, l) => SubTaskFanoutAwaitedRepeat(d, l), batch, limit, sw);
            await TestSetAsync("Continuation Chain", (d, l) => ContinuationChainRepeat(d, l), batch, limit, sw);
            await TestSetAsync("Continuation Fanout", (d, l) => ContinuationFanoutRepeat(d, l), batch, limit, sw);
            await TestSetAsync("Yield Chain Awaited", (d, l) => YieldAwaitChainRepeat(d, l), batch, limit, sw);
            await TestSetAsync("Async Chain Awaited", (d, l) => AsyncChainAwaitedRepeat(d, l), batch, limit, sw);
            await TestSetAsync("Async Chain Return", (d, l) => AsyncChainReturnRepeat(d, l), batch, limit, sw);
            await TestSetAsync("Sync Chain Awaited", (d, l) => SyncChainAwaitedRepeat(d, l), batch, limit, sw);
            await TestSetAsync("CachedTask Chain Await", (d, l) => CachedTaskChainAwaitedRepeat(d, l), batch, limit, sw);
            await TestSetAsync("CachedTask Chain Check", (d, l) => CachedTaskChainCheckedRepeat(d, l), batch, limit, sw);
            await TestSetAsync("CachedTask Chain Return", (d, l) => CachedTaskChainReturnRepeat(d, l), batch, limit, sw);
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

        private static Task QUWICallChainRepeat(int depth, long count)
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

        [StructLayout(LayoutKind.Explicit, Size = 128)]
        struct Counter
        {
            [FieldOffset(64)]
            public long Count;
        }

        private static Task QueueUserWorkItemState(long count)
        {
            var obj = new object();
            var semaphore = new SemaphoreSlim(0);
            var remaining = new Counter() { Count = count };
            for (var i = 0; i < count; i++)
            {
                ThreadPool.QueueUserWorkItem((o) => { var c = Interlocked.Decrement(ref remaining.Count); if (c == 0) semaphore.Release(); }, obj);
            }
            return semaphore.WaitAsync();
        }

        private static async Task YieldAwaitChainRepeat(int depth, long count)
        {
            var total = count / depth;
            for (var i = 0L; i < total; i++)
            {
                await YieldAwaitChain(depth - 1);
            }
        }

        private async static Task YieldAwaitChain(int depth)
        {
            if (depth == 0)
            {
                await Task.Yield();
                return;
            }

            var t = YieldAwaitChain(depth - 1);
            await Task.Yield();
            await t;
        }

        private static async Task SyncChainAwaitedRepeat(int depth, long count)
        {
            var total = count / depth;
            for (var i = 0L; i < total; i++)
            {
                await SyncChainAwaited(depth - 1);
            }
        }
        private async static Task SyncChainAwaited(int depth)
        {
            if (depth == 0)
            {
                return;
            }

            await SyncChainAwaited(depth - 1);
        }

        private static async Task CachedTaskChainAwaitedRepeat(int depth, long count)
        {
            var total = count / depth;
            for (var i = 0L; i < total; i++)
            {
                await CachedTaskChainAwaitedRepeat(depth - 1);
            }
        }

        private static async Task CachedTaskChainAwaitedRepeat(int depth)
        {
            if (depth == 0)
            {
                await Task.CompletedTask;
                return;
            }

            await CachedTaskChainAwaitedRepeat(depth - 1);
        }

        private static async Task CachedTaskChainCheckedRepeat(int depth, long count)
        {
            var total = count / depth;
            for (var i = 0L; i < total; i++)
            {
                await CachedTaskChainChecked(depth - 1);
            }
        }

        private static Task CachedTaskChainChecked(int depth)
        {
            if (depth == 0)
            {
                return Task.CompletedTask;
            }

            var task = CachedTaskChainChecked(depth - 1);
            if (task.Status == TaskStatus.RanToCompletion)
            {
                return task;
            }
            return TaskAwaited(task);
        }

        private static async Task CachedTaskChainReturnRepeat(int depth, long count)
        {
            var total = count / depth;
            for (var i = 0L; i < total; i++)
            {
                await CachedTaskChainReturn(depth - 1);
            }
        }

        private static Task CachedTaskChainReturn(int depth)
        {
            if (depth == 0)
            {
                return Task.CompletedTask;
            }

            return CachedTaskChainReturn(depth - 1);
        }

        private async static Task AsyncChainAwaitedRepeat(int depth, long count)
        {
            var total = count / depth;
            var semaphore = new SemaphoreSlim(0);
            for (var i = 0; i < total; i++)
            {
                var t = AsyncChainAwaited(depth - 1, semaphore.WaitAsync());
                semaphore.Release();
                await t;
            }
        }

        private static async Task AsyncChainAwaited(long depth, Task task)
        {
            if (depth == 0)
            {
                await task;
                return;
            }

            await AsyncChainAwaited(depth - 1, task);
        }

        private async static Task AsyncChainReturnRepeat(int depth, long count)
        {
            var total = count / depth;
            var semaphore = new SemaphoreSlim(0);
            for (var i = 0; i < total; i++)
            {
                var t = AsyncChainReturn(depth - 1, semaphore.WaitAsync());
                semaphore.Release();
                await t;
            }
        }

        private static Task AsyncChainReturn(long depth, Task task)
        {
            if (depth == 0)
            {
                return TaskAwaited(task);
            }

            return AsyncChainReturn(depth - 1, task);
        }
        private static async Task TaskAwaited(Task task)
        {
            await task;
        }

        private static async Task SubTaskChainReturnRepeat(int depth, long count)
        {
            var total = count / depth;
            for (var i = 0L; i < total; i++)
            {
                await SubTaskChainAwaited(depth - 1);
            }
        }

        private static Task SubTaskChainAwaited(int depth)
        {
            if (depth == 0)
            {
                return Task.Run(() => Task.CompletedTask);
            }

            return Task.Run(() => SubTaskChainAwaited(depth - 1));
        }

        private static async Task SubTaskChainAwaitedRepeat(int depth, long count)
        {
            var total = count / depth;
            for (var i = 0L; i < total; i++)
            {
                await SubTaskAwaitedAsync(depth - 1);
            }
        }

        private async static Task SubTaskAwaitedAsync(int depth)
        {
            if (depth == 0)
            {
                await Task.Run(() => Task.CompletedTask);
                return;
            }

            await Task.Run(() => SubTaskAwaitedAsync(depth - 1));
        }

        private static async Task SubTaskFanoutAwaitedRepeat(int depth, long count)
        {
            var total = count / depth;
            for (var i = 0L; i < total; i++)
            {
                await SubTaskFanoutAwaited(depth);
            }
        }

        static Func<Task> YieldAction = async () => await Task.Yield();
        private async static Task SubTaskFanoutAwaited(int depth)
        {
            var tasks = new Task[depth];

            for (var i = 0; i < tasks.Length; i++)
            {
                tasks[i] = Task.Run(YieldAction);
            }

            await Task.WhenAll(tasks);
        }

        private static async Task ContinuationChainRepeat(int depth, long count)
        {
            var total = count / depth;
            for (var i = 0L; i < total; i++)
            {
                await ContinuationChain(depth);
            }
        }

        private static Task ContinuationChain(int depth)
        {
            var task = Task.Run(YieldAction);

            for (var i = 0; i < depth; i++)
            {
                task = task.ContinueWith((t) => { }, TaskContinuationOptions.RunContinuationsAsynchronously);
            }

            return task;
        }

        private static async Task ContinuationFanoutRepeat(int depth, long count)
        {
            var total = count / depth;
            for (var i = 0L; i < total; i++)
            {
                await ContinuationFanout(depth);
            }
        }

        private async static Task ContinuationFanout(int depth)
        {
            var tasks = new Task[depth];
            var task = Task.Run(YieldAction);

            for (var i = 0; i < depth; i++)
            {
                tasks[i] = task.ContinueWith((t) => { }, TaskContinuationOptions.RunContinuationsAsynchronously);
            }

            await Task.WhenAll(tasks);
        }

        // Testing stuff

        private static async Task TestSetAsync(string testName, Func<int, long, Task> testAsync, int batchBeforeGC, long total, Stopwatch sw)
        {
            foreach (var dopFromPool in booleans)
            {
                foreach (var depth in dopSet)
                {
                    if (depth == 1)
                    {
                        string name = dopFromPool ? testName + " (TP)" : testName;
                        Console.Write(name.Substring(0, Math.Min(Column1, name.Length)) + new string(' ', Math.Max(0, Column1 - name.Length)));
                    }
                    else
                    {
                        var depthText = $"- Depth {depth,4}";
                        Console.Write(depthText + new string(' ', Column1 - depthText.Length));
                    }
                    for (var i = 0; i < dopSet.Length; i++)
                    {
                        await TestAsync(testName, testAsync, batchBeforeGC, depth, dopSet[i], dopFromPool, total, sw);
                    }
                    Console.WriteLine();
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        private static async Task TestAsync(string testName, Func<int, long, Task> testAsync, int batchBeforeGC, int depth, int dop, bool dopFromPool, long total, Stopwatch sw)
        {
            await testAsync(depth, dopSet[dopSet.Length -1] * dopSet[dopSet.Length - 1]);

            var loops = Math.Max(total / batchBeforeGC, 1);
            GC.Collect();
            sw.Reset();
            for (var i = 0; i < loops; i++)
            {
                GC.Collect();
                var gcLatency = GCSettings.LatencyMode;
                GCSettings.LatencyMode = GCLatencyMode.LowLatency;
                sw.Start();
                await ParallelTestAsync(testAsync, batchBeforeGC, depth, dop, dopFromPool);
                sw.Stop();
                GCSettings.LatencyMode = gcLatency;
            }
            sw.Stop();
            var time = $"{PerSecond(total / sw.Elapsed.TotalSeconds)}";
            Console.Write(new string(' ', 12 - time.Length) + time);
        }

        private async static Task ParallelTestAsync(Func<int, long, Task> testAsync, long count, int depth, int dop, bool dopFromPool)
        {
            var tasks = new Task[dop];
            var subCount = Math.Max(1L, count / dop);

            for (var i = 0; i < dop; i++)
            {
                Action body = () => testAsync(depth, subCount);
                tasks[i] = dopFromPool ? Task.Run(body) : nonTpFactory.StartNew(body);
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

        private sealed class DedicatedThreadsTaskScheduler : TaskScheduler
        {
            private readonly BlockingCollection<Task> _tasks = new BlockingCollection<Task>();

            public DedicatedThreadsTaskScheduler(int numThreads)
            {
                for (int i = 0; i < numThreads; i++)
                {
                    new Thread(() =>
                    {
                        foreach (Task t in _tasks.GetConsumingEnumerable()) TryExecuteTask(t);
                    })
                    { IsBackground = true }.Start();
                }
            }

            protected override void QueueTask(Task task) => _tasks.Add(task);

            protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued) => TryExecuteTask(task);

            protected override IEnumerable<Task> GetScheduledTasks() => _tasks;
        }
    }
}
