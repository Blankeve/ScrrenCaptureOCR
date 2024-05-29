using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ScreenCaptureOCR.forms
{
    public partial class LoadingMessageBox : Form
    {
        private PictureBox pictureBoxBackground;
        private Form parentForm;

        public LoadingMessageBox(Form parentForm, string message, MessageBoxIcon icon)
        {
            InitializeComponent();
            this.parentForm = parentForm; // 记录父窗体
            SetupImage();
            SetupMessageBox(parentForm, message, icon);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            parentForm.Enabled = true; // 启用父窗体
        }

        private void SetupImage()
        {
            // 创建一个PictureBox控件来显示GIF动画
            pictureBoxBackground = new PictureBox();
            pictureBoxBackground.Dock = DockStyle.Fill;
            pictureBoxBackground.Image = Properties.Resources.loading;
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            pictureBoxBackground.SizeMode = PictureBoxSizeMode.StretchImage;
            this.Controls.Add(pictureBoxBackground);
        }

        private void SetupMessageBox(Form parentForm, string message, MessageBoxIcon icon)
        {
            // 禁止窗体调整大小
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.ShowInTaskbar = false;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Icon = Properties.Resources.appicon1;
            this.Text = message;
            // 设置消息框大小为指定的大小
            this.Size = new Size(parentForm.Width - 100, parentForm.Width - 100);
            // 设置消息框居中显示
            this.StartPosition = FormStartPosition.Manual;

            // 计算子窗体的位置，使其在父窗体的中间显示
            this.Location = new Point(
                parentForm.Location.X + (parentForm.Width - this.Width) / 2,
                parentForm.Location.Y + (parentForm.Height - this.Height) / 2
            );

            this.TransparencyKey = Color.Transparent; // 设置透明色
            this.TopMost = true; // 显示在最顶层
        }

        private void CustomMessageBox_Load(object sender, EventArgs e)
        {
            parentForm.Enabled = false; // 禁用父窗体
        }
    }
}
