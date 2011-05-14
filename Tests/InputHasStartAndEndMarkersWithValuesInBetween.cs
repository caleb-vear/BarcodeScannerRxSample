using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Concurrency;
namespace BarcodeScannerRx.Tests
{
    [TestClass]
    public class InputHasStartAndEndMarkersWithValuesInBetween : BarcodeTest
    {
        protected override IEnumerable<IEnumerable<Recorded<Notification<char>>>> Given()
        {
            yield return "^hello$".ToRecordedNotifications().StartingAt(250);
            yield return EnumerableEx.Return(OnCompleted<char>(300));
        }

        protected override IEnumerable<Recorded<Notification<string>>> ExpectedOutput()
        {
            yield return OnNext(250, "hello");
            yield return OnCompleted<string>(300);
        }

        [TestMethod]
        public void SubscriptionCompletesAt300Ticks()
        {
            InputSequence.Subscriptions.AssertEqual(
                Subscribe(200, 300)
                );
        }
    }
}