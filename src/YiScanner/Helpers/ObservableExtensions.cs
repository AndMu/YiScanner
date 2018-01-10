using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace Wikiled.YiScanner.Helpers
{
    public static class ObservableExtensions
    {
        public static IObservable<T> RepeatAfterDelay<T>(this IObservable<T> source, TimeSpan delay, IScheduler scheduler)
        {
            var repeatSignal = Observable
                .Empty<T>()
                .Delay(delay, scheduler);

            // when source finishes, wait for the specified
            // delay, then repeat.
            return source.Concat(repeatSignal).Repeat();
        }
    }
}
