using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

public static class WindowTitleHelper
{
    public static string GetWindowTitleFallback(string processName)
    {
        var processes = Process.GetProcessesByName(processName);
        if (processes.Length == 0)
            return null;

        foreach (var proc in processes)
        {
            IntPtr mainHandle = proc.MainWindowHandle;
            string mainTitle = GetWindowTitle(mainHandle);

            if (!string.IsNullOrWhiteSpace(mainTitle))
                return mainTitle;

            // If main window has no title, find another window for this process
            string fallbackTitle = null;

            EnumWindows((hWnd, lParam) =>
            {
                uint pid;
                GetWindowThreadProcessId(hWnd, out pid);
                if (pid == proc.Id && hWnd != mainHandle && IsWindowVisible(hWnd))
                {
                    string title = GetWindowTitle(hWnd);
                    if (!string.IsNullOrWhiteSpace(title))
                    {
                        fallbackTitle = title;
                        return false; // Stop enum
                    }
                }
                return true;
            }, IntPtr.Zero);

            return fallbackTitle;
        }

        return null;
    }

    private static string GetWindowTitle(IntPtr hWnd)
    {
        int length = GetWindowTextLength(hWnd);
        if (length == 0) return null;

        StringBuilder builder = new StringBuilder(length + 1);
        GetWindowText(hWnd, builder, builder.Capacity);
        return builder.ToString();
    }

    // P/Invoke declarations
    [DllImport("user32.dll")]
    private static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

    private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

    [DllImport("user32.dll")]
    private static extern bool IsWindowVisible(IntPtr hWnd);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern int GetWindowTextLength(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
}
