using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace HttpClientTimeouts.Core
{
    public class Processor
    {
        private const int DegreeOfPrallelism = 50;
        private readonly IExternalService _service;
        private Stopwatch _sw;
        public Processor(IExternalService service)
        {
            _service = service;
        }

        public void StartProcessing()
        {
            _sw = Stopwatch.StartNew();

            // Uncomment 1 at a time.

            //1. with 50 parallels - finished in 88287
            SemaphoreSlimSelect(100_000);

            ////2. with 50 parallels - finished in 85095 
            //TransformBlockForEach(100_000);

            ////3. Finished in 161549 (2:41)
            //ParallelFor(100_000);

            ////4. Tons of errors. stopped after about 3 mins
            //ParallelForWithAsync(100_000);

            ////5. tons of errors stopped after 5 mins.
            //TaskWaitAll(100_000);

            ////6. No errors. runs forever. This is the control test.
            //ForEach(100_000);

        }

        private void SemaphoreSlimSelect(int maxCount)
        {
            SemaphoreSlim mutex = new SemaphoreSlim(DegreeOfPrallelism);
            IEnumerable<int> arr = Enumerable.Range(1, maxCount);
            var tasks = arr.Select(async i =>
            {
                await mutex.WaitAsync();
                try
                {
                    await CallSendAsync(i);
                }
                finally
                {
                    mutex.Release();
                }
            });

            Task.WhenAll(tasks).GetAwaiter().GetResult();
        }

        private void TransformBlockForEach(int maxCount)
        {
            IEnumerable<int> arr = Enumerable.Range(1, maxCount);

            TransformBlock<int, object> callSendAsyncBlock = new TransformBlock<int, object>(async i =>
            {
                await CallSendAsync(i);
                return null;
            }, new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = DegreeOfPrallelism
            });

            foreach (var i in arr)
            {
                callSendAsyncBlock.Post(i);
            }

            callSendAsyncBlock.Complete();
        }

        private void ForEach(int maxCount)
        {
            IEnumerable<int> arr = Enumerable.Range(1, maxCount);
            foreach (var i in arr)
            {
                CallSendAsync(i).GetAwaiter().GetResult();
            }
        }

        private void ParallelFor(int maxCount)
        {
            Parallel.For(0, maxCount, i =>
            {
                CallSendAsync(i).GetAwaiter().GetResult();
            });
        }

        private void ParallelForWithAsync(int maxCount)
        {
            Parallel.For(0, maxCount, async i =>
            {
                await CallSendAsync(i);
            });
        }

        private void TaskWaitAll(int maxCount)
        {
            IEnumerable<int> arr = Enumerable.Range(1, maxCount);
            Task.WhenAll(
                    arr
                        .Select(CallSendAsync))
                .GetAwaiter()
                .GetResult();
        }

        private async Task CallSendAsync(int iteration)
        {
            Stopwatch sw = Stopwatch.StartNew();
            try
            {
                // send some payload
                await _service.SendAsync(Encoding.ASCII.GetBytes(Guid.NewGuid().ToString()));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex}");
                Console.WriteLine("\n\n\n");
            }
            finally
            {
                Console.WriteLine($"Iteration {iteration}, Duration {sw.ElapsedMilliseconds}, Total Duration {_sw.ElapsedMilliseconds}");
            }

        }

    }
}
