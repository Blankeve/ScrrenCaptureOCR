using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScreenCaptureOCR.forms
{
    public partial class LogForm : Form
    {
        private static LogForm instance; // 静态变量保存唯一实例
        private TextBox logTextBox;

        // 获取唯一实例的静态方法
        public static LogForm GetInstance()
        {
            if (instance == null || instance.IsDisposed)
            {
                instance = new LogForm();
                instance.CreateHandle(); // 立即创建窗体句柄
            }
            return instance;
        }

        // 公有静态方法直接添加日志
        public static async Task AppendLog(string log)
        {
            await Task.Run(() =>
            {
                LogForm logForm = GetInstance();
                logForm.Logger(log);
            });
        }


        private LogForm()
        {
            InitializeComponent();
            InitializeLogForm();
            // 订阅窗体关闭事件
            this.FormClosing += LogForm_FormClosing;
        }

        private void InitializeLogForm()
        {
            this.Icon = Properties.Resources.log;
            this.Text = "Debug Log";
            logTextBox = new TextBox();
            logTextBox.Multiline = true;
            logTextBox.ReadOnly = true;
            logTextBox.ScrollBars = ScrollBars.Vertical;
            logTextBox.Dock = DockStyle.Fill;
            // 设置窗体背景为黑色
            this.BackColor = Color.Black;
            // 设置文本框背景为黑色
            logTextBox.BackColor = Color.Black;
            // 设置文本框前景（文本）颜色为白色
            logTextBox.ForeColor = Color.White;
            Controls.Add(logTextBox);
        }

        // Method to append logs to the logTextBox
        public void Logger(string log)
        {
            if (logTextBox.InvokeRequired)
            {
                if (IsHandleCreated && !IsDisposed)
                {
                    logTextBox.Invoke(new Action(() =>
                    {
                        logTextBox.AppendText(log + Environment.NewLine);
                    }));
                }
            }
            else
            {
                logTextBox.AppendText(log + Environment.NewLine);
            }
        }


        private void LogForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true; // 取消关闭操作
            this.Hide(); // 隐藏窗体而不是关闭它
        }

        public void ShowAndActivate()
        {
            // 在UI线程上调度操作
            if (instance == null || instance.IsDisposed)
            {
                // 如果句柄未创建，则等待句柄创建后再显示和激活窗体
                HandleCreated += (sender, e) => ShowAndActivate();
            }
            else
            {
                Invoke(new Action(() =>
                {
                    // 显示日志窗体并激活
                    Show();
                    Activate();
                }));
            }
        }


        public static void ShowLogForm()
        {
            // 获取日志窗体实例并显示激活
            GetInstance().ShowAndActivate();
        }


        private void LogForm_Load(object sender, EventArgs e)
        {

        }
    }
}
