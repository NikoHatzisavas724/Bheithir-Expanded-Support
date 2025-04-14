using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using System.Timers;
using Bheithir.Emulators;

class ProcessWatcher
{
    private static HashSet<string> seenProcesses = new HashSet<string>();
    private static System.Timers.Timer pollTimer;

    private static bool IsStartupEnabled(string appName, string exePath)
    {
        using (var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(
            @"Software\Microsoft\Windows\CurrentVersion\Run", false))
        {
            return key?.GetValue(appName)?.ToString() == exePath;
        }
    }

    private static void EnableStartup(string appName, string exePath)
    {
        using (var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(
            @"Software\Microsoft\Windows\CurrentVersion\Run", true))
        {
            key.SetValue(appName, exePath);
        }
    }

    private static void DisableStartup(string appName)
    {
        using (var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(
            @"Software\Microsoft\Windows\CurrentVersion\Run", true))
        {
            if (key != null)
            {
                var valueNames = key.GetValueNames();
                foreach (var valueName in valueNames)
                {
                    if (valueName.IndexOf(appName, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        key.DeleteValue(valueName, false);
                        Console.WriteLine($"Removed startup entry: {valueName}");
                    }
                }
            }
        }
    }



    private static readonly List<string> targetProcessNames = new List<string>
    {
        "citron", "ares", "bsnes", "mGBA", "dosbox-x", "DOSBox", "fcuex", "Fusion",
        "mame", "Mesen", "PPSSPPWindows64", "redream", "snes9x-x64", "snes9x",
        "visualboyadvance-m", "PPSSPPWindows"
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
        { "PPSSPPWindows", new Ppsspp32() },
        { "redream", new Redream() }
    };

    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        var assembly = Assembly.GetExecutingAssembly();
        using (Stream stream = assembly.GetManifestResourceStream("Bheithir.bheithir.ico"))
        {
            Icon trayIconIcon = new Icon(stream);
            string appName = "Bheithir";
            string exePath = Process.GetCurrentProcess().MainModule.FileName;

            bool isStartupEnabled = IsStartupEnabled(appName, exePath);

            ToolStripMenuItem startupItem = new ToolStripMenuItem("Run at startup")
            {
                Checked = isStartupEnabled,
                CheckOnClick = true
            };
            startupItem.CheckedChanged += (s, e) =>
            {
                if (startupItem.Checked)
                    EnableStartup(appName, exePath);
                else
                    DisableStartup(appName);
            };

            NotifyIcon trayIcon = new NotifyIcon()
            {
                Icon = trayIconIcon,
                Visible = true,
                Text = "Bheithir",
                ContextMenuStrip = new ContextMenuStrip()
            };

            trayIcon.ContextMenuStrip.Items.Add(startupItem);
            trayIcon.ContextMenuStrip.Items.Add(new ToolStripMenuItem("Exit", null, (s, e) =>
            {
                trayIcon.Visible = false;
                Application.Exit();
            }));

            CheckRunningProcessesOnStartup();

            Console.WriteLine("Polling for new processes...");
            StartPolling();
            Application.Run();
        }
    }

    private static void StartPolling()
    {
        pollTimer = new System.Timers.Timer(3000);
        pollTimer.Elapsed += PollProcesses;
        pollTimer.AutoReset = true;
        pollTimer.Start();
    }

    private static void PollProcesses(object sender, ElapsedEventArgs e)
    {
        var currentProcesses = Process.GetProcesses().Select(p => p.ProcessName).ToHashSet();

        foreach (var procName in currentProcesses)
        {
            if (targetProcessNames.Contains(procName) && !seenProcesses.Contains(procName))
            {
                seenProcesses.Add(procName);
                Console.WriteLine($"Matched process started: {procName}");
                HandleEmulatorStart(procName);
            }
        }

        seenProcesses.RemoveWhere(p => !currentProcesses.Contains(p));
    }

    private static void HandleEmulatorStart(string processName)
    {
        if (emulators.TryGetValue(processName, out Presence presence))
        {
            presence.Initialize();
            new System.Threading.Thread(() =>
            {
                while (true)
                {
                    if (!Process.GetProcesses().Any(p => p.ProcessName.StartsWith(presence.ProcessName)))
                    {
                        presence.Deinitialize();
                        Console.WriteLine("Thanks for using Bheithir!");
                        break;
                    }
                    else
                    {
                        presence.Update();
                        System.Threading.Thread.Sleep(1000);
                    }
                }
            })
            { IsBackground = true }.Start();
        }
    }
    private static void CheckRunningProcessesOnStartup()
    {
        var currentProcesses = Process.GetProcesses().Select(p => p.ProcessName).ToHashSet();

        foreach (var procName in currentProcesses)
        {
            if (targetProcessNames.Contains(procName) && !seenProcesses.Contains(procName))
            {
                seenProcesses.Add(procName);
                Console.WriteLine($"Matched process already running at startup: {procName}");
                HandleEmulatorStart(procName);
            }
        }
    }

}
