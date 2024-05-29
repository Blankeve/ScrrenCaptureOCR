using System;
using System.Drawing;
using System.Windows.Forms;

namespace SimpleOCR
{
    public partial class ZoomedImageForm : Form
    {
        private PictureBox pictureBoxZoom;

        public ZoomedImageForm(Form parentForm)
        {
            this.Owner = parentForm; // 设置主窗体为当前窗体的所有者
            InitializeComponent();
        }

        public ZoomedImageForm(Image image)
        {
            // 初始化窗体和 PictureBox
            this.Text = "放大图像";
            this.Size = new Size(800, 600); // 设置初始窗体大小
            this.StartPosition = FormStartPosition.CenterParent;
            this.Icon = ScreenCaptureOCR.Properties.Resources.enlargement;
            pictureBoxZoom = new PictureBox
            {
                Dock = DockStyle.Fill,
                Image = image,
                SizeMode = PictureBoxSizeMode.Zoom
            };

            this.Controls.Add(pictureBoxZoom);
        }

        private void ZoomedImageForm_Load(object sender, EventArgs e)
        {

        }
    }
}
