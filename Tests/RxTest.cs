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
        public IEnumerable<Recorded<Notification<T>>> OnNextForAll<T>(TimeSpan time, IEnumerable<T> values)
        {
            return OnNextForAll(time.Ticks, values);
        }

        public IEnumerable<Recorded<Notification<T>>> OnNextForAll<T>(long ticks, IEnumerable<T> values)
        {
            return values.Select(x => new Recorded<Notification<T>>(ticks, new Notification<T>.OnNext(x)));
        }

        public IEnumerable<Recorded<Notification<T>>> OnNext<T>(TimeSpan time, T value)
        {
            return OnNext(time.Ticks, value);
        }
            
        public IEnumerable<Recorded<Notification<T>>> OnNext<T>(long ticks, T value)
        {
            var onNext = new Recorded<Notification<T>>(ticks, new Notification<T>.OnNext(value));
            return EnumerableEx.Return(onNext);
        }

        public IEnumerable<Recorded<Notification<T>>> OnCompleted<T>(TimeSpan time)
        {
            return OnCompleted<T>(time.Ticks);
        }

        public IEnumerable<Recorded<Notification<T>>> OnCompleted<T>(long ticks)
        {
            var onCompleted = new Recorded<Notification<T>>(ticks, new Notification<T>.OnCompleted());
            return EnumerableEx.Return(onCompleted);
        }

        public IEnumerable<Recorded<Notification<T>>> OnError<T>(TimeSpan time, Exception exception)
        {
            return OnError<T>(time.Ticks, exception);
        }

        public IEnumerable<Recorded<Notification<T>>> OnError<T>(long ticks, Exception exception)
        {
            var onError = new Recorded<Notification<T>>(ticks,
                                                        new Notification<T>.OnError(exception));
            return EnumerableEx.Return(onError);
        }

        public Subscription Subscribe(TimeSpan start, TimeSpan end)
        {
            return Subscribe(start.Ticks, end.Ticks);
        }
            
        public Subscription Subscribe(long start, long end)
        {
            return new Subscription(start, end);
        }

        public Subscription Subscribe(TimeSpan start)
        {
            return Subscribe(start.Ticks);
        }
        
        public Subscription Subscribe(long start)
        {
            return new Subscription(start, 1000);
        }     
    }
}