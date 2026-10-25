using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SlugHub.ConsoleAppSample
{
    class Program
    {
        public static async Task Main()
        {
            Console.WriteLine("This sample will create a slug for a piece of text, with 10000 iterations");
            Console.WriteLine("Press any key to continue");
            Console.ReadLine();

            Console.Clear();

            var slugGenerator = new SlugGenerator();

            var stopwatch = Stopwatch.StartNew();

            var slugTasks = new List<Task<string>>();

            for (var i = 1; i <= 10000; i++)
            {
                var task = slugGenerator.GenerateSlugAsync("Some text that needs slugging " + i);
                slugTasks.Add(task);
            }

            var results = await Task.WhenAll(slugTasks);
            stopwatch.Stop();

            var slugGenerationTime = stopwatch.Elapsed.Milliseconds;

            foreach (var result in results)
            {
                Console.WriteLine(result);
            }

            Console.WriteLine();
            Console.WriteLine("Took " + slugGenerationTime + "ms to generate 10000 slugs");
            Console.WriteLine("");
            Console.WriteLine("Press enter to exit");
            Console.ReadLine();

            Environment.Exit(0);
        }
    }
}
