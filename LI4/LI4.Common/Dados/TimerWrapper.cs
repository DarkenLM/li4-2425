using System;
using System.Threading;

namespace LI4.Common.Dados;

/// <summary>
/// A wrapper class around <see cref="System.Threading.Timer"/> to provide controlled timer functionality.
/// The timer can be configured to execute a callback function once or repeatedly at a specified interval.
/// </summary>
/// <remarks>
/// For more information on the <see cref="System.Threading.Timer"/> class, refer to:
/// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.threading.timer?view=net-9.0">Microsoft Documentation</a>
/// </remarks>
public class TimerWrapper {
    private Timer _timer;
    private Action _callback;
    private int _delayMilliseconds;
    private bool _isLooping;

    /// <summary>
    /// Gets or sets the start time of the timer.
    /// </summary>
    public DateTime startTime { get; set;  }

    /// <summary>
    /// Initializes a new instance of the <see cref="TimerWrapper"/> class.
    /// </summary>
    /// <param name="dueTime">The time to wait before the callback is first invoked, in milliseconds.</param>
    /// <param name="callback">The callback function to be invoked when the timer elapses.</param>
    /// <param name="isLooping">Determines if the timer repeats itself after each callback invocation. Default is false.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the <paramref name="dueTime"/> is less than or equal to zero.</exception>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="callback"/> is null.</exception>
    public TimerWrapper(int dueTime, Action callback, bool isLooping = false) {
        if (dueTime <= 0) {
            throw new ArgumentOutOfRangeException(nameof(dueTime), "DueTime must be greater than zero.");
        }

        _callback = callback ?? throw new ArgumentNullException(nameof(callback), "Callback cannot be null.");
        _isLooping = isLooping;
        _delayMilliseconds = dueTime;

        _timer = new Timer(timerCallback, null, Timeout.Infinite, Timeout.Infinite);
    }

    /// <summary>
    /// Callback method invoked by the <see cref="System.Threading.Timer"/>.
    /// </summary>
    /// <param name="state">An optional state object passed to the callback (not used in this implementation).</param>
    /// <remarks>
    /// This method invokes the callback and stops the timer if it's not set to loop.
    /// </remarks>
    public void timerCallback(object state) {
        _callback.Invoke();

        if (!_isLooping) {
            dispose();
        }
    }

    /// <summary>
    /// Starts the timer, invoking the callback after the specified delay.
    /// </summary>
    /// <remarks>
    /// If the timer is set to loop, it will continue calling the callback at the specified interval.
    /// Otherwise, it will invoke the callback once after the initial delay.
    /// </remarks>
    public void start() {
        this.startTime = DateTime.Now;

        if (_isLooping) {
            _timer.Change(_delayMilliseconds, _delayMilliseconds);
        } else {
            _timer.Change(_delayMilliseconds, Timeout.Infinite);
        }
    }

    /// <summary>
    /// Stops the timer and prevents any further callback invocations.
    /// </summary>
    /// <remarks>
    /// If the timer is set to loop, this method stops the loop.
    /// </remarks>
    public void stop() {
        _timer?.Change(Timeout.Infinite, Timeout.Infinite);
    }

    /// <summary>
    /// Disposes the timer and releases all resources used by the current instance.
    /// </summary>
    /// <remarks>
    /// After calling this method, the timer cannot be reused.
    /// </remarks>
    public void dispose() {
        _timer?.Dispose();
        _timer = null;
    }
}
