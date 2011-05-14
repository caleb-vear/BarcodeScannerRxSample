using System;
using System.Collections.Generic;
using System.Concurrency;
using System.Linq;
using System.Text;

namespace BarcodeScannerRx
{
    static class Program
    {
        private static readonly ISubject<char> UserInput = new Subject<char>();

        static void Main(string[] args)
        {
            const string testInput = "^^123^6534345$$^^$^^^345345^23423$";

            Console.WriteLine("Initial Test, no delay");
            testInput.ToObservable(Scheduler.TaskPool)
                .ToBarcodeReadings()
                .Subscribe(WriteOutput);

            Console.ReadLine();
            Console.WriteLine("Type characters to process, press enter when you are done.");

            UserInput
                .ToBarcodeReadings()
                .Subscribe(WriteOutput);

            PumpUserInput();
        }

        private static void PumpUserInput()
        {
            while(true)
            {
                var key = Console.ReadKey();

                if (key.Key == ConsoleKey.Enter)
                {
                    UserInput.OnCompleted();
                    break;
                }

                UserInput.OnNext(key.KeyChar);
            }
        }

        static void WriteOutput(string output)
        {
            Console.WriteLine("\nReading: {0}", output == string.Empty ? "string.Empty" : output);
        }
    }
}
