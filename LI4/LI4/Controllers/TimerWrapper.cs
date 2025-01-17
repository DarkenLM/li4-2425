using System;
using System.Threading;

namespace LI4.Controllers;

/**
 * If necessary to edit this class more information here
 * https://learn.microsoft.com/en-us/dotnet/api/system.threading.timer?view=net-9.0
 */
public class TimerWrapper {
    private Timer _timer;
    private Action _callback;
    private int _delayMilliseconds;
    private bool _isLooping;
    public DateTime startTime { get; set;  }

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

    /**
     * for know, this function is not to be used
     * We can if necessary use an object as a parameter an use variables and methods from that object
     */
    public void timerCallback(object state) {
        _callback.Invoke();

        if (!_isLooping) {
            dispose();
        }
    }

    /**
     * This function starts the timer with the values used to create the object
     */
    public void start() {
        this.startTime = DateTime.Now;

        if (_isLooping) {
            _timer.Change(_delayMilliseconds, _delayMilliseconds);
        } else {
            _timer.Change(_delayMilliseconds, Timeout.Infinite);
        }
    }

    /**
     * This function stops the timer, it is usefull if the timer has a loop or if we want to stop mid run.
     */
    public void stop() {
        _timer?.Change(Timeout.Infinite, Timeout.Infinite);
    }

    /**
     * This function disposes the timer (Releases all resources used by the current instance of Timer.)
     */
    public void dispose() {
        _timer?.Dispose();
        _timer = null;
    }
}
