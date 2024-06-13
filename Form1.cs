using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VideoView
{
    public partial class VideoView : Form
    {
        WebContent webContent;
        public VideoView()
        {
            InitializeComponent();
            webContent = new WebContent(pbVideo);
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            webContent.StopGetContent();
            btnStop.Visible = false;
            btnStart.Visible = true;
        }

        private async void btnStart_Click(object sender, EventArgs e)
        {
            btnStop.Visible = true;
            btnStart.Visible = false;
            webContent.StartGetContent();
            await webContent.GetHttpContent();
        }

    }
}
