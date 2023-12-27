using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

public class TerminalOpener
{
    public static void OpenCommandPrompt()
    {
        // Set up process start information
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = "cmd.exe", // Command Prompt
            // For PowerShell, you might use "powershell.exe"
            // Add any additional arguments if needed
        };

        // Start the process
        Process proc = Process.Start(startInfo);

        // Optionally, you might want to hold the reference to the process
        // to interact with it or close it later
    }
}
namespace Havoks_Virus
{
    internal class Console
    {
    }
}
