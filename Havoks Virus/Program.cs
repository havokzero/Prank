using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography.Xml;
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
            AllocConsole();  // Allocate a console for this app

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            OpenJobSearchTabsOnce(); // Ensure this is placed correctly before running the form
                                     // Create and show the first PrankForm as soon as the application starts.
            PrankForm firstForm = new PrankForm();
            firstForm.Show();

            // Continue running the application with the first form
            Application.Run();

            // Set the animated cursor globally
            try
            {
                string cursorPath = "Media/rspin.ani"; // Adjust the path to your .ani file
                IntPtr cursorHandle = LoadCursorFromFile(cursorPath);
                if (!IntPtr.Zero.Equals(cursorHandle))
                {
                    Cursor animatedCursor = new Cursor(cursorHandle);
                    Cursor.Current = animatedCursor;
                }
                else
                {
                    throw new ApplicationException("Could not create cursor from file " + cursorPath);
                }
            }
            catch (Exception err)
            {
                // Use Console.WriteLine to write error messages to the console
                System.Console.WriteLine(err.Message);
                MessageBox.Show(err.Message);  // Or use MessageBox to show the error in a window
            }

            // Running the PrankForm
            Application.Run(new PrankForm());
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
