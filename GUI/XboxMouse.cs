using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XboxMouse
{
    public partial class XboxMouse : Form
    {

        System.Threading.Thread DriverThread;

        public XboxMouse()
        {
            InitializeComponent();
        }

        private void CloseApplication(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }

        private void OpenAbout(object sender, EventArgs e)
        {
            About about = new About();
            about.Show();
        }

        private void StartXboxMouse(object sender, EventArgs e)
        {
         

            
        }

    }
}
