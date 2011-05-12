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
            Console.WriteLine("Delay in sequence test");

            var firstPart = testInput.Substring(0, 8).ToObservable(Scheduler.TaskPool);
            var secondPart = testInput.Substring(8).ToObservable(Scheduler.TaskPool).Delay(TimeSpan.FromSeconds(6));

            firstPart
                .Concat(secondPart)
                .ToBarcodeReadings()
                .Subscribe(WriteOutput);

            Console.ReadLine();
        }

        static IObservable<string> ToBarcodeReadings(this IObservable<char> input)
        {
            input = input
                .Publish() // My test uses a cold observable so multiple subscriptions get the full sequence, publish fixes that.
                .RefCount(); // Ref cound means that once we have no more subscribers kill the subscription to the base input.

            var timeOut = Observable.Interval(TimeSpan.FromSeconds(5));
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
            Console.WriteLine("Reading: {0}", output == string.Empty ? "string.Empty" : output);
        }
    }
}
