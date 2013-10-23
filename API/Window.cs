using System;
using System.Collections.Generic;
using System.Text;

namespace TrifleJS.API
{
    public class Window
    {
        /// <summary>
        /// List of currently executing timers for window.setTimeout() and window.setInterval()
        /// </summary>
        public static Dictionary<int, Timer> timers = new Dictionary<int, Timer>();

        /// <summary>
        /// Defers execution of a callback by number of milliseconds, same as window.setTimeout()
        /// </summary>
        /// <param name="callbackId">ID of the callback in JavaScript context</param>
        /// <param name="ms">Millseconds to wait for</param>
        /// <returns></returns>
        public static int SetTimeout(string callbackId, int ms) {
            return SetTimer(callbackId, ms, true);
        }

        /// <summary>
        /// Executes a callback at repeated intervals, same as window.setInterval()
        /// </summary>
        /// <param name="callbackId">ID of the callback in JavaScript context</param>
        /// <param name="ms">>Millseconds between intervals</param>
        /// <returns></returns>
        public static int SetInterval(string callbackId, int ms) {
            return SetTimer(callbackId, ms, false);
        }

        /// <summary>
        /// Clears a timeout specified with SetTimeout
        /// </summary>
        /// <param name="timeoutId">ID of the timeout call</param>
        public static void ClearTimeout(int timeoutId) {
            ClearTimer(timeoutId);
        }

        /// <summary>
        /// Clears execution specified with SetInterval
        /// </summary>
        /// <param name="intervalId"></param>
        public static void ClearInterval(int intervalId) {
            ClearTimer(intervalId);
        }

        private static int SetTimer(string callbackId, int ms, bool once) {
            Timer timer = new Timer();
            timer.Callback += delegate
            {
                if (once) { timer.Enabled = false; }
                Callback.execute(callbackId, once, null);
            };
            timer.Enabled = true;
            timer.Interval = ms;
            timer.Start();
            int id = Environment.TickCount;
            Window.timers.Add(id, timer);
            return id;
        }

        private static void ClearTimer(int timerId) {
            Timer timer = Window.timers[timerId];
            if (timer != null) {
                timer.Dispose();
                Window.timers.Remove(timerId);
            }
        }

    /// <summary>
    /// Internal timer for window.setTimeout() and window.setInterval().
    /// This is to ensure that async calls always run on the same thread.
    /// </summary>
        public class Timer : IDisposable
        {

            public void Tick()
            {
                if (Enabled && Environment.TickCount >= nextTick)
                {
                    Callback.Invoke(this, null);
                    nextTick = Environment.TickCount + Interval;
                }
            }

            private int nextTick = 0;

            public void Start()
            {
                this.Enabled = true;
                Interval = interval;
            }

            public void Stop()
            {
                this.Enabled = false;
            }

            public event EventHandler Callback;

            public bool Enabled = false;

            private int interval = 1000;

            public int Interval
            {
                get { return interval; }
                set { interval = value; nextTick = Environment.TickCount + interval; }
            }

            public void Dispose()
            {
                this.Callback = null;
                this.Stop();
            }

        }

        /// <summary>
        /// Checks through all available timers 
        /// to see if any of them should be executed.
        /// </summary>
        public static void CheckTimers()
        {
            foreach (Timer timer in new List<Timer>(timers.Values))
            {
                timer.Tick();
            }
        }
    }
}
