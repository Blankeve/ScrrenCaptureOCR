using System;
using System.Drawing;
using System.Windows.Forms;
using WindowsFormsApp1.cs;

namespace ScreenCaptureOCR
{
    public enum RecognitionStatus
    {
        Success,
        Failure,
        Processing
    }

    public class CustomTitleBar : Control
    {
        private bool _dragging = false;
        private Point _startPoint = new Point(0, 0);
        private string title;
        private Label lblStatus; // 添加 Label 控件

        public CustomTitleBar(PictureBox icon,string title)
        {
            this.Height = 30; // 设置标题栏的高度
            this.Dock = DockStyle.Top;
            this.BackColor = Color.Teal;
            this.ForeColor = Color.White;
            this.DoubleBuffered = true;
            this.title = title;

            if (icon != null)
            {
                // 添加图标
                PictureBox iconPictureBox = new PictureBox();
                iconPictureBox = icon; // 替换为您的图标
                iconPictureBox.Size = new Size(20, 20);
                iconPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                iconPictureBox.Location = new Point(5, (this.Height - iconPictureBox.Height) / 2);
                this.Controls.Add(iconPictureBox);
            }



            // 添加最小化按钮
            Button minimizeButton = new Button();
            minimizeButton.Text = "-";
            minimizeButton.ForeColor = Color.White;
            minimizeButton.BackColor = Color.Transparent;
            minimizeButton.FlatStyle = FlatStyle.Flat;
            minimizeButton.Size = new Size(30, 30);
            minimizeButton.Location = new Point(this.Width - 60, 0);
            minimizeButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            minimizeButton.Click += (s, e) =>
            {
                Form parentForm = this.FindForm();
                if (parentForm != null)
                {
                    FormMain mainForm = (FormMain)parentForm;
                    Animation.AnimateWindow(mainForm.Handle, 500, Animation.AW_SLIDE | Animation.AW_VER_POSITIVE | Animation.AW_ACTIVATE | 0x00010000);
                    // 取消最小化操作，隐藏窗体到托盘
                    mainForm.Hide();
                    mainForm.notifyIconMain.Visible = true;
                }
            };
            this.Controls.Add(minimizeButton);

            // 添加关闭按钮
            Button closeButton = new Button();
            closeButton.Text = "X";
            closeButton.ForeColor = Color.White;
            closeButton.BackColor = Color.Transparent;
            closeButton.FlatStyle = FlatStyle.Flat;
            closeButton.Size = new Size(30, 30);
            closeButton.Location = new Point(this.Width - 30, 0);
            closeButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            closeButton.Click += (s, e) => { Application.Exit(); };
            this.Controls.Add(closeButton);

            // 添加 Label 控件
            lblStatus = new Label();
            lblStatus.Text = "欢迎使用";
            lblStatus.TextAlign = ContentAlignment.MiddleCenter;
            lblStatus.AutoSize = false;
            lblStatus.Visible = true; // 默认隐藏
            this.Controls.Add(lblStatus);

            lblStatus.MouseDown += new MouseEventHandler(Label_MouseDown);
            lblStatus.MouseMove += new MouseEventHandler(Label_MouseMove);
            lblStatus.MouseUp += new MouseEventHandler(Label_MouseUp);
            // 添加 Resize 事件处理程序
            this.Resize += CustomTitleBar_Resize;
        }

        private void CustomTitleBar_Resize(object sender, EventArgs e)
        {
            // 设置 Label 控件的位置
            lblStatus.Location = new Point((this.Width - lblStatus.Width) / 2, (this.Height - lblStatus.Height) / 2);
        }

        private void Label_MouseDown(object sender, MouseEventArgs e)
        {
            OnMouseDown(e);
        }

        private void Label_MouseMove(object sender, MouseEventArgs e)
        {
            OnMouseMove(e);
        }

        private void Label_MouseUp(object sender, MouseEventArgs e)
        {
            OnMouseUp(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            _dragging = true;
            _startPoint = new Point(e.X, e.Y);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (_dragging)
            {
                Form parentForm = this.FindForm();
                if (parentForm != null)
                {
                    parentForm.Location = new Point(parentForm.Location.X + e.X - _startPoint.X, parentForm.Location.Y + e.Y - _startPoint.Y);
                }
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            _dragging = false;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using (SolidBrush brush = new SolidBrush(this.ForeColor))
            {
                // 计算文字垂直居中位置
                float y = (this.Height - e.Graphics.MeasureString(title, this.Font).Height) / 2;
                e.Graphics.DrawString(title, this.Font, brush, new PointF(30, y)); // 标题文字绘制在垂直居中位置
            }
        }

        public void ShowRecognitionStatus(string status, RecognitionStatus recognitionStatus)
        {
            lblStatus.Text = status;
            switch (recognitionStatus)
            {
                case RecognitionStatus.Success:
                    lblStatus.ForeColor = Color.Orange;
                    break;
                case RecognitionStatus.Failure:
                    lblStatus.ForeColor = Color.Red;
                    break;
                case RecognitionStatus.Processing:
                    lblStatus.ForeColor = Color.White;
                    break;
                default:
                    lblStatus.ForeColor = this.ForeColor;
                    break;
            }
            lblStatus.Visible = true;
        }



        public void HideRecognitionStatus()
        {
            lblStatus.Text = "";
            lblStatus.Visible = false;
        }

    }
}
