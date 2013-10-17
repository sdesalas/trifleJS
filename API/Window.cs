using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace TrifleJS.API
{
    public class Window
    {
        private static Dictionary<int, Timer> timers = new Dictionary<int, Timer>();

        public static int SetTimeout(string callbackId, int ms) {
            return SetTimer(callbackId, ms, true);
        }

        public static int SetInterval(string callbackId, int ms) {
            return SetTimer(callbackId, ms, false);
        }

        public static void ClearTimeout(int timeoutId) {
            ClearTimer(timeoutId);
        }

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
