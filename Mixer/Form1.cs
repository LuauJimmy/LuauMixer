using CSCore.CoreAudioAPI;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Windows;

namespace Mixer
{
    public partial class Form1 : Form
    {



        const int INCREASE_VOLUME = 1;
        const int DECREASE_VOLUME = 2;
        const int INCREASE_VOLUME_FOCUSED = 3;
        const int DECREASE_VOLUME_FOCUSED = 4;

        public AudioSessionManager2 currentSession;
        private string FocusedProcess => WindowsUtils.GetActiveWindowTitle().Split(".exe")[0];

        public Form1()
        {
            InitializeComponent();
            notifyIcon1.Icon = new System.Drawing.Icon(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal) + @"\Icon.ico");
            notifyIcon1.Visible = true;
            notifyIcon1.Text = "Mixer";
            notifyIcon1.ContextMenuStrip = contextMenuStrip1;
            WindowsUtils.RegisterHotKey(this.Handle, DECREASE_VOLUME_FOCUSED, 0, (int)Keys.F21);
            WindowsUtils.RegisterHotKey(this.Handle, INCREASE_VOLUME_FOCUSED, 0, (int)Keys.F22);
            WindowsUtils.RegisterHotKey(this.Handle, DECREASE_VOLUME, 0, (int)Keys.F23);
            WindowsUtils.RegisterHotKey(this.Handle, INCREASE_VOLUME, 0, (int)Keys.F24);

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



        private void HandleVolumeChangeMessage(ref Message m)
        {
            switch (m.WParam.ToInt32())
            {
                case DECREASE_VOLUME:
                    WindowsUtils.ChangeVolume(-0.07f, "msedge");
                    break;
                case INCREASE_VOLUME:
                    WindowsUtils.ChangeVolume(0.07f, "msedge");
                    break;
                case DECREASE_VOLUME_FOCUSED:
                    WindowsUtils.ChangeVolume(-0.07f, FocusedProcess);
                    break;
                case INCREASE_VOLUME_FOCUSED:
                    WindowsUtils.ChangeVolume(0.07f, FocusedProcess);
                    break;
                default:
                    break;
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x0312)
            {
                HandleVolumeChangeMessage(ref m);
            }
            base.WndProc(ref m);
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
