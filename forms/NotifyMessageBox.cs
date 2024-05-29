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
        private static NotifyMessageBox instance;
        private PictureBox pictureBoxStatus;
        private Label labelText = new Label();
        private Button closeButton = new Button();
        private System.Windows.Forms.Timer timer;
        private int targetY;
        private const int AnimationDuration = 1000; // 动画持续时间（毫秒）
        private const int AnimationSteps = 50; // 动画步数
        private int stepCount;

        private NotifyMessageBox()
        {
            InitializeComponent();
            SetupMessageBox();
            SetupAnimationTimer();
            Opacity = 0; // 设置窗体初始不透明度为0
        }

        public static NotifyMessageBox Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new NotifyMessageBox();
                }
                return instance;
            }
        }

        public static void ShowDialog(string text)
        {
            Instance?.DisplayMessageBox(text, "info");
        }

        public static void ShowSuccessDialog(string text)
        {
            Instance?.DisplayMessageBox(text, "success");
        }

        public static void ShowFailedDialog(string text)
        {
            Instance?.DisplayMessageBox(text, "failed");
        }

        private void DisplayMessageBox(string text, string status)
        {
            labelText.Text = text;
            SetupImage(status);
            ShowMessageBox();
        }

        private void SetupAnimationTimer()
        {
            timer = new Timer();
            timer.Interval = AnimationDuration / AnimationSteps;
            timer.Tick += Timer_Tick;
            FormClosed += (sender, e) => timer.Stop(); // 停止计时器
        }

        private async void Timer_Tick(object sender, EventArgs e)
        {
            if (IsDisposed || Disposing) // 检查窗体是否已释放
            {
                timer.Stop(); // 停止计时器
                return;
            }

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
                CloseMessageBox(); // 在动画完成后关闭通知框
            }
        }

        private void CloseMessageBox()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(CloseMessageBox));
                return;
            }

            Hide(); // 隐藏通知框而不是关闭它
        }


        private void SetupImage(string status)
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
            pictureBoxStatus.Location = new Point(10, (Height - pictureBoxStatus.Height) / 2);
            labelText.Location = new Point(pictureBoxStatus.Right + 10, (Height - labelText.Height) / 2);
            closeButton.Location = new Point(Width - closeButton.Width - 10, (Height - closeButton.Height) / 2);
        }

        private void ShowMessageBox()
        {
            // 设置消息框位置为屏幕右上角
            int padding = 100; // 右上角距离屏幕边缘的距离
            int x = Screen.PrimaryScreen.WorkingArea.Right - Width - padding;
            int y = padding;
            Location = new Point(x, y);
            targetY = padding;
            stepCount = 0;
            timer.Start();
            Show(); // 调用基类的Show方法显示窗体
            Owner?.Focus();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            instance = null;
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
            this.Size = new Size(250, pictureBoxStatus.Height);

            // 设置消息框位置为屏幕右上角
            int padding = 10; // 右上角距离屏幕边缘的距离
            int x = Screen.PrimaryScreen.WorkingArea.Right - this.Width - padding;
            int y = padding;
            this.Location = new Point(x, y);

            pictureBoxStatus.SizeMode = PictureBoxSizeMode.AutoSize;
            // 设置 labelText 的文字颜色为白色
            labelText.ForeColor = Color.DarkCyan;

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
        }

        private void NotifyMessageBox_Load(object sender, EventArgs e)
        {

        }
    }
}
