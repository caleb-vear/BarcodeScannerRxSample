using System;
using System.Collections.Generic;
using System.Concurrency;
using System.Linq;
using System.Reactive.Testing;
using System.Reactive.Testing.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BarcodeScannerRx.Tests
{    
    public abstract class BarcodeTest
    {
        protected TestScheduler Scheduler { get; private set; }
        public HotObservable<char> InputSequence { get; private set; }
        public IEnumerable<Recorded<Notification<string>>> Results { get; private set; }

        protected abstract IEnumerable<IEnumerable<Recorded<Notification<char>>>> Given();

        protected abstract IEnumerable<Recorded<Notification<string>>> ExpectedOutput();

        [TestInitialize]
        public void When()
        {
            Scheduler = new TestScheduler();

            var messages = from mSequence in Given()
                           from m in mSequence
                           select m;

            InputSequence = Scheduler.CreateHotObservable(messages.ToArray());

            Results = Scheduler.Run(() => InputSequence.ToBarcodeReadings());
        }

        [TestMethod]
        public void OutputIsAsExpected()
        {
            Results.AssertEqual(ExpectedOutput());
        }

        public Recorded<Notification<T>> OnNext<T>(long ticks, T value)
        {
            return new Recorded<Notification<T>>(ticks, new Notification<T>.OnNext(value));
        }

        public Recorded<Notification<T>> OnCompleted<T>(long ticks)
        {
            return new Recorded<Notification<T>>(ticks, new Notification<T>.OnCompleted());
        }

        public Recorded<Notification<T>> OnError<T>(long ticks, Exception exception)
        {
            return new Recorded<Notification<T>>(ticks,
                                                 new Notification<T>.OnError(exception));
        }

        public Subscription Subscribe(long start, long end)
        {
            return new Subscription(start, end);
        }

        public Subscription Subscribe(long start)
        {
            return new Subscription(start);
        }     
    }
}