using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;

public class TerminalOpener
{
    [DllImport("kernel32.dll")]
    private static extern IntPtr GetConsoleWindow();

    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    // For removing title bar buttons or making other modifications
    [DllImport("user32.dll")]
    private static extern bool SetWindowText(IntPtr hWnd, string lpString);
    
    // Constants for window styles
    const int GWL_STYLE = -16;
    const int WS_DISABLED = 0x08000000;

    public static void OpenCommandPrompt()
    {
        try
        {
            // Open a new console window
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                // Add any additional arguments or configurations
            };

            Process proc = Process.Start(startInfo);

            // Modify console window after a short delay to ensure it's loaded
            Task.Delay(1000).ContinueWith(t => ModifyConsoleWindow());
        } 
         catch (Exception ex)
        {
        System.Console.WriteLine(  "Error Opening Console" + ex.Message);
        }
    }

    private static void ModifyConsoleWindow()
    {
        IntPtr consoleWindow = GetConsoleWindow();
        if (consoleWindow != IntPtr.Zero)
        {
            // Example: Set the window title
            SetWindowText(consoleWindow, "Countdown: 30");

            // Remove close/minimize/maximize buttons or disable the window
            int windowStyle = SetWindowLong(consoleWindow, GWL_STYLE, WS_DISABLED);

            // Add additional modifications as needed

            // Note: Be careful with disabling or hiding important controls. Ensure the user can still close the window or understand it's a prank.
        }
    }
}