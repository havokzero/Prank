using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;


namespace Havoks_Virus
{
    public class PrankForm : Form
    {
        private System.ComponentModel.IContainer components = null;
        private PictureBox pictureBox = new PictureBox();
        private List<Image> gifFrames = new List<Image>();
        private int currentFrame = 0;
        private Timer moveTimer = new Timer();
        private static int formCount = 0;
        private const int maxForms = 14;
        private List<Image> loadedImages = new List<Image>();
        private static List<PrankForm> openForms = new List<PrankForm>();
        private Timer wallpaperTimer = new Timer();
        private string originalWallpaperPath;
        private Random random = new Random();
        private static Audio sharedAudio = new Audio();
        private Audio audio;
        // diagnostic tools 
        private Timer diagnosticTimer;
        private int formCounter = 0;
        private Timer createFormTimer = new Timer();
        private static bool jobSearchTabsOpened = false;
        private Mouse myMouseHandler;



        public PrankForm()
        {
            InitializeComponent();
            SetupMovementTimer();
            originalWallpaperPath = GetOriginalWallpaperPath();
            ExtractFramesFromGif("Media/blade.gif");
            LoadContentAsync();
            SetupTimer();
            SetupWallpaperChangeTimer();

            //sharedAudio = new Audio(); // Initialize sharedAudio here

            ManageFormInstances(); // Handle the creation and organization of multiple forms
            SetInitialWallpaper();

            createFormTimer.Interval = 5000; // 5 seconds delay                         //  Testing 
            createFormTimer.Tick += (sender, e) => CreateFormWithRandomDelay();        //   Random
            createFormTimer.Start();                                                  //    Delay

            if (formCount == 0)  // Only start spawning additional forms for the first instance
            {
                SpawnAdditionalForm();  // Begin asynchronous form creation
            }

            if (!jobSearchTabsOpened)
            {
                // Open job search tabs using the JobSearch class
                OpenJobSearchTabs();
                jobSearchTabsOpened = true; // Set the flag to true to indicate tabs have been opened
            }

            //Diagnotic initialization 
          //  diagnosticTimer = new Timer();
          //  diagnosticTimer.Interval = 5000; // Check every 5 seconds, adjust as needed
          //  diagnosticTimer.Tick += (sender, e) => CheckFormCount();
          //  diagnosticTimer.Start();

            myMouseHandler = new Mouse("Media/rspin.ani"); // Initialize the Mouse object with the cursor file path

            // Start a timer to periodically reapply the custom cursor
            Timer cursorTimer = new Timer();
            cursorTimer.Interval = 1000; // Set the interval (in milliseconds) as needed
            cursorTimer.Tick += (sender, e) => myMouseHandler.ApplyCursor();
            cursorTimer.Start();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (myMouseHandler != null)
            {
                myMouseHandler.StopMouseMovement();
            }
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            // Set up PictureBox to display images or GIFs
            pictureBox.Dock = DockStyle.Fill;
            pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            Controls.Add(pictureBox);

            // Configure the form's properties
            FormBorderStyle = FormBorderStyle.None;
            TopMost = true;
            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(200, 200);
            BackColor = Color.Black;
            TransparencyKey = Color.Black;
            ShowInTaskbar = false;

            // Initialize the Mouse handler with the path to your animated cursor
          //  string cursorPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Media", "rspin.ani");
         //   myMouseHandler = new Mouse(cursorPath);
          //  myMouseHandler.ApplyCursor();
          //  myMouseHandler.StartMouseMovement();
        }

        private void OpenJobSearchTabs()
        {
            // Open job search tabs using the JobSearch class
            JobSearch.OpenJobSearchTabs();
        }

        private void SetupMovementTimer()
        {
            moveTimer = new Timer();
            moveTimer.Interval = 2500; // Interval in milliseconds (adjust as needed)
            moveTimer.Tick += (sender, e) => MoveFormRandomly();
            moveTimer.Start();
        }

        private void MoveFormRandomly()
        {
            // Randomly reposition the form on the desktop
            int screenWidth = Screen.PrimaryScreen.Bounds.Width;
            int screenHeight = Screen.PrimaryScreen.Bounds.Height;
            int newX = random.Next(screenWidth - Width);
            int newY = random.Next(screenHeight - Height);
            SetDesktopLocation(newX, newY);

            // Randomize the next movement interval
            moveTimer.Interval = random.Next(1500, 5500); // Random time between 1.5 and 3.5 seconds
        }

        private async void SetupTimer()
        {
            // A general purpose timer for actions like updating the GIF frame
            moveTimer.Interval = 500; // Half a second
            moveTimer.Tick += (sender, e) => UpdateGifFrame();
            moveTimer.Start();
        }

        private void UpdateGifFrame()
        {
            // Update the current frame of the GIF
            if (gifFrames.Count > 0)
            {
                pictureBox.Image = gifFrames[currentFrame];
                currentFrame = (currentFrame + 1) % gifFrames.Count;
            }
        }

        private async void LoadContentAsync()
        {
            // Load additional heavy content like images asynchronously
            await Task.Run(() => LoadHeavyContent());
        }

        private void LoadHeavyContent()
        {
            // Load additional images or other resources
            string[] imageFiles = Directory.GetFiles("Media", "*.jpg");
            foreach (var imagePath in imageFiles)
            {
                Image img = Image.FromFile(imagePath);
                loadedImages.Add(img);

                // Dispose of the image to release its resources
                img.Dispose();
            }
        }

        private void SetupWallpaperChangeTimer()
        {
            // Configure and start timer for changing wallpaper periodically
            wallpaperTimer.Interval = 30000; // Change every 30 seconds
            wallpaperTimer.Tick += (sender, e) => ChangeWallpaper();
            wallpaperTimer.Start();
        }

        private void ChangeWallpaper()
        {
            // Change the desktop wallpaper
            string wallpaperPath = Path.Combine(Application.StartupPath, "Media\\wallpaper.jpg");
            WallpaperChanger.SetWallpaper(wallpaperPath, WallpaperChanger.Style.Stretched);
        }

        private string GetOriginalWallpaperPath()
        {
            // Retrieve and return the current wallpaper path
            return Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop")?.GetValue("WallPaper").ToString() ?? string.Empty;
        }

        private void ExtractFramesFromGif(string path)
        {
            // Extract each frame from the GIF to display animatedly
            Image gifImg = Image.FromFile(path);
            FrameDimension dimension = new FrameDimension(gifImg.FrameDimensionsList[0]);
            int frameCount = gifImg.GetFrameCount(dimension);
            for (int i = 0; i < frameCount; i++)
            {
                gifImg.SelectActiveFrame(dimension, i);
                gifFrames.Add((Image)gifImg.Clone());
            }
        }

        private void SetInitialWallpaper()
        {
            // Set the initial wallpaper when the prank starts
            string wallpaperPath = Path.Combine(Application.StartupPath, "Media\\wallpaper.jpg");
            WallpaperChanger.SetWallpaper(wallpaperPath, WallpaperChanger.Style.Stretched);
        }



        //testing another method 
        private async void ManageFormInstances()
        {
            openForms.Add(this);
            formCount++;

            System.Console.WriteLine($"Form Added. Current form count: {formCount}"); // Replace with actual logging if available

            if (formCount < maxForms) // Check if we should continue spawning forms
            {
                // Generate a random delay between 2 and 5 seconds
                Random random = new Random();
                int delaySeconds = random.Next(2, 6); // Generates a random number between 2 and 5 (inclusive)
                int delayMilliseconds = delaySeconds * 1000; // Convert seconds to milliseconds

                // Introduce the random delay before creating the next form
                await Task.Delay(delayMilliseconds);

                // Create the next form after the delay
                CreateFormOnRandomScreen(); // Create the next form
            }
            else if (formCount == maxForms)
            {
                System.Console.WriteLine("Reached maximum form limit."); // Logging for reaching max limit
            }
        }

        //testing this random delay 
        private async void CreateFormWithRandomDelay()
        {
            if (formCounter < maxForms)
            {
                // Generate a random delay between 2 and 5 seconds
                int delaySeconds = random.Next(2, 6); // Generates a random number between 2 and 5 (inclusive)
                int delayMilliseconds = delaySeconds * 1000; // Convert seconds to milliseconds

                // Introduce the random delay before creating the next form
                await Task.Delay(delayMilliseconds);

                // Create the next form
                CreateFormOnRandomScreen();

                formCounter++;

                // Check if the maximum number of forms is reached
                if (formCounter >= maxForms)
                {
                    createFormTimer.Stop();
                    System.Console.WriteLine("Reached maximum form limit.");
                }
            }
        }

        private void CreateFormOnRandomScreen()
        {
            if (formCount >= maxForms) return;  // Early exit if the maximum number of forms is reached

            // Get all screens and pick a random one
            Screen[] screens = Screen.AllScreens;
            Screen targetScreen = screens[random.Next(screens.Length)]; // Pick a random screen

            // Create a new form and set its starting position within the bounds of the target screen
            PrankForm newForm = new PrankForm();
            newForm.StartPosition = FormStartPosition.Manual;
            Rectangle area = targetScreen.WorkingArea;
            newForm.Location = new Point(random.Next(area.Left, area.Right - newForm.Width),
                                         random.Next(area.Top, area.Bottom - newForm.Height));
            newForm.Show();

            // Thread-safe increment of the shared form counter
            Interlocked.Increment(ref formCount);
        }

        private async void SpawnAdditionalForm()
        {
            int delayBetweenForms = 5000; // Initial delay (5 seconds) before the first form

            while (true) // Keep the loop running until explicitly break out of it.
            {
                // Synchronize access to formCount if necessary.
                int currentFormCount = Interlocked.CompareExchange(ref formCount, 0, 0); // Snapshot of formCount

                // Check if we've reached or exceeded the number of maximum forms.
                if (currentFormCount >= maxForms)
                {
                    System.Diagnostics.Debug.WriteLine("Reached maximum form limit. Exiting spawn loop.");
                    break; // Break out of the loop if no more forms should be created.
                }

                // Execute form creation on the appropriate thread.
                InvokeCreateForm();

                // Incremental delay for the next form (e.g., 2 seconds longer than the previous delay)
                delayBetweenForms += 2000; // Adjust the increment as needed.

                // Wait for the specified delay before trying to create the next form.
                await Task.Delay(delayBetweenForms);
            }
        }

        private void InvokeCreateForm()
        {
            // Invoke the form creation on the UI thread if necessary
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(CreateFormOnRandomScreen));
            }
            else
            {
                CreateFormOnRandomScreen();  // Direct call if already on the UI thread
            }
        }

        protected override void Dispose(bool disposing)
        {
            // Dispose of resources properly when form is disposed
            if (disposing)
            {
                if (components != null) components.Dispose();
                // Dispose other managed resources
            }
            base.Dispose(disposing);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            // Perform actions when form is closed, like disposing shared audio
            formCount--;
            openForms.Remove(this);
            if (formCount == 0 && sharedAudio != null)
            {
                sharedAudio.Dispose();
            }
            base.OnFormClosed(e);
        }



        // Diagnotic tool for determining how many forms are on screen
       /* private void CheckFormCount()
        {
            // Count the actual number of visible forms
            int actualFormCount = openForms.Count;

            // Log the expected vs. actual count
            System.Diagnostics.Debug.WriteLine($"Expected form count: {formCount}, Actual visible form count: {actualFormCount}");

            // Detailed logging for discrepancy
            if (actualFormCount < formCount)
            {
                System.Diagnostics.Debug.WriteLine("Discrepancy detected in form count.");

                // Log details about each open form (e.g., location, visibility)
                foreach (var form in openForms)
                {
                    System.Diagnostics.Debug.WriteLine($"Form at {form.Location}, Visible: {form.Visible}");
                }
            }
        }*/
    }
}
