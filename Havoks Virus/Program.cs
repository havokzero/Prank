using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Havoks_Virus
{
    static class Program
    {
        private static bool tabsOpened = false; // Track if job search tabs have been opened

        // DLL Imports and constants
        //  [DllImport("kernel32.dll", SetLastError = true)]
        //private static extern bool AllocConsole();

        //  [DllImport("kernel32.dll")]
        //private static extern IntPtr GetConsoleWindow();

        // [DllImport("user32.dll")]
        // public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        //  [DllImport("user32.dll")]
        // public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        // [DllImport("user32.dll")]
        // private static extern bool DeleteMenu(IntPtr hMenu, uint uPosition, uint uFlags);

        // [DllImport("User32.dll")]
        // private static extern IntPtr LoadCursorFromFile(string fileName);

        // [DllImport("user32.dll")]
        // private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("User32.dll")]
        private static extern IntPtr LoadCursorFromFile(string fileName);


        private const int GWL_EXSTYLE = -20; // nIndex for Set/GetWindowLong for extended window style
        private const int WS_EX_TOOLWINDOW = 0x00000080; // Extended window style for hiding from taskbar and Alt+Tab
        private const uint SC_CLOSE = 0xF060; // Command to close the window
        private const uint SC_MINIMIZE = 0xF020; // Command to minimize the window
        private const uint SC_MAXIMIZE = 0xF030; // Command to maximize the window
        private const uint MF_BYCOMMAND = 0x00000000; // Flag for DeleteMenu

        [STAThread]
        static void Main()
        {
            //AllocConsole(); // Allocate a console for debugging

            // Initialization for Windows Forms
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);

            // Create and initialize the HiddenConsole form
            //HiddenConsole hiddenConsole = new HiddenConsole();
            //hiddenConsole.Show();

            // Start the HiddenConsole form and the main application
           // Application.Run(hiddenConsole); // This will start the hidden console     this will not allow other forms to show its one or the other

           // IntPtr consoleWindow = GetConsoleWindow();

            // Hide the console window from Alt+Tab and the taskbar
          //  int extendedStyle = GetWindowLong(consoleWindow, GWL_EXSTYLE);
         //   SetWindowLong(consoleWindow, GWL_EXSTYLE, extendedStyle | WS_EX_TOOLWINDOW);

            // Remove close, minimize, maximize buttons
          //  IntPtr hMenu = GetSystemMenu(consoleWindow, false);
           // DeleteMenu(hMenu, SC_CLOSE, MF_BYCOMMAND);
            //DeleteMenu(hMenu, SC_MINIMIZE, MF_BYCOMMAND);
            //DeleteMenu(hMenu, SC_MAXIMIZE, MF_BYCOMMAND);

            // Implement countdown in the console title bar
            //StartCountdown(30);

            // Standard WinForms initialization
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            OpenJobSearchTabsOnce(); // Ensures tabs are opened only once at the start

            // Load and apply the animated cursor
            string cursorPath = "Media/rspin.ani"; // Adjust the path to your .ani file
            IntPtr cursorHandle = LoadCursorFromFile(cursorPath);
            if (!IntPtr.Zero.Equals(cursorHandle))
            {
                Application.UseWaitCursor = false;
                Cursor.Current = new Cursor(cursorHandle);
                Application.ApplicationExit += (sender, args) => Cursor.Current.Dispose();
            }
            else
            {
                MessageBox.Show("Failed to load cursor from file: " + cursorPath);
            }

            //Thread countdownThread = new Thread(() => StartCountdown(30));
            //countdownThread.IsBackground = true; // Mark it as a background thread
            //countdownThread.Start();

            // Initialize and play audio only once here, rather than in each PrankForm
            Audio sharedAudio = new Audio();
            sharedAudio.Start();

            HiddenConsole hiddenConsole = new HiddenConsole();
            hiddenConsole.Show();  // Make sure this is called

            // Run the application with the PrankForm as the main form
            Application.Run(new PrankForm());

            // Initialize and set the mouse cursor and start jitter effect
            Mouse prankMouse = new Mouse("Media/rspin.ani");
            prankMouse.StartMouseMovement();

            
        }

        // Logic to open job search tabs only once, delayed by 7 seconds
        public static void OpenJobSearchTabsOnce()
        {
            Task.Delay(6575).ContinueWith(t =>
            {
                if (!tabsOpened)
                {
                    JobSearch.OpenJobSearchTabs();
                    tabsOpened = true; // Ensure it only runs once
                }
            });
        }

        // Countdown logic, updating console title each second
       /* static void StartCountdown(int seconds)
        {
            for (int i = seconds; i >= 0; i--)
            {
                // Update the console title with the countdown
                Console.Title = $"{i} seconds... Until File Encryption";
                // Wait a second before continuing the countdown
                Thread.Sleep(1000); // Note: Thread.Sleep is fine here since we're in a non-UI thread.
            }
        }*/
    }
}
