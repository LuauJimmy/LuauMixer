using CSCore.CoreAudioAPI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Mixer
{
    public static class WindowsUtils
    {
        // DLL libraries used to manage hotkeys
        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);
        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        internal static AudioSessionManager2 GetDefaultAudioSessionManager2(DataFlow dataFlow)
        {
            using (var enumerator = new MMDeviceEnumerator())
            {
                using (var device = enumerator.GetDefaultAudioEndpoint(dataFlow, Role.Multimedia))
                {
                    return Task.Run(() =>
                    {
                        Debug.WriteLine("DefaultDevice: " + device.FriendlyName);
                        var sessionManager = AudioSessionManager2.FromMMDevice(device);
                        return sessionManager;
                    }).Result;
                }
            }
        }

        internal static void ChangeVolume(float amount, string processName)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var Session = GetDefaultAudioSessionManager2(DataFlow.Render);
            using (var sessionManager = Session)
            {

                using (var sessionEnumerator = sessionManager.GetSessionEnumerator())
                {
                    Console.WriteLine(sessionEnumerator.Count);
                    foreach (var session in sessionEnumerator)
                    {
                        using (var simpleVolume = session.QueryInterface<SimpleAudioVolume>())
                        using (var sessionControl = session.QueryInterface<AudioSessionControl2>())
                        {
                            if (sessionControl.Process.ProcessName.Equals(processName))
                            {
                                float newVol = simpleVolume.MasterVolume + amount;
                                if (newVol <= 0 || newVol >= 1) return;
                                simpleVolume.MasterVolume += amount;
                            }
                        }
                    }
                }
            }
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
        }

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out int outPid);

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        internal static string GetActiveWindowTitle()
        {

            const int nChars = 256;
            StringBuilder Buff = new StringBuilder(nChars);
            IntPtr handle = GetForegroundWindow();
            GetWindowThreadProcessId(handle, out int pid);
            try
            {
                var name = Process.GetProcessById(pid).MainModule.ModuleName;
                return name;
            }
            catch (Exception ex)
            {
                throw new Exception("Process ID was null.");
            }

        }
    }
}
