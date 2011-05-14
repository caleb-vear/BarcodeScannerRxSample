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
        public IEnumerable<Recorded<Notification<T>>> OnNextForAll<T>(long ticks, IEnumerable<T> values)
        {
            return values.Select(x => new Recorded<Notification<T>>(ticks, new Notification<T>.OnNext(x)));
        }

        public IEnumerable<Recorded<Notification<T>>> OnNext<T>(long ticks, T value)
        {
            var onNext = new Recorded<Notification<T>>(ticks, new Notification<T>.OnNext(value));
            return EnumerableEx.Return(onNext);
        }

        public IEnumerable<Recorded<Notification<T>>> OnCompleted<T>(long ticks)
        {
            var onCompleted = new Recorded<Notification<T>>(ticks, new Notification<T>.OnCompleted());
            return EnumerableEx.Return(onCompleted);
        }

        public IEnumerable<Recorded<Notification<T>>> OnError<T>(long ticks, Exception exception)
        {
            var onError = new Recorded<Notification<T>>(ticks,
                                                        new Notification<T>.OnError(exception));
            return EnumerableEx.Return(onError);
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