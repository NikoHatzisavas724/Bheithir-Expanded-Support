using DiscordRPC;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace Bheithir.Emulators
{
    class Mgba : Presence
    {
        public Mgba()
        {
            DiscordAppId = "1360045853552935097";
            ProcessName = "mGBA";
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
        public static string RemoveBeforeSecondPipe(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            int firstPipe = input.IndexOf('|');
            if (firstPipe == -1) return input;

            int secondPipe = input.IndexOf('|', firstPipe + 1);
            if (secondPipe == -1) return input;

            int startIndex = secondPipe + 1;

            // Skip the space directly after the second pipe if it exists
            if (startIndex < input.Length && input[startIndex] == ' ')
                startIndex++;

            return input.Substring(startIndex);
        }
        public static bool HasTwoPipes(string input)
        {
            if (string.IsNullOrEmpty(input))
                return false;

            int firstPipe = input.IndexOf('|');
            if (firstPipe == -1) return false;

            int secondPipe = input.IndexOf('|', firstPipe + 1);
            return secondPipe != -1;
        }

        public static string RemoveAfter64Bit(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            string marker = "(64-bit)";
            int index = input.IndexOf(marker);
            if (index == -1)
                return input;

            return input.Substring(0, index).TrimEnd();
        }
        public static string RemoveAfterSecondPipe(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            int firstPipe = input.IndexOf('|');
            if (firstPipe == -1) return input;

            int secondPipe = input.IndexOf('|', firstPipe + 1);
            if (secondPipe == -1) return input;

            // Move back to remove the space before the second pipe, if it exists
            int endIndex = secondPipe;
            if (endIndex > 0 && input[endIndex - 1] == ' ')
                endIndex--;

            return input.Substring(0, endIndex).TrimEnd();
        }
        public static string RemoveParenthesesAndBrackets(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            // Pattern to match anything in () or []
            string pattern = @"\s*[\(\[].*?[\)\]]";
            return Regex.Replace(input, pattern, "").Trim();
        }
        public static string RemoveBeforeDash(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            int dashIndex = input.IndexOf('-');
            if (dashIndex == -1)
                return input;

            int startIndex = dashIndex + 1;

            // Skip the space after the dash, if it exists
            if (startIndex < input.Length && input[startIndex] == ' ')
                startIndex++;

            return input.Substring(startIndex);
        }

        public override void SetNewPresence()
        {
            string[] titleParts = WindowPattern.Split(WindowTitle);
            string details;
            try
            {   
                if (titleParts.Length == 1)
                    details = "No game loaded";
                else if (titleParts[0] == "mGBA"){
                    details = "No game loaded";
                }
                else
                    details = RemoveBeforeDash(RemoveParenthesesAndBrackets(titleParts[0]));
            }
            catch (Exception) { return; }

            string status;
            try
            {
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
                        LargeImageKey = "mgbalogo",
                        LargeImageText = "mGBA"
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
