using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Testing;
using System.Concurrency;
using System.Reactive.Testing.Mocks;

namespace BarcodeScannerRx.Tests
{
    public static class RxTestExtensions
    {
        public static HotObservable<T> CreateHotObservable<T>(this TestScheduler scheduler, params IEnumerable<Recorded<Notification<T>>>[] messageSequences)
        {
            // We have to explicitly say we want to use the TestSchedulerExtensions.CreateHotObservable method
            // so that end up calling back into ourselves and then stack overflow
            return TestSchedulerExtensions.CreateHotObservable(scheduler, messageSequences.Concat().ToArray());
        }

        public static Recorded<T> RecordedAt<T>(this T value, long ticks)
        {
            return new Recorded<T>(ticks, value);
        }

        public static IEnumerable<Recorded<T>> RecordAllAt<T>(this IEnumerable<T> messages, long ticks)
        {
            return messages.Select(m => new Recorded<T>(ticks, m));
        }

        public static IEnumerable<Recorded<T>> TimeBetweenEach<T>(this IEnumerable<Recorded<T>> recordings, TimeSpan timeBetween)
        {
            return recordings.Select((r, i) => new Recorded<T>(r.Time + timeBetween.Ticks * i, r.Value));
        }

        public static IEnumerable<Recorded<T>> StartingAt<T>(this IEnumerable<Recorded<T>> originalRecording, long sequenceStartTicks)
        {
            return originalRecording.Select(recording => new Recorded<T>(recording.Time + sequenceStartTicks, recording.Value));
        }
    }
}