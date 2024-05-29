using ScreenCaptureOCR.Properties;
using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace ScreenCaptureOCR.forms
{
    public partial class NotifyMessageBox : Form
    {
        private PictureBox pictureBoxStatus;
        private Label labelText = new Label();
        private Button closeButton = new Button();
        private Form parentForm;
        private Timer timer;
        private int targetY;
        private const int AnimationDuration = 1000; // 动画持续时间（毫秒）
        private const int AnimationSteps = 50; // 动画步数
        private int stepCount;

        public NotifyMessageBox(Form parentForm)
        {
            this.parentForm = parentForm;
            InitializeComponent();
            SetupMessageBox();
            SetupAnimationTimer();
            this.Opacity = 0; // 设置窗体初始不透明度为0
            this.Owner = parentForm; // 设置主窗体为当前窗体的所有者
            this.CreateHandle(); // 立即创建窗体句柄
        }

        private void SetupAnimationTimer()
        {
            timer = new Timer();
            timer.Interval = AnimationDuration / AnimationSteps;
            timer.Tick += Timer_Tick;
        }

        private async void Timer_Tick(object sender, EventArgs e)
        {
            if (stepCount <= AnimationSteps)
            {
                double deltaOpacity = 1.0 / AnimationSteps;
                Opacity += deltaOpacity;
                stepCount++;
            }
            else
            {
                timer.Stop();
                await Task.Delay(2000);
                this.Invoke((MethodInvoker)Close);
            }
        }

        private void SetupImage(string status)
        {
            MethodInvoker setImageAction = () =>
            {
                if (status.Equals("success"))
                {
                    pictureBoxStatus.Image = Properties.Resources.success;
                }
                else if (status.Equals("failed"))
                {
                    pictureBoxStatus.Image = Properties.Resources.failed;
                }
                else
                {
                    pictureBoxStatus.Image = Properties.Resources.info;
                }

                // 图片加载完成后重新计算控件位置
                pictureBoxStatus.Location = new Point(10, (this.Height - pictureBoxStatus.Height) / 2);
                labelText.Location = new Point(pictureBoxStatus.Right + 10, (this.Height - labelText.Height) / 2);
                closeButton.Location = new Point(this.Width - closeButton.Width - 10, (this.Height - closeButton.Height) / 2);
            };

            // 如果当前线程不是 UI 线程，则使用 Invoke 方法在 UI 线程上执行操作
            if (pictureBoxStatus.InvokeRequired)
            {
                pictureBoxStatus.Invoke(setImageAction);
            }
            else
            {
                setImageAction();
            }
        }

        public new void Show()
        {
            // 设置消息框位置为屏幕右上角
            int padding = 100; // 右上角距离屏幕边缘的距离
            int x = Screen.PrimaryScreen.WorkingArea.Right - Width - padding;
            int y = padding;
            Location = new Point(x, y);
            targetY = padding;
            stepCount = 0;
            timer.Start();
            base.Show(); // 调用基类的Show方法显示窗体
            this.Owner.Focus();
        }

        private void SetupMessageBox()
        {
            this.BackgroundImage = Properties.Resources.background;
            this.BackgroundImageLayout = ImageLayout.Stretch;
            pictureBoxStatus = new PictureBox();
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            this.Controls.Add(pictureBoxStatus);
            this.Controls.Add(labelText);
            this.Controls.Add(closeButton);
            this.ShowInTaskbar = false;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.White; // 设置背景色为白色
            this.StartPosition = FormStartPosition.Manual;
            this.Size = new Size(parentForm.Width - 100, pictureBoxStatus.Height);

            // 设置消息框位置为屏幕右上角
            int padding = 10; // 右上角距离屏幕边缘的距离
            int x = Screen.PrimaryScreen.WorkingArea.Right - this.Width - padding;
            int y = padding;
            this.Location = new Point(x, y);

            pictureBoxStatus.SizeMode = PictureBoxSizeMode.AutoSize;
            // 设置 labelText 的文字颜色为白色
            labelText.ForeColor = Color.Green;

            // 设置 closeButton 的位置为垂直居中
            int closeButtonY = (this.Height - closeButton.Height) / 2;
            closeButton.Location = new Point(this.Width - closeButton.Width - 10, closeButtonY);

            // 调整 labelText 的字体大小
            labelText.Font = new Font("Segoe UI", 11); // 修改为您希望的字体和大小
            labelText.AutoSize = true;

            closeButton.Size = new Size(20, 20); // 设置关闭按钮的大小
            closeButton.Location = new Point(this.Width - 20, 0); // 设置关闭按钮的位置
            closeButton.BackColor = Color.Transparent; // 设置按钮背景色为透明
            closeButton.FlatStyle = FlatStyle.Flat; // 设置按钮为扁平样式
            closeButton.FlatAppearance.BorderSize = 0; // 设置按钮边框大小为0
            closeButton.FlatAppearance.BorderColor = Color.FromArgb(0, 255, 255, 255); // 确保边框颜色为透明
            closeButton.FlatAppearance.MouseDownBackColor = Color.Transparent;
            closeButton.FlatAppearance.MouseOverBackColor = Color.Transparent;
            closeButton.BackgroundImage = Resources.close;
            closeButton.BackgroundImageLayout = ImageLayout.Stretch;
            closeButton.ForeColor = Color.Red; // 设置按钮文本颜色
            closeButton.Click += (sender, e) => this.Close(); // 添加点击事件，点击关闭窗体

            // 添加按钮获取焦点和失去焦点时的事件处理程序
            closeButton.GotFocus += (sender, e) => closeButton.ForeColor = Color.Transparent;
            closeButton.LostFocus += (sender, e) => closeButton.ForeColor = Color.Transparent;


            pictureBoxStatus.BackColor = Color.Transparent;
            labelText.BackColor = Color.Transparent;

            // this.TransparencyKey = Color.White; // 设置透明色为白色
        }

        public void ShowDialog(string text)
        {
            this.Invoke((MethodInvoker)delegate
            {
                labelText.Text = text;
                SetupImage("info");
                Show(); // 在 UI 线程中显示消息框
            });
        }

        public void ShowSuccessDialog(string text)
        {
            this.Invoke((MethodInvoker)delegate
            {
                labelText.Text = text;
                SetupImage("success");
                Show(); // 在 UI 线程中显示消息框
            });
        }

        public void ShowFailedDialog(string text)
        {
            this.Invoke((MethodInvoker)delegate
            {
                labelText.Text = text;
                SetupImage("failed");
                Show(); // 在 UI 线程中显示消息框
            });
        }

        public new void Close()
        {
            this.Invoke((MethodInvoker)delegate
            {
                this.Hide(); // 隐藏窗体而不关闭
            });
        }

        public void ReShow()
        {
            this.Invoke((MethodInvoker)delegate
            {
                this.Show(); // 再次显示窗体
            });
        }
    }
}
