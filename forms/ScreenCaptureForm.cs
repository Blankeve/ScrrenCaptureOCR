using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace SimpleOCR
{
    public partial class ScreenCaptureForm : Form
    {
        private Point startPoint;
        private Rectangle rect;
        private bool isCapturing;
        public string CapturedImagePath { get; private set; }

        public ScreenCaptureForm()
        {
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopMost = true;
            this.Opacity = 0.5;
            this.BackColor = Color.Black;
            this.DoubleBuffered = true;

            this.MouseDown += ScreenCaptureForm_MouseDown;
            this.MouseMove += ScreenCaptureForm_MouseMove;
            this.MouseUp += ScreenCaptureForm_MouseUp;
            this.KeyPreview = true; // 启用键盘事件预览
            this.KeyDown += ScreenCaptureForm_KeyDown;

        }

        private void ScreenCaptureForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }


        private void ScreenCaptureForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                startPoint = e.Location;
                isCapturing = true;
            }
        }

        private void ScreenCaptureForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (isCapturing)
            {
                Point endPoint = e.Location;
                rect = new Rectangle(Math.Min(startPoint.X, endPoint.X), Math.Min(startPoint.Y, endPoint.Y), Math.Abs(startPoint.X - endPoint.X), Math.Abs(startPoint.Y - endPoint.Y));
                this.Invalidate(); // Redraw the form
            }
        }

        private void ScreenCaptureForm_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isCapturing = false;
                if (rect.Width > 0 && rect.Height > 0)
                {
                    CaptureRectangle();
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (isCapturing)
            {
                using (Pen pen = new Pen(Color.Red, 2))
                {
                    e.Graphics.DrawRectangle(pen, rect);
                }
            }
        }

        private void CaptureRectangle()
        {
            Bitmap screenshot = new Bitmap(rect.Width, rect.Height);
            using (Graphics g = Graphics.FromImage(screenshot))
            {
                g.CopyFromScreen(rect.Location, Point.Empty, rect.Size);
            }
            string tempPath = Path.Combine(Path.GetTempPath(), $"screenshot_{Guid.NewGuid()}.png");
            screenshot.Save(tempPath, ImageFormat.Png);
            CapturedImagePath = tempPath;
        }

        private void ScreenCaptureForm_Load(object sender, EventArgs e)
        {

        }
    }
}

