using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScreenCaptureOCR.forms
{
    public partial class SupportForm : Form
    {
        private Form parentForm;
        private PictureBox pictureBoxBackground;

        public SupportForm(Form parentForm)
        {
            InitializeComponent();
            this.parentForm = parentForm; // 记录父窗体
            SetupImage();
            SetupMessageBox();
        }

        private void SetupImage()
        {
            // 创建一个PictureBox控件来显示GIF动画
            pictureBoxBackground = new PictureBox();
            pictureBoxBackground.Dock = DockStyle.Fill;
            //pictureBoxBackground.Image = Properties.Resources.zfpay;
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            pictureBoxBackground.SizeMode = PictureBoxSizeMode.StretchImage;
            this.Controls.Add(pictureBoxBackground);
        }

        private void SetupMessageBox()
        {
            // 禁止窗体调整大小
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.ShowInTaskbar = false;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Icon = Properties.Resources.support;
            this.Text = "";
            // 设置消息框大小为指定的大小
            this.Size = new Size(parentForm.Width - 100, parentForm.Width - 100);
/*            this.TransparencyKey = Color.Transparent; // 设置透明色
*/            this.TopMost = true; // 显示在最顶层
        }

        private void SupportForm_Load(object sender, EventArgs e)
        {
            this.StartPosition = FormStartPosition.Manual;

            // 计算子窗体的位置，使其在父窗体的中间显示
            this.Location = new Point(
                parentForm.Location.X + (parentForm.Width - this.Width) / 2,
                parentForm.Location.Y + (parentForm.Height - this.Height) / 2
            );

        }
    }
}
