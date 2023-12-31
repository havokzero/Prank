using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Havoks_Virus
{
    static class Program
    {
        [DllImport("User32.dll")]
        private static extern IntPtr LoadCursorFromFile(string fileName);

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

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
                MessageBox.Show(err.Message);
            }

            Application.Run(new PrankForm());
        }
    }
}
