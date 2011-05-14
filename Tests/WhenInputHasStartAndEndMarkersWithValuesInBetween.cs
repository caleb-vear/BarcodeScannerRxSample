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
                    "^hello$".ToRecordedNotifications().StartingAt(250)
                );

            var barcodeReadings = inputSequence.ToBarcodeReadings();

            var results = scheduler.Run(() => barcodeReadings);

            results.AssertEqual(
                OnNext(250, "hello")
                );
        }
    }
}