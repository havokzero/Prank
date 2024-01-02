using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

namespace Havoks_Virus
{
    public class DtIcon
    {
        private string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        private string[] prankNames = new string[] { "Havok Virus", "Delfos.A", "Delfos.B", /*... other names ...*/ };
        private string iconPath = @"Media\spin.ico"; // Adjust with the correct path to your icon file

        public void CreatePrankIcons()
        {
            foreach (var name in prankNames)
            {
                string filePath = Path.Combine(desktopPath, name + ".lnk"); // .lnk for shortcut files
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    writer.WriteLine("URL=HavokPrank"); // Dummy content, adjust as needed for the prank
                    // Note: Properly creating a .lnk file usually requires interacting with Windows Shell,
                    // which is beyond the scope of this simple text-writing example.
                }

                // Optionally, set the icon for the shortcut if you know how to manipulate .lnk files or use a library for it.
            }
        }

        public void Cleanup()
        {
            // Remove all prank icons from the desktop
            foreach (var name in prankNames)
            {
                string filePath = Path.Combine(desktopPath, name + ".lnk");
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }

            // Restore any original settings or cleanup additional resources if needed
        }
    }
}

