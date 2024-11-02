using CSCore.CoreAudioAPI;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Mixer
{
    public partial class Form1 : Form
    {

        // DLL libraries used to manage hotkeys
        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);
        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        const int INCREASE_VOLUME = 1;
        const int DECREASE_VOLUME = 2;
        const int INCREASE_VOLUME_FOCUSED = 3;
        const int DECREASE_VOLUME_FOCUSED = 4;

        public AudioSessionManager2 currentSession;
        private string FocusedProcess => GetActiveWindowTitle().Split(".exe")[0];

        public Form1()
        {
            InitializeComponent();
            notifyIcon1.Icon = new System.Drawing.Icon(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal) + @"\Icon.ico");
            notifyIcon1.Visible = true;
            notifyIcon1.Text = "Mixer";
            notifyIcon1.ContextMenuStrip = contextMenuStrip1;
            RegisterHotKey(this.Handle, DECREASE_VOLUME_FOCUSED, 0, (int)Keys.F21);
            RegisterHotKey(this.Handle, INCREASE_VOLUME_FOCUSED, 0, (int)Keys.F22);
            RegisterHotKey(this.Handle, DECREASE_VOLUME, 0, (int)Keys.F23);
            RegisterHotKey(this.Handle, INCREASE_VOLUME, 0, (int)Keys.F24);

        }

        private bool allowVisible;     // ContextMenu's Show command used
        //private bool allowClose;       // ContextMenu's Exit command used

        protected override void SetVisibleCore(bool value)
        {
            if (!allowVisible)
            {
                value = false;
                if (!this.IsHandleCreated) CreateHandle();
            }
            base.SetVisibleCore(value);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x0312)
            {
                HandleVolumeChangeMessage(ref m);
            }
            base.WndProc(ref m);
        }

        private void HandleVolumeChangeMessage(ref Message m)
        {
            var focusedProcess = GetActiveWindowTitle().Split(".exe")[0];
            switch (m.WParam.ToInt32())
            {
                case DECREASE_VOLUME:
                    ChangeVolume(-0.07f, "msedge");
                    break;
                case INCREASE_VOLUME:
                    ChangeVolume(0.07f, "msedge");
                    break;
                case DECREASE_VOLUME_FOCUSED:
                    ChangeVolume(-0.07f, focusedProcess);
                    break;
                case INCREASE_VOLUME_FOCUSED:
                    ChangeVolume(0.07f, focusedProcess);
                    break;
                default:
                    break;
            }
        }

        private void ChangeVolume(float amount, string processName)
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

        private AudioSessionManager2 GetDefaultAudioSessionManager2(DataFlow dataFlow)
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

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out int outPid);

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        private string GetActiveWindowTitle()
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



        //private string GetProcessNameFrom

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void showControlsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.SetVisibleCore(true);
            this.Show();
        }
    }
}
