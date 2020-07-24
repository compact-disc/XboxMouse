using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;

namespace XboxMouse
{
    public partial class XboxMouse : Form
    {

        private Thread MouseThread;
        private Boolean Enabled;

        public XboxMouse()
        {
            InitializeComponent();
            this.Enabled = false;
            StatusLabel.Text = "Not Running";
            StatusLabel.BackColor = Color.IndianRed;
            this.ActiveControl = StartButton;
        }

        private void CloseApplication(object sender, EventArgs e)
        {
            if (this.Enabled)
            {
                MouseThread.Abort();
                this.Enabled = false;
            }

            System.Windows.Forms.Application.Exit();
        }

        private void OpenAbout(object sender, EventArgs e)
        {
            About about = new About();
            about.Show();
        }

        private void StartXboxMouse(object sender, EventArgs e)
        {
            if(this.Enabled == false)
            {
                MouseThread = new Thread(new ThreadStart(MouseDriver));
                MouseThread.Start();

                this.Enabled = true;

                StatusLabel.Text = "Running";
                StatusLabel.BackColor = Color.PaleGreen;

            }
        }

        private void MouseDriver()
        {
            XboxMouseDriver XDriver = new XboxMouseDriver();
        }

        private void StopXboxMouse(object sender, EventArgs e)
        {
            if(this.Enabled == true)
            {
                MouseThread.Abort();
                this.Enabled = false;
                StatusLabel.Text = "Not Running";
                StatusLabel.BackColor = Color.IndianRed;
            }
        }
    }
}
