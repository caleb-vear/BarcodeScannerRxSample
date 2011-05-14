using System.Collections.Generic;
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

            results.AssertEqual(EnumerableEx.Concat(
                OnNext(250, "hello"),
                OnCompleted<string>(300)
                ));

            inputSequence.Subscriptions.AssertEqual(
                Subscribe(200, 300)
                );
        }

        [TestMethod]
        public void InputHasMultipleStartsBeforeEndMarker()
        {
            var scheduler = new TestScheduler();
            var inputSequence = scheduler.CreateHotObservable(
                OnNextForAll(250, "^^blah^world$")
                );

            var results = scheduler.Run(() => inputSequence.ToBarcodeReadings());

            results.AssertEqual(EnumerableEx.Concat(
                OnNext(250, "world")
                ));

            inputSequence.Subscriptions.AssertEqual(
                Subscribe(200)
                );
        }

        [TestMethod]
        public void InputHasNoValidBarcodeSequence()
        {
            var scheduler = new TestScheduler();
            var inputSequence = scheduler.CreateHotObservable(
                OnNextForAll(250, "blah some text blah yay!"),
                OnCompleted<char>(500)
                );

            var results = scheduler.Run(() => inputSequence.ToBarcodeReadings());          

            results.AssertEqual(
                OnCompleted<string>(500)
                );

            inputSequence.Subscriptions.AssertEqual(
                Subscribe(200, 500)
            );
        }

        [TestMethod]
        public void InputSequenceHasStartButNoEndMarker()
        {
            var scheduler = new TestScheduler();
            var inputSequence = scheduler.CreateHotObservable(
                OnNextForAll(250, "blah^helloworld"),
                OnCompleted<char>(300)
                );

            var results = scheduler.Run(() => inputSequence.ToBarcodeReadings());

            results.AssertEqual(
                OnCompleted<string>(300)
                );

            inputSequence.Subscriptions.AssertEqual(
                Subscribe(200, 300)
            );
        }

        [TestMethod]
        public void InputSequenceHasEndButNoStartMarker()
        {
            var scheduler = new TestScheduler();
            var inputSequence = scheduler.CreateHotObservable(
                OnNextForAll(250, "HowAreYouToday?$xyz"),
                OnCompleted<char>(300)
                );

            var results = scheduler.Run(() => inputSequence.ToBarcodeReadings());

            results.AssertEqual(
                OnCompleted<string>(300)
                );

            inputSequence.Subscriptions.AssertEqual(
                Subscribe(200, 300)
                );
        }
    }
}