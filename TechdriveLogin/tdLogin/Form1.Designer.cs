namespace TechdriveLogin
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.tbUsername = new Guna.UI2.WinForms.Guna2TextBox();
            this.tbPassword = new Guna.UI2.WinForms.Guna2TextBox();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblSubtitle = new System.Windows.Forms.Label();
            this.lblWelcome = new System.Windows.Forms.Label();
            this.cbRemeber = new Guna.UI2.WinForms.Guna2CheckBox();
            this.btnLogin = new Guna.UI2.WinForms.Guna2Button();
            this.lblLinkForgot = new System.Windows.Forms.LinkLabel();
            this.cbUnhidepassword = new Guna.UI2.WinForms.Guna2CheckBox();
            this.pnlCard = new System.Windows.Forms.Panel();
            this.pnlAccentTop = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.pnlCard.SuspendLayout();
            this.SuspendLayout();

            // 
            // pnlCard — center white-ish card panel
            // 
            this.pnlCard.BackColor = System.Drawing.Color.FromArgb(13, 27, 62);
            this.pnlCard.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.pnlCard.Controls.Add(this.pnlAccentTop);
            this.pnlCard.Controls.Add(this.pictureBox1);
            this.pnlCard.Controls.Add(this.lblTitle);
            this.pnlCard.Controls.Add(this.lblSubtitle);
            this.pnlCard.Controls.Add(this.lblWelcome);
            this.pnlCard.Controls.Add(this.tbUsername);
            this.pnlCard.Controls.Add(this.tbPassword);
            this.pnlCard.Controls.Add(this.cbRemeber);
            this.pnlCard.Controls.Add(this.lblLinkForgot);
            this.pnlCard.Controls.Add(this.cbUnhidepassword);
            this.pnlCard.Controls.Add(this.btnLogin);
            this.pnlCard.Location = new System.Drawing.Point(40, 30);
            this.pnlCard.Name = "pnlCard";
            this.pnlCard.Size = new System.Drawing.Size(380, 560);
            this.pnlCard.TabIndex = 0;

            // 
            // pnlAccentTop — thin cyan gradient bar at top of card
            // 
            this.pnlAccentTop.BackColor = System.Drawing.Color.FromArgb(0, 191, 255);
            this.pnlAccentTop.Location = new System.Drawing.Point(0, 0);
            this.pnlAccentTop.Name = "pnlAccentTop";
            this.pnlAccentTop.Size = new System.Drawing.Size(380, 4);
            this.pnlAccentTop.TabIndex = 20;

            // 
            // pictureBox1 — company logo
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.BackgroundImage")));
            this.pictureBox1.Location = new System.Drawing.Point(115, 18);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(150, 70);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);

            // 
            // lblTitle — "TECHDRIVE" in large bold
            // 
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(30, 95);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(320, 42);
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "TECHDRIVE";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // 
            // lblSubtitle — "CAR RENTALS" in cyan
            // 
            this.lblSubtitle.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSubtitle.ForeColor = System.Drawing.Color.FromArgb(0, 191, 255);
            this.lblSubtitle.Location = new System.Drawing.Point(30, 137);
            this.lblSubtitle.Name = "lblSubtitle";
            this.lblSubtitle.Size = new System.Drawing.Size(320, 16);
            this.lblSubtitle.TabIndex = 2;
            this.lblSubtitle.Text = "— CAR RENTALS —";
            this.lblSubtitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // 
            // lblWelcome — "Sign in to your account"
            // 
            this.lblWelcome.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWelcome.ForeColor = System.Drawing.Color.FromArgb(160, 190, 220);
            this.lblWelcome.Location = new System.Drawing.Point(30, 168);
            this.lblWelcome.Name = "lblWelcome";
            this.lblWelcome.Size = new System.Drawing.Size(320, 22);
            this.lblWelcome.TabIndex = 3;
            this.lblWelcome.Text = "Sign in to your account";
            this.lblWelcome.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // 
            // tbUsername — Guna2 rounded text box
            // 
            this.tbUsername.BackColor = System.Drawing.Color.Transparent;
            this.tbUsername.BorderColor = System.Drawing.Color.FromArgb(30, 80, 160);
            this.tbUsername.BorderRadius = 8;
            this.tbUsername.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.tbUsername.DefaultText = "";
            this.tbUsername.DisabledState.BorderColor = System.Drawing.Color.FromArgb(30, 80, 160);
            this.tbUsername.DisabledState.FillColor = System.Drawing.Color.FromArgb(8, 20, 50);
            this.tbUsername.DisabledState.ForeColor = System.Drawing.Color.Gray;
            this.tbUsername.DisabledState.PlaceholderForeColor = System.Drawing.Color.Gray;
            this.tbUsername.FillColor = System.Drawing.Color.FromArgb(8, 20, 50);
            this.tbUsername.FocusedState.BorderColor = System.Drawing.Color.FromArgb(0, 191, 255);
            this.tbUsername.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbUsername.ForeColor = System.Drawing.Color.White;
            this.tbUsername.HoverState.BorderColor = System.Drawing.Color.FromArgb(100, 160, 220);
            this.tbUsername.Location = new System.Drawing.Point(30, 205);
            this.tbUsername.Name = "tbUsername";
            this.tbUsername.PasswordChar = '\0';
            this.tbUsername.PlaceholderForeColor = System.Drawing.Color.FromArgb(100, 140, 180);
            this.tbUsername.PlaceholderText = "Username";
            this.tbUsername.SelectedText = "";
            this.tbUsername.Size = new System.Drawing.Size(320, 46);
            this.tbUsername.TabIndex = 1;
            this.tbUsername.TextOffset = new System.Drawing.Point(12, 0);
            this.tbUsername.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbUsername_KeyDown);

            // 
            // tbPassword — Guna2 rounded password box
            // 
            this.tbPassword.BackColor = System.Drawing.Color.Transparent;
            this.tbPassword.BorderColor = System.Drawing.Color.FromArgb(30, 80, 160);
            this.tbPassword.BorderRadius = 8;
            this.tbPassword.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.tbPassword.DefaultText = "";
            this.tbPassword.DisabledState.BorderColor = System.Drawing.Color.FromArgb(30, 80, 160);
            this.tbPassword.DisabledState.FillColor = System.Drawing.Color.FromArgb(8, 20, 50);
            this.tbPassword.DisabledState.ForeColor = System.Drawing.Color.Gray;
            this.tbPassword.DisabledState.PlaceholderForeColor = System.Drawing.Color.Gray;
            this.tbPassword.FillColor = System.Drawing.Color.FromArgb(8, 20, 50);
            this.tbPassword.FocusedState.BorderColor = System.Drawing.Color.FromArgb(0, 191, 255);
            this.tbPassword.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbPassword.ForeColor = System.Drawing.Color.White;
            this.tbPassword.HoverState.BorderColor = System.Drawing.Color.FromArgb(100, 160, 220);
            this.tbPassword.Location = new System.Drawing.Point(30, 267);
            this.tbPassword.Name = "tbPassword";
            this.tbPassword.PasswordChar = '\0';
            this.tbPassword.PlaceholderForeColor = System.Drawing.Color.FromArgb(100, 140, 180);
            this.tbPassword.PlaceholderText = "Password";
            this.tbPassword.SelectedText = "";
            this.tbPassword.Size = new System.Drawing.Size(320, 46);
            this.tbPassword.TabIndex = 2;
            this.tbPassword.TextOffset = new System.Drawing.Point(12, 0);
            this.tbPassword.UseSystemPasswordChar = true;
            this.tbPassword.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbPassword_KeyDown);

            // 
            // cbRemeber — Remember Me checkbox
            // 
            this.cbRemeber.AutoSize = true;
            this.cbRemeber.CheckedState.BorderColor = System.Drawing.Color.FromArgb(0, 191, 255);
            this.cbRemeber.CheckedState.BorderRadius = 3;
            this.cbRemeber.CheckedState.BorderThickness = 0;
            this.cbRemeber.CheckedState.FillColor = System.Drawing.Color.FromArgb(0, 191, 255);
            this.cbRemeber.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbRemeber.ForeColor = System.Drawing.Color.FromArgb(160, 190, 220);
            this.cbRemeber.Location = new System.Drawing.Point(30, 328);
            this.cbRemeber.Name = "cbRemeber";
            this.cbRemeber.Size = new System.Drawing.Size(104, 19);
            this.cbRemeber.TabIndex = 3;
            this.cbRemeber.Text = "Remember me";
            this.cbRemeber.UncheckedState.BorderColor = System.Drawing.Color.FromArgb(50, 90, 150);
            this.cbRemeber.UncheckedState.BorderRadius = 3;
            this.cbRemeber.UncheckedState.BorderThickness = 1;
            this.cbRemeber.UncheckedState.FillColor = System.Drawing.Color.Transparent;

            // 
            // lblLinkForgot — Forgot Password link
            // 
            this.lblLinkForgot.ActiveLinkColor = System.Drawing.Color.White;
            this.lblLinkForgot.AutoSize = true;
            this.lblLinkForgot.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLinkForgot.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.lblLinkForgot.LinkColor = System.Drawing.Color.FromArgb(0, 191, 255);
            this.lblLinkForgot.Location = new System.Drawing.Point(236, 330);
            this.lblLinkForgot.Name = "lblLinkForgot";
            this.lblLinkForgot.Size = new System.Drawing.Size(113, 15);
            this.lblLinkForgot.TabIndex = 4;
            this.lblLinkForgot.TabStop = true;
            this.lblLinkForgot.Text = "Forgot Password?";
            this.lblLinkForgot.VisitedLinkColor = System.Drawing.Color.FromArgb(0, 191, 255);
            this.lblLinkForgot.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblLinkForgot_LinkClicked);

            // 
            // cbUnhidepassword — Show/Hide password checkbox
            // 
            this.cbUnhidepassword.AutoSize = true;
            this.cbUnhidepassword.CheckedState.BorderColor = System.Drawing.Color.FromArgb(135, 226, 98);
            this.cbUnhidepassword.CheckedState.BorderRadius = 3;
            this.cbUnhidepassword.CheckedState.BorderThickness = 0;
            this.cbUnhidepassword.CheckedState.FillColor = System.Drawing.Color.FromArgb(135, 226, 98);
            this.cbUnhidepassword.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbUnhidepassword.ForeColor = System.Drawing.Color.FromArgb(160, 190, 220);
            this.cbUnhidepassword.Location = new System.Drawing.Point(30, 360);
            this.cbUnhidepassword.Name = "cbUnhidepassword";
            this.cbUnhidepassword.Size = new System.Drawing.Size(115, 19);
            this.cbUnhidepassword.TabIndex = 5;
            this.cbUnhidepassword.Text = "Show Password";
            this.cbUnhidepassword.UncheckedState.BorderColor = System.Drawing.Color.FromArgb(50, 90, 150);
            this.cbUnhidepassword.UncheckedState.BorderRadius = 3;
            this.cbUnhidepassword.UncheckedState.BorderThickness = 1;
            this.cbUnhidepassword.UncheckedState.FillColor = System.Drawing.Color.Transparent;
            this.cbUnhidepassword.CheckedChanged += new System.EventHandler(this.cbUnhidepassword_CheckedChanged);

            // 
            // btnLogin — Guna2 styled Login button with lime-green from logo
            // 
            this.btnLogin.BorderRadius = 8;
            this.btnLogin.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLogin.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnLogin.DisabledState.FillColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.btnLogin.DisabledState.ForeColor = System.Drawing.Color.Gray;
            this.btnLogin.FillColor = System.Drawing.Color.FromArgb(0, 191, 255);
            this.btnLogin.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLogin.ForeColor = System.Drawing.Color.FromArgb(5, 15, 40);
            this.btnLogin.HoverState.FillColor = System.Drawing.Color.FromArgb(135, 226, 98);
            this.btnLogin.HoverState.ForeColor = System.Drawing.Color.FromArgb(5, 15, 40);
            this.btnLogin.Location = new System.Drawing.Point(30, 400);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(320, 48);
            this.btnLogin.TabIndex = 6;
            this.btnLogin.Text = "Sign In";
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);

            // 
            // Form1 — main login form
            // 
            this.AcceptButton = this.btnLogin;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(6, 12, 35);
            this.ClientSize = new System.Drawing.Size(460, 620);
            this.Controls.Add(this.pnlCard);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TechDrive Car Rentals — Login";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.pnlCard.ResumeLayout(false);
            this.pnlCard.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private Guna.UI2.WinForms.Guna2TextBox tbUsername;
        private Guna.UI2.WinForms.Guna2TextBox tbPassword;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblSubtitle;
        private System.Windows.Forms.Label lblWelcome;
        private Guna.UI2.WinForms.Guna2CheckBox cbRemeber;
        private Guna.UI2.WinForms.Guna2Button btnLogin;
        private System.Windows.Forms.LinkLabel lblLinkForgot;
        private Guna.UI2.WinForms.Guna2CheckBox cbUnhidepassword;
        private System.Windows.Forms.Panel pnlCard;
        private System.Windows.Forms.Panel pnlAccentTop;
    }
}
