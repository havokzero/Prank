using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Resources;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;


namespace Havoks_Virus
{
    public class PrankForm : Form
    {
        // Member declarations
        private PictureBox pictureBox = new PictureBox();
        private List<Image> gifFrames = new List<Image>(); // Stores each frame of the GIF
        private int currentFrame = 0; // Tracks the current frame of the GIF
        private Timer moveTimer = new Timer(); // Timer for controlling movement speed
        private static int formCount = 0; // Keep track of the number of forms
        private const int maxForms = 12; // Maximum number of forms allowed
        private List<Image> loadedImages = new List<Image>(); // To store loaded images
        private static List<PrankForm> openForms = new List<PrankForm>(); // Track all open forms
        private Timer wallpaperTimer = new Timer(); // Timer for changing the wallpaper
        private string originalWallpaperPath; // To store the path of the original wallpaper
       // private static Audio sharedAudio = new Audio(); // Audio system for the prank form            // Audio is not initialized in this class as it created a crazy loop, it will generate in Program.cs
        private Random random = new Random(); // Random number generator
        private Mouse prankMouse; // Declare the Mouse member
        private static bool wallpaperHasBeenSet = false; // Ensure wallpaper is only set once
        private DtIcon prankIconManager;
        //private Mouse prankMouse; // Declare the Mouse member


        // Constructor
        public PrankForm()
        {
            InitializeComponent(); // Initializes components from the designer file.
            InitializePrankForm(); // Further configuration for prank behavior.
            prankIconManager = new DtIcon(); // Instantiate DtIcon class
            prankIconManager.HideDesktopIcons(); // Hide all desktop icons
            prankIconManager.CreatePrankIcons(); // Create prank icons on the desktop
            //InitializeMouse();
        }

        // InitializePrankForm handles all the setup and configuration
        private void InitializePrankForm()
        {
            ConfigureFormProperties(); // Set the form's visual and behavioral properties.
            ConfigurePictureBox();    // Set up the PictureBox to display the GIF.
            LoadAndDisplayGif("Media/blade.gif"); // Load and display the initial GIF.

            // Initialize and start background audio if the file paths are correctly set.
          //  sharedAudio = new Audio(); // Ensure the Audio class handles file not found or other errors gracefully.
          //  sharedAudio.Start();       // Start audio playback.

            SetupTimer(); // Configure a timer for tasks such as moving the form or changing GIF frames.
            this.Shown += PrankForm_Shown; // Attach event handler for when the form is first shown.

            originalWallpaperPath = GetOriginalWallpaperPath(); // Save original wallpaper path for restoration later.
            SetInitialWallpaper(); // Change the wallpaper to the prank wallpaper.

            ManageFormSpawning(); // Handle the creation of additional forms if needed.

            System.Diagnostics.Debug.WriteLine($"Form {formCount} created."); // Use Debug.WriteLine for debugging to avoid console outputs in release.

            formCount++; // Increment the global form count.
            openForms.Add(this); // Add this form instance to the list of open forms.

            prankMouse = new Mouse("Media/rspin.ani"); // Ensure the path is correct!
            prankMouse.StartMouseMovement();
            prankIconManager = new DtIcon();
        }

        private void InitializeMouse()
        {
            // Initialize and set the mouse cursor and start jitter effect
            prankMouse = new Mouse("Media/rspin.ani"); // Ensure the path is correct!
            prankMouse.StartMouseMovement();
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x80;  // WS_EX_TOOLWINDOW style hides the form from Alt+Tab
                return cp;
            }
        }

        private void PrankForm_Shown(object sender, EventArgs e)
        {
            SetupTimer(); // Start updating frames only after form is shown
            SetupMovementTimer();
            RepositionFormRandomly();
        }

        private void InitializeComponent()
        {
            ComponentResourceManager resources = new ComponentResourceManager(typeof(PrankForm));
            SuspendLayout();
            // 
            // PrankForm
            // 
            ClientSize = new Size(284, 261);
            Icon = (Icon)resources.GetObject("$spin.ico");
            Name = "PrankForm";
            ResumeLayout(false);
        }

        private void ConfigureFormProperties()
        {
           // this.Text = "Test Form"; // Set a title to see in the taskbar
            this.DoubleBuffered = true; // Reduce flickering effect
            this.Size = new Size(200, 200); // Set the size of the prank form
            this.BackColor = Color.Black;
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.TopMost = true;
            this.TransparencyKey = Color.Black;
            this.ShowInTaskbar = false;
        }

        private void ConfigurePictureBox()
        {
            pictureBox.Dock = DockStyle.Fill; // Make PictureBox fill the entire form
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom; // Set the PictureBox to zoom image
            this.Controls.Add(pictureBox); // Add PictureBox to form's controls
        }

        private void LoadAndDisplayGif(string gifPath)
        {
            try
            {
                Image gifImg = Image.FromFile(gifPath); // Load the GIF image from the specified path
                pictureBox.Image = gifImg; // Set the PictureBox's Image to the loaded GIF
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load GIF: " + ex.Message); // Show error message if loading fails
            }
        }
        
        private string GetOriginalWallpaperPath()
        {
            return Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop")?.GetValue("WallPaper").ToString() ?? string.Empty;
        }

        private void SetInitialWallpaper()
        {
            if (!wallpaperHasBeenSet)
            {
                // Set wallpaper here
                string wallpaperPath = Application.StartupPath + "Media/wallpaper.jpg";
                //System.Console.WriteLine($"Setting wallpaper from: {wallpaperPath}");     // this is used for debugging
                WallpaperChanger.SetWallpaper(wallpaperPath, WallpaperChanger.Style.Stretched);

                wallpaperHasBeenSet = true;
            }
        }

        private void ChangeWallpaper()
        {
            // Method to actually change the wallpaper
            string wallpaperPath = Application.StartupPath + "Media/wallpaper.jpg";
           // System.Console.WriteLine($"Setting wallpaper from: {wallpaperPath}");     // this is used for debugging
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
              //  System.Console.WriteLine("Error loading heavy content: " + ex.Message);       this will not be passed in final product
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
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            //Dispose of the Mouse instance
            prankMouse?.Dispose();

            // Dispose managed resources
            components?.Dispose();
            moveTimer?.Dispose();
            wallpaperTimer?.Dispose();

            // Add other cleanup code if necessary
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
         //   if (formCount == 1 && sharedAudio != null)
            {               
                //sharedAudio.Dispose();
               prankMouse?.Dispose(); // Dispose of the Mouse instance when form is closed
            }

            openForms.Remove(this);
            formCount--;

            base.OnFormClosed(e);

            prankIconManager.RestoreDesktop(); // Remove prank icons
            prankIconManager.ShowDesktopIcons(); // Show desktop icons again
            // Dispose of other resources and perform clean-up
            // Dispose of the Mouse instance
            prankMouse?.Dispose();

            // Dispose managed resources
            components?.Dispose();
            moveTimer?.Dispose();
            wallpaperTimer?.Dispose();
            // Add other cleanup code if necessary
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
            //ensure the form is visible and positioned randomly on screen
            this.Visible = true;
            Rectangle newLocation;
            int screenWidth = Screen.PrimaryScreen.WorkingArea.Width;
            int screenHeight = Screen.PrimaryScreen.WorkingArea.Height;

            do
            {
                // Calculate the maximum allowable coordinates
                // These are set to one to ensure no negative values are called
                // which could happen if set to zero
                int newX = random.Next(Math.Max(screenWidth - this.Width, 1));
                int newY = random.Next(Math.Max(screenHeight - this.Height, 1));

                newLocation = new Rectangle(newX, newY, this.Width, this.Height);
                // Keep trying new locations until one doesn't overlap
            } while (IsOverlapping(newLocation));

            // Set the form's location to the new non-overlapping point
            this.Location = new Point(newLocation.X, newLocation.Y);
        }

        private bool IsOverlapping(Rectangle newLocation)
        {
            foreach (var form in openForms)
            {
                // Don't compare the form to itself
                if (form == this) continue;
                // Get the existing form's location and size as a rectangle
                Rectangle existingLocation = new Rectangle(form.Location, form.Size);
                // Optionally reduce the size of the rectangle here if you want to allow some overlapping
                existingLocation.Inflate(-100, -100);  // adjust overlap tolerance  Default value for zero overlap on forms is -5 to -10
                // Check if the new location overlaps with the existing one
                if (newLocation.IntersectsWith(existingLocation))
                    return true;  // Overlap found
            }
            return false;  // No overlap found
        }

        private Timer movementTimer = new Timer();

        private void SetupMovementTimer()
        {
            Random rng = new Random();
            int minInterval = 1000; // Minimum interval in milliseconds (1 second)
            int maxInterval = 8000; // Maximum interval in milliseconds (8 seconds)
            movementTimer.Interval = rng.Next(minInterval, maxInterval); // Set a random interval between the min and max
            movementTimer.Tick += (sender, e) => RepositionFormRandomly();
            movementTimer.Start();
        }

        private void ManageFormSpawning()
        {
            if (formCount < maxForms)
            {
             //   sharedAudio.Start();

                Timer spawnTimer = new Timer { Interval = 2000 };
                spawnTimer.Tick += (sender, e) => SpawnAdditionalForm();
                spawnTimer.Start();
            }
        }
    }
}