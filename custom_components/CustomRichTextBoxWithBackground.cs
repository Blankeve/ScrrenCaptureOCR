using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ScreenCaptureOCR
{
    public class CustomRichTextBoxWithBackground : Control
    {
        private RichTextBox richTextBox;
        private Image backgroundImage;

        public CustomRichTextBoxWithBackground()
        {
            // 创建 RichTextBox 控件
            richTextBox = new RichTextBox();
            richTextBox.Dock = DockStyle.Fill;

            // 设置 RichTextBox 的父控件为当前自定义控件
            richTextBox.Parent = this;

            // 隐藏 RichTextBox 的边框
            SendMessage(richTextBox.Handle, EM_SETRECT, IntPtr.Zero, IntPtr.Zero);

            // 添加 RichTextBox 控件到自定义控件中
            Controls.Add(richTextBox);
        }

        // 导入 Windows API 函数
        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);
        private const int EM_SETRECT = 0xB2;

        public Image BackgroundImage
        {
            get { return backgroundImage; }
            set
            {
                backgroundImage = value;
                Invalidate(); // 重绘控件
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // 如果有背景图像，则绘制背景图像
            if (backgroundImage != null)
            {
                e.Graphics.DrawImage(backgroundImage, ClientRectangle);
            }
        }
    }
}
