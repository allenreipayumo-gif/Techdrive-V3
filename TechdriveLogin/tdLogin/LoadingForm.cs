using System;
using System.Drawing;
using System.Windows.Forms;

namespace TechdriveLogin
{
    public class LoadingForm : Form
    {
        private Timer animationTimer;
        private int progressValue = 0;
        private Label statusLabel;
        private Panel progressBarBg;
        private Panel progressBarFill;

        public LoadingForm()
        {
            this.Size = new Size(520, 320);
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(2, 36, 78); // Deep Navy

            // Custom border painting
            this.Paint += (s, e) =>
            {
                using (var pen = new Pen(Color.FromArgb(29, 59, 172), 2))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, this.Width - 1, this.Height - 1);
                }
            };

            InitializeControls();

            // Set up animation timer
            animationTimer = new Timer();
            animationTimer.Interval = 30; // 30ms interval for butter-smooth animation
            animationTimer.Tick += AnimationTimer_Tick;
            animationTimer.Start();
        }

        private void InitializeControls()
        {
            // Title Label
            Label title = new Label();
            title.Text = "TECHDRIVE V3";
            title.Font = new Font("Century Gothic", 24F, FontStyle.Bold);
            title.ForeColor = Color.FromArgb(135, 226, 98); // Techdrive Green
            title.Location = new Point(50, 60);
            title.Size = new Size(420, 45);
            title.TextAlign = ContentAlignment.MiddleCenter;
            this.Controls.Add(title);

            // Subtitle
            Label subtitle = new Label();
            subtitle.Text = "Enterprise Fleet Management Systems";
            subtitle.Font = new Font("Century Gothic", 10F, FontStyle.Regular);
            subtitle.ForeColor = Color.White;
            subtitle.Location = new Point(50, 110);
            subtitle.Size = new Size(420, 20);
            subtitle.TextAlign = ContentAlignment.MiddleCenter;
            this.Controls.Add(subtitle);

            // Status Label
            statusLabel = new Label();
            statusLabel.Text = "Connecting to CockroachDB cluster...";
            statusLabel.Font = new Font("Century Gothic", 9.5F, FontStyle.Italic);
            statusLabel.ForeColor = Color.FromArgb(255, 222, 89); // Vibrant Yellow
            statusLabel.Location = new Point(50, 180);
            statusLabel.Size = new Size(420, 25);
            statusLabel.TextAlign = ContentAlignment.MiddleCenter;
            this.Controls.Add(statusLabel);

            // Custom animating progress bar background
            progressBarBg = new Panel();
            progressBarBg.BackColor = Color.FromArgb(29, 59, 172);
            progressBarBg.Location = new Point(80, 215);
            progressBarBg.Size = new Size(360, 8);
            this.Controls.Add(progressBarBg);

            // Custom animating progress bar fill
            progressBarFill = new Panel();
            progressBarFill.BackColor = Color.FromArgb(135, 226, 98); // Techdrive Green
            progressBarFill.Location = new Point(80, 215);
            progressBarFill.Size = new Size(0, 8);
            this.Controls.Add(progressBarFill);
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            progressValue += 2; // Increment progress

            // Smooth fill scaling
            progressBarFill.Width = (int)((progressValue / 100.0) * 360);

            // Dynamic loading status messages
            if (progressValue < 30)
            {
                statusLabel.Text = "Establishing secure database connection...";
                statusLabel.ForeColor = Color.FromArgb(255, 222, 89); // Yellow
            }
            else if (progressValue < 60)
            {
                statusLabel.Text = "Syncing vehicle maintenance cycles...";
                statusLabel.ForeColor = Color.FromArgb(135, 226, 98); // Green
            }
            else if (progressValue < 90)
            {
                statusLabel.Text = "Generating active booking alerts...";
                statusLabel.ForeColor = Color.FromArgb(135, 226, 98);
            }
            else
            {
                statusLabel.Text = "Welcome back, Admin! Redirecting...";
                statusLabel.ForeColor = Color.White;
            }

            // Close form once loading is 100% complete
            if (progressValue >= 100)
            {
                animationTimer.Stop();
                animationTimer.Dispose();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}
