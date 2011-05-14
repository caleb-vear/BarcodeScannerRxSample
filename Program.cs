using System;
using System.Collections.Generic;
using System.Concurrency;
using System.Linq;
using System.Text;

namespace BarcodeScannerRx
{
    static class Program
    {
        static void Main(string[] args)
        {
            const string testInput = "^^123^6534345$$^^$^^^345345^23423$";

            Console.WriteLine("Initial Test, no delay");
            testInput.ToObservable(Scheduler.TaskPool)
                .ToBarcodeReadings()
                .Subscribe(WriteOutput);

            Console.ReadLine();
            Console.WriteLine("Type characters to process, press enter when you are done.");

            var userInput = new Subject<char>();

            userInput
                .ObserveOn(Scheduler.TaskPool)
                .ToBarcodeReadings()
                .Subscribe(WriteOutput);

            while(true)
            {
                var key = Console.ReadKey();

                if (key.Key == ConsoleKey.Enter)
                {
                    userInput.OnCompleted();
                    break;
                }

                userInput.OnNext(key.KeyChar);
            }
        }        

        static void WriteOutput(string output)
        {
            Console.WriteLine("\nReading: {0}", output == string.Empty ? "string.Empty" : output);
        }
    }
}
