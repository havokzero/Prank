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
        private const int maxForms = 10;  // Maximum number of forms allowed ..... Don't actually kill their PC 6-10 seem to work perfect
        private List<Image> loadedImages = new List<Image>(); // To store loaded images
        private static List<PrankForm> openForms = new List<PrankForm>();
        private Timer wallpaperTimer = new Timer();
        private string originalWallpaperPath;
        //private Audio backgroundAudio;
        private Audio sharedAudio = new Audio();
        private Random random = new Random();
        

        public PrankForm()
        {
            InitializeComponent();
            SetupMovementTimer();
            originalWallpaperPath = GetOriginalWallpaperPath(); // Store original wallpaper path
            ExtractFramesFromGif("Media/blade.gif");
            LoadContentAsync(); // Load content asynchronously
            SetupTimer();
            SetupWallpaperChangeTimer();
            //sharedAudio = new Audio(); // Initialize the background audio
            //sharedAudio.Start(); // Start playing background audio

            openForms.Add(this); // Track this form
            formCount++; // Increment the global form count


            this.Size = new Size(200, 200);

            // Only initiate additional forms if under the maximum limit
            if (formCount < maxForms)
            {
                // Only create additional forms from the first form
                if (openForms.Count == 1)
                {
                    CreateFormsForAllScreens();
                }

                // Setup timer to spawn additional forms
                Timer spawnTimer = new Timer();
                spawnTimer.Interval = 8000; // 8 seconds, adjust as needed
                spawnTimer.Tick += (sender, e) => SpawnAdditionalForm();
                spawnTimer.Start();

                // Setup wallpaper change timer
                Timer wallpaperTimer = new Timer();
                wallpaperTimer.Interval = 30000; // 30 seconds
                wallpaperTimer.Tick += (sender, e) => ChangeWallpaper();
                wallpaperTimer.Start();

                // Set initial wallpaper
                string wallpaperPath = Application.StartupPath + "\\Media\\wallpaper.jpg";
                System.Console.WriteLine("Attempting to set wallpaper from: " + wallpaperPath);
                WallpaperChanger.SetWallpaper(wallpaperPath, WallpaperChanger.Style.Stretched);

                // Initialize shared audio once for the first form
                if (formCount == 0)
                {
                    sharedAudio = new Audio(); // Initialize only once
                    sharedAudio.Start(); // Start playing background audio
                }

                openForms.Add(this);
                formCount++;
            }
        }

        private void openCmdButton_Click(object sender, EventArgs e)
        {
            TerminalOpener.OpenCommandPrompt();
        }
        

        private void someButton_Click(object sender, EventArgs e)
        {
            System.Console.WriteLine("Button Clicked");
            // Perform button click operations
        }


        private void InitializeComponent()
        {
            // Initialize and configure PictureBox
            this.pictureBox.Dock = DockStyle.Fill;
            this.pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            this.Controls.Add(pictureBox);

            // Configure form properties
            this.FormBorderStyle = FormBorderStyle.None; // No borders
            this.TopMost = true; // Always on top
            this.StartPosition = FormStartPosition.CenterScreen; // Center screen
            this.Size = new Size(200, 200); // Size of the form
            this.BackColor = Color.Black; // Background color
            this.TransparencyKey = Color.Black; // Make the same color transparent
            this.ShowInTaskbar = false;  // Hide form from appearing in the taskbar
            this.ResumeLayout(false);
            
        }

        private string GetOriginalWallpaperPath()
        {
            return Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop")?.GetValue("WallPaper").ToString() ?? string.Empty;
        }

        private void SetupWallpaperChangeTimer()
        {
            wallpaperTimer.Interval = 1000; // 30 seconds
            wallpaperTimer.Tick += (sender, e) => ChangeWallpaper();
            wallpaperTimer.Start();
        }

        private void ChangeWallpaper()
        {
            string wallpaperPath = Path.Combine(Application.StartupPath, "Media\\wallpaper.jpg");
            WallpaperChanger.SetWallpaper(wallpaperPath, WallpaperChanger.Style.Stretched); // Choose style as needed
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
            moveTimer.Interval = new Random().Next(100, 1000); // Random movement speed for each form
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

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            // Dispose of other resources here
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
            movementTimer.Interval = 1000; // Adjust as needed for movement frequency
            movementTimer.Tick += (sender, e) => RepositionFormRandomly();
            movementTimer.Start();
        }
    }
}