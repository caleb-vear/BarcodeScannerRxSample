using System.Collections.Generic;
using System.Linq;
using System.Reactive.Testing;

namespace BarcodeScannerRx.Tests
{
    public static class RxTestExtensions
    {
        public static IEnumerable<Recorded<Notification<T>>> StartingAt<T>(this IEnumerable<Recorded<Notification<T>>> originalRecording, long sequenceStartTicks)
        {
            return originalRecording.Select(recording => new Recorded<Notification<T>>(recording.Time + sequenceStartTicks, recording.Value));
        }
    }
}