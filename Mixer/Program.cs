using System.Data;
using System.Diagnostics;
using System.Runtime.InteropServices;
using CSCore.CoreAudioAPI;
using System.Diagnostics;
using System.Runtime.InteropServices;


namespace Mixer
{

    internal static class Program
    {

        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            var icon = new NotifyIcon();
            icon.Visible = true;
            Application.Run(new Form1());

        }

    }
}