using System;
using System.Linq;

namespace BarcodeScannerRx
{
    public static class BarcodeReaderOperators
    {
        public static IObservable<string> ToBarcodeReadings(this IObservable<char> input)
        {
            input = input
                .Publish() // My test uses a cold observable so multiple subscriptions get the full sequence, publish fixes that.
                .RefCount(); // Ref cound means that once we have no more subscribers kill the subscription to the base input.

            var timeOut = input.Select(_ => Observable.Interval(TimeSpan.FromSeconds(5)))
                .Switch();

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
    }
}