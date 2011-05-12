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

        static IObservable<string> ToBarcodeReadings(this IObservable<char> input)
        {
            input = input
                .Publish() // My test uses a cold observable so multiple subscriptions get the full sequence, publish fixes that.
                .RefCount(); // Ref cound means that once we have no more subscribers kill the subscription to the base input.

            var timeOut = input.Select(_ => Observable.Interval(TimeSpan.FromSeconds(5)))
                .Switch()
                .Do(_ => Console.WriteLine("\nInput Timed Out"));

            var sequenceStarts = input.Where(c => c == '^');
            var sequenceEnds = input.Where(c => c == '$');

            var readings = from start in sequenceStarts
                            select input
                                .TakeUntil(sequenceEnds)
                                .Aggregate("", (c1, c2) => c1 + c2)
                                .TakeUntil(timeOut); // I don't use TimeOut because I don't want the TimeoutException

            // Switch turns an IObservable<IObservable<T>> into IObservable<T> always taking from the last IObservable<T> that was pushed through.
            return readings.Switch();
        }

        static void WriteOutput(string output)
        {
            Console.WriteLine("\nReading: {0}", output == string.Empty ? "string.Empty" : output);
        }
    }
}
