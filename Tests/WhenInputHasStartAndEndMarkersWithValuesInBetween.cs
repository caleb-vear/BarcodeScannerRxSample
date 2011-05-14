using System.Collections.Generic;
using System.Linq;
using System.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Concurrency;
namespace BarcodeScannerRx.Tests
{
    [TestClass]
    public class WhenInputHasStartAndEndMarkersWithValuesInBetween : BarcodeTest
    {
        [TestMethod]
        public void OneReadingWasProduced()
        {
            var scheduler = new TestScheduler();

            var inputSequence = scheduler.CreateHotObservable(
                Input("^hello$", 0).StartingAt(250).ToArray()
                );

            var barcodeReadings = inputSequence.ToBarcodeReadings();

            var results = scheduler.Run(() => barcodeReadings);

            results.AssertEqual(
                OnNext(250, "hello")
                );
        }

        public IEnumerable<Recorded<Notification<T>>> Input<T>(IEnumerable<T> input, long ticksBetween)
        {
            long ticks = 0;
            foreach (var x in input)
            {
                yield return OnNext(ticks, x);
                ticks += ticksBetween;
            }
        }
    }
}