using Polly;
using Polly.Timeout;

namespace LearnPolly_Resilency_
{
    internal class Program
    {
        // time out pattern
        static async Task Main(string[] args)
        {
            // Define a timeout policy of 3 seconds
            var timeoutPolicy = Policy
                .TimeoutAsync(3, TimeoutStrategy.Pessimistic, onTimeoutAsync: (context, timespan, task, ex) =>
                {
                    Console.WriteLine($"Execution timed out after {timespan.TotalSeconds} seconds.");
                    return Task.CompletedTask;
                });

            try
            {
                await timeoutPolicy.ExecuteAsync(async () =>
                {
                    Console.WriteLine("Starting operation...");
                    await Task.Delay(1000); // Simulate long work (5s)
                    Console.WriteLine("Finished operation");
                });
            }
            catch (TimeoutRejectedException)
            {
                Console.WriteLine("Operation was cancelled due to timeout.");
            }
        }
        // Circuit breaker
        //static void Main(string[] args)
        //{
        //    // Circuit breaker: break after 2 consecutive failures, stay open for 5 seconds
        //    var circuitBreaker = Policy
        //        .Handle<Exception>()
        //        .CircuitBreaker(2, TimeSpan.FromSeconds(5),
        //            onBreak: (ex, ts) => Console.WriteLine($"Circuit opened due to: {ex.Message}"),
        //            onReset: () => Console.WriteLine("Circuit closed again"),
        //            onHalfOpen: () => Console.WriteLine("Circuit is half-open; next call is a trial"));

        //    for (int i = 1; i <= 10; i++)
        //    {
        //        try
        //        {
        //            circuitBreaker.Execute(() =>
        //            {
        //                Console.WriteLine($"Attempt {i}");
        //                DoWork(i);
        //            });
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine($"Operation failed: {ex.Message}");
        //        }

        //        System.Threading.Thread.Sleep(1000);
        //    }
        //}


        static void DoWork()
        {
            // Simulate random failure
            if (new Random().Next(1, 20) != 3)  // succeeds 1 in 3 times
            {
                throw new Exception("Something went wrong!");
            }
            Console.WriteLine("Work succeeded!");
        }
        static void DoWork(int attempt)
        {
            // Fail for first 6 attempts
            if (attempt <= 6)
                throw new Exception("Simulated failure");

            Console.WriteLine("Work succeeded!");
        }
    }
}


// Simple retry
//static void Main(string[] args)
//{
//    var retrypoli = Policy.Handle<Exception>()
//                        .Retry(5, (exc, att) =>
//                        {
//                            Console.WriteLine($"Retry {att} because: {exc.Message}");
//                            Thread.Sleep(5000);
//                        });
//    retrypoli.Execute(() =>
//    {
//        Console.WriteLine("Starting operation");
//        DoWork();
//    });
//    Console.WriteLine("Hello, World!");
//}

// retry with exponential
//static void Main(string[] args)
//{
//    var retryPolicy = Policy
//        .Handle<Exception>()
//        .WaitAndRetry(
//            retryCount: 8,  // total retries
//            sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)), // 2s, 4s, 8s, 16s, 32s
//            onRetry: (exception, timespan, attempt, context) =>
//            {
//                Console.WriteLine($"Retry {attempt} after {timespan.TotalSeconds}s due to: {exception.Message}");
//            });

//    retryPolicy.Execute(() =>
//    {
//        Console.WriteLine("Trying operation...");
//        DoWork();
//    });
//}