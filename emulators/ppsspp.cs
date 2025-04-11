using DiscordRPC;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace Bheithir.Emulators
{
    class Ppsspp : Presence
    {
        private string oldtitle;
        public Ppsspp()
        {
            DiscordAppId = "1360163797918023771";
            ProcessName = "PPSSPPWindows64";
            WindowPattern = new Regex("(\\s-\\s)(?!.*(\\s-\\s))", RegexOptions.Compiled);
        }

        public override void Initialize()
        {
            Client = new DiscordRpcClient(DiscordAppId);

            Process = Process.GetProcesses().Where(x => x.ProcessName.StartsWith(ProcessName)).ToList()[0];
            WindowTitle = Process.MainWindowTitle;
            oldtitle = WindowTitle;
            Client.OnReady += (sender, e) => { };
            Client.OnPresenceUpdate += (sender, e) => { };

            try
            {
                Client.Initialize();
                Console.WriteLine("Successfully connected to client!");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Connection to client was not successful!\nERROR: {e.Message}");
                return;
            }

            try { SetNewPresence(); }
            catch (Exception e)
            {
                Console.WriteLine($"Setting presence was not successful!\nERROR: {e.Message}");
                return;
            }
        }
        public override void Update()
        {
            Client.OnPresenceUpdate += (sender, e) => { };
            Client.Invoke();
            OnUpdate();
        }
        public override void Deinitialize()
        {
            Client.ClearPresence();
            Client.Dispose();
        }

        public override void OnUpdate()
        {
            Process process;
            try
            {
                process = Process.GetProcesses().Where(x => x.ProcessName.StartsWith(ProcessName)).ToList()[0];
            }
            catch (Exception) { return; }

            if (process.MainWindowTitle != WindowTitle)
            {
                Process = process;
                WindowTitle = Process.MainWindowTitle;
                if (WindowTitle == "")
                {
                    WindowTitle = WindowTitleHelper.GetWindowTitleFallback("ares");
                }

                if (!(WindowTitle == oldtitle))
                {
                    oldtitle = WindowTitle;
                    SetNewPresence();
                }
            }
        }

        public override void SetNewPresence()
        {
            string[] titleParts = WindowPattern.Split(WindowTitle);
            string details;
            try
            {
                if (titleParts.Length > 2)
                {
                    details = ParsingUtils.RemoveBeforeColon(ParsingUtils.RemoveBeforeDash(ParsingUtils.RemoveParenthesesAndBrackets(titleParts[2])));
                }
                else
                    details = "No game loaded";

            }
            catch (Exception)
            { return; }

            string status;
            try
            {
                // status = RemoveBeforeDash(RemoveParenthesesAndBrackets(WindowTitle));
                status = "";
            }
            catch (Exception) { return; }

            try
            {
                Client.SetPresence(new RichPresence
                {
                    Details = details,
                    State = status,
                    Timestamps = new Timestamps(DateTime.UtcNow),
                    Assets = new Assets()
                    {
                        LargeImageKey = "ppsspplogo",
                        LargeImageText = "PPSSPP"
                    }
                });
                Console.WriteLine("Presence successfully set!");
            }
            catch (Exception)
            {
                Console.WriteLine("Presence was not set successfully!");
                return;
            }
        }
    }
}
