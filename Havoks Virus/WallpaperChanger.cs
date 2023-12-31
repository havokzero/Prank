using System;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Drawing;
using System.Diagnostics;
using System.IO;


namespace Havoks_Virus
{
    public static class WallpaperChanger
    {
        public enum Style
        {
            Stretched,
            Centered,
            Tiled // Added tiled option for completion
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        private const int SPI_SETDESKWALLPAPER = 20;
        private const int SPIF_UPDATEINIFILE = 0x01;
        private const int SPIF_SENDCHANGE = 0x02;

        public static void SetWallpaper(string imagePath, Style style)
        {
            string styleValue = "2"; // Default to stretched
            string tileValue = "0";  // Generally "0"

            // Determine the correct settings for the style
            switch (style)
            {
                case Style.Stretched:
                    styleValue = "2"; // Stretched
                    break;
                case Style.Centered:
                    styleValue = "1"; // Centered
                    break;
                case Style.Tiled:
                    styleValue = "1"; // Tiled
                    tileValue = "1";
                    break;
            }

            // Set the registry values for control panel to change wallpaper style
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true))
            {
                if (key != null)
                {
                    key.SetValue(@"WallpaperStyle", styleValue); // 0, 1, 2...
                    key.SetValue(@"TileWallpaper", tileValue); // 0 or 1
                }

                // Set the wallpaper using the system parameters info function
                int result = SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, imagePath, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);

                if (result == 0) // If setting the wallpaper failed
                {
                    int error = Marshal.GetLastWin32Error();
                    Debug.WriteLine("Failed to set wallpaper. Error: " + error);
                }
                else
                {
                    Debug.WriteLine("Wallpaper set successfully.");
                }
            }
        }
    }
}
