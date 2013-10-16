using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace TrifleJS.Interop
{
    public class Window
    {
        private static Dictionary<int, Timer> timers = new Dictionary<int, Timer>();

        public static int setTimeout(string callbackId, int ms) {
            return setTimer(callbackId, ms, true);
        }

        public static int setInterval(string callbackId, int ms) {
            return setTimer(callbackId, ms, false);
        }

        public static void clearTimeout(int timeoutId) {
            clearTimer(timeoutId);
        }

        public static void clearInterval(int intervalId) {
            clearTimer(intervalId);
        }

        private static int setTimer(string callbackId, int ms, bool once) {
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

        private static void clearTimer(int timerId) {
            Timer timer = Window.timers[timerId];
            if (timer != null) {
                timer.Dispose();
                Window.timers.Remove(timerId);
            }
        }
    }
}
