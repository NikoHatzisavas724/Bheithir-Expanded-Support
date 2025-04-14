using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using Bheithir.Emulators;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
class ProcessWatcher
{
    private static NotifyIcon trayIcon;
    private static readonly List<string> targetProcessNames = new List<string>
    {
        "citron",
        "ares",
        "bsnes",
        "mGBA",
        "dosbox-x",
        "DOSBox",
        "fcuex",
        "Fusion",
        "mame",
        "Mesen",
        "PPSSPPWindows64",
        "redream",
        "snes9x-x64",
        "snes9x",
        "visualboyadvance-m",
        "PPSSPPWindows",
    };
    private static readonly Dictionary<string, Presence> emulators = new Dictionary<string, Presence>()
    {
        { "DOSBox", new DosBox() },
        { "dosbox-x", new DosBox_X() },
        { "fceux", new Fceux() },
        { "snes9x-x64", new Snes9x() },
        { "snes9x", new Snes9x32() },
        { "fusion", new Fusion() },
        { "visualboyadvance-m", new Vbam() },
        { "mame", new Mame() },
        { "mGBA", new Mgba() },
        { "Mesen", new Mesen() },
        { "ares", new Ares() },
        { "citron", new Citron() },
        { "bsnes", new Bsnes() },
        { "PPSSPPWindows64", new Ppsspp() },
        { "PPSSPPWindows", new Ppsspp32()},
        { "redream", new Redream() }
    };
    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        trayIcon = new NotifyIcon()
        {
            Icon = SystemIcons.Application,
            Visible = true,
            Text = "Bheithir",
            ContextMenuStrip = new ContextMenuStrip()
            {
                Items = {
                    new ToolStripMenuItem("Exit", null, (s, e) => {
                        trayIcon.Visible = false;
                        Application.Exit();
                    })
                }
            }
        };



        System.Threading.Thread.Sleep(5000);
        Console.WriteLine("Watching for new processes...");

        var startWatch = new ManagementEventWatcher(
            new WqlEventQuery("SELECT * FROM Win32_ProcessStartTrace"));

        startWatch.EventArrived += new EventArrivedEventHandler(ProcessStarted);
        startWatch.Start();

        Application.Run();

        startWatch.Stop();
    }

    private static void ProcessStarted(object sender, EventArrivedEventArgs e)
    {
        string processName = (string)e.NewEvent.Properties["ProcessName"].Value;

        string cleanName = processName.Replace(".exe", "");
        processName = cleanName;
        if (targetProcessNames.Contains(processName))
        {
            Console.WriteLine($"Matched process started: {processName}");
            string emulator = cleanName;
            Presence presence = emulators[emulator];
            Console.WriteLine(presence.ProcessName);
            presence.Initialize();
            while (true)
            {
                // Console.WriteLine("");
                // Console.WriteLine(Process.GetProcesses().Any(x => x.ProcessName.StartsWith(presence.ProcessName)));
                if (!Process.GetProcesses().Any(x => x.ProcessName.StartsWith(presence.ProcessName)))
                {
                    presence.Deinitialize();
                    Console.WriteLine("Thanks for using Bheithir!");

                    string exePath = Process.GetCurrentProcess().MainModule.FileName;

                    Process.Start(exePath);

                    Environment.Exit(0);
                    return;
                }
                else
                {
                    presence.Update();
                }
            }
        }
    }
}
