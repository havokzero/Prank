using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Havoks_Virus
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 
        [DllImport("kernel32.dll")]
        static extern bool AllocConsole();
        

        [STAThread]
        static void Main()
        {
            AllocConsole(); // This will open a console window
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new PrankForm()); // Ensure PrankForm is being instantiated
        }

        
    }
}
