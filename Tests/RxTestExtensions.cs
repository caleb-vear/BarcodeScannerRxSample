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
            var allMessages = from messageSequence in messageSequences
                              from m in messageSequence
                              select m;

            // We have to explicitly say we want to use the TestSchedulerExtensions.CreateHotObservable method
            // so that end up calling back into ourselves and then stack overflow
            return TestSchedulerExtensions.CreateHotObservable(scheduler, allMessages.ToArray());
        }

        public static IEnumerable<Recorded<Notification<T>>> ToRecordedNotifications<T>(this IEnumerable<T> messages)
        {
            return messages.Select(m => new Recorded<Notification<T>>(0, new Notification<T>.OnNext(m)));
        }

        public static IEnumerable<Recorded<Notification<T>>> TimeBetween<T>(this IEnumerable<Recorded<Notification<T>>> recordings, long timeBetween)
        {
            return recordings.Select((r, i) => new Recorded<Notification<T>>(r.Time + timeBetween * i, r.Value));
        }

        public static IEnumerable<Recorded<Notification<T>>> StartingAt<T>(this IEnumerable<Recorded<Notification<T>>> originalRecording, long sequenceStartTicks)
        {
            return originalRecording.Select(recording => new Recorded<Notification<T>>(recording.Time + sequenceStartTicks, recording.Value));
        }
    }
}