﻿using DiscordRPC;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Bheithir.Emulators;

namespace Bheithir.Emulators
{
    class Snes9x32 : Presence
    {
        public Snes9x32()
        {
            DiscordAppId = "1342995297550205089";
            ProcessName = "snes9x";
            WindowPattern = new Regex("(\\s-\\s)(?!.*(\\s-\\s))", RegexOptions.Compiled);
        }

        public override void Initialize()
        {
            Client = new DiscordRpcClient(DiscordAppId);

            Process = Process.GetProcesses().Where(x => x.ProcessName.StartsWith(ProcessName)).ToList()[0];
            WindowTitle = Process.MainWindowTitle;

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
                SetNewPresence();
            }
        }

        public override void SetNewPresence()
        {
            string[] titleParts;
            try
            {
                titleParts = WindowPattern.Split(WindowTitle);
            }
            catch (Exception)
            {
                return;
            }
            string details;
            try
            {
                if (titleParts.Length == 1)
                    details = "No game loaded";
                else
                    details = ParsingUtils.ParseTitle(ParsingUtils.RemoveParenthesesAndBrackets(titleParts[0]));
            }
            catch (Exception) { return; }

            string status;
            try
            {
                if (titleParts.Length == 1)
                    status = titleParts[0].Replace("Snes9x ", "v");
                else
                    status = titleParts[2].Replace("Snes9x ", "v");
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
                        LargeImageKey = "snes",
                        LargeImageText = "Snes9x"
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
