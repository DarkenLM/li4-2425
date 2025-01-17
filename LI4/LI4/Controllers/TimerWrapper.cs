using System;
using System.Threading;

namespace LI4.Controllers;

public class TimerWrapper : IDisposable {
    private Timer _timer;
    private Action _callback;
    private int _delayMilliseconds;
    private bool _isLooping;

    /**
     * This class creates an instance of the timer
     * The calback value is the function that is to be run every x miliseconds.
     * This x miliseconds are defined by dueTime.
     * The isLooping defines if the timer loops or not
     */
    public TimerWrapper(int dueTime, Action callback, bool isLooping = false) {
        if (dueTime <= 0) {
            throw new ArgumentOutOfRangeException(nameof(dueTime), "DueTime must be greater than zero.");
        }

        _callback = callback ?? throw new ArgumentNullException(nameof(callback), "Callback cannot be null.");
        _isLooping = isLooping;
        _delayMilliseconds = dueTime;

        _timer = new Timer(timerCallback, null, Timeout.Infinite, Timeout.Infinite);
    }

    public void timerCallback(object state) {
        _callback.Invoke();

        if (!_isLooping) {
            Dispose();
        }
    }

    public void Start() {
        if (_isLooping) {
            _timer.Change(_delayMilliseconds, _delayMilliseconds);
        } else {
            _timer.Change(_delayMilliseconds, Timeout.Infinite);
        }
    }

    public void Stop() {
        _timer?.Change(Timeout.Infinite, Timeout.Infinite);
    }

    public void Dispose() {
        _timer?.Dispose();
        _timer = null;
    }
}
