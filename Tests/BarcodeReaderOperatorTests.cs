using System.Concurrency;
using System.Linq;
using System.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BarcodeScannerRx.Tests
{
    [TestClass]
    public class BarcodeReaderOperatorTests : BarcodeTest
    {
        [TestMethod]
        public void InputHasStartAndEndMarkersWithValuesInBetween()
        {
            var scheduler = new TestScheduler();
            var inputSequence = scheduler.CreateHotObservable(
                OnNextForAll(250, "^hello$"),
                OnCompleted<char>(300)
                );

            var results = scheduler.Run(() => inputSequence.ToBarcodeReadings());

            var expectedResults = EnumerableEx.Concat(
                OnNext(250, "hello"),
                OnCompleted<string>(300)
                );

            results.AssertEqual(expectedResults);
            inputSequence.Subscriptions.AssertEqual(
                Subscribe(200, 300)
                );
        }
    }
}