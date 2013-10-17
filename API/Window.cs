using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace TrifleJS.API
{
    public class Window
    {
        private static Dictionary<int, Timer> timers = new Dictionary<int, Timer>();

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
            Timer timer = new Timer(ms);
            timer.Elapsed += delegate
            {
                Callback.execute(callbackId, once, null);
            };
            timer.AutoReset = !once;
            timer.Enabled = true;
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
    }
}
