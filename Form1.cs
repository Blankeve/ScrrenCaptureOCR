using OpenCvSharp;
using Sdcb.PaddleInference;
using Sdcb.PaddleOCR.Models.Local;
using Sdcb.PaddleOCR.Models;
using Sdcb.PaddleOCR;
using SimpleOCR;
using System;
using System.Drawing;
using System.Windows.Forms;
using WindowsFormsApp1.cs;
using ScreenCaptureOCR.color;
using System.Configuration;
using ScreenCaptureOCR.forms;
using System.Threading.Tasks;
using System.Reflection;
using System.Media;
using ScreenCaptureOCR.system;
using ScreenCaptureOCR.app;

namespace ScreenCaptureOCR
{
    public partial class FormMain : Form
    {
        private TableLayoutPanel tableLayoutPanelMain = new TableLayoutPanel();
        private TableLayoutPanel tableLayoutPanelButtons;
        private PictureBox pictureBoxDisplay;
        private Label labelDisplay;
        private Panel pannelOperation;
        private Button btnLoadImage;
        private Button btnScreenCapture;
        private string filepath = "";
        PictureBox appIcon = new PictureBox();
        public System.Windows.Forms.NotifyIcon notifyIconMain;
        public bool autoStartMenuItemChecked;
        private string appTitle = "截屏识字";
        public ContextMenuStrip contextMenuStrip;
        private bool isFormVisible = false;
        private LoadingMessageBox messageBox;
        private NotifyMessageBox notify;
        private SoundPlayer soundPlayer;
        private LogForm logForm = LogForm.GetInstance();
        private SupportForm supportForm;
        private EditForm editForm;
        private GlobalKeyboardHook keyboardHook;

        public FormMain()
        {
            InitializeComponent();
            InitializeFormMain();
            InitializeLayout();
            InitializeNotifyIcon();
            SetDoubleBuffered(this);
        }

        private void SetDoubleBuffered(Control control)
        {
            if (SystemInformation.TerminalServerSession)
                return;

            PropertyInfo aProp = typeof(Control).GetProperty("DoubleBuffered",
                BindingFlags.NonPublic | BindingFlags.Instance);

            aProp.SetValue(control, true, null);

            foreach (Control subControl in control.Controls)
            {
                SetDoubleBuffered(subControl);
            }
        }


        private void InitializeNotifyIcon()
        {
            // 设置托盘图标的文本
            notifyIconMain.Text = appTitle;
            notifyIconMain.Visible = true;

            // 初始化ContextMenuStrip
            contextMenuStrip = new ContextMenuStrip();

            // 自定义菜单项外观
            ToolStripMenuItem openMenuItem = new ToolStripMenuItem("显示");
            openMenuItem.Font = new Font("Segoe UI", 10);
            openMenuItem.BackColor = Color.FromArgb(0xFF, 0x29, 0x2A, 0x2D); // 设置背景色为指定的 ARGB 颜色值
            openMenuItem.ForeColor = Color.White;
            openMenuItem.Click += OpenMenuItem_Click;

            ToolStripMenuItem autoStartMenuItem = new ToolStripMenuItem("开机启动");
            autoStartMenuItem.Font = new Font("Segoe UI", 10);
            autoStartMenuItem.BackColor = Color.FromArgb(0xFF, 0x29, 0x2A, 0x2D); // 设置背景色为指定的 ARGB 颜色值
            autoStartMenuItem.ForeColor = Color.White;
            autoStartMenuItem.Checked = autoStartMenuItemChecked;
            autoStartMenuItem.Click += autoStartMenuItem_click;


            ToolStripMenuItem exitMenuItem = new ToolStripMenuItem("退出");
            exitMenuItem.Font = new Font("Segoe UI", 10);
            exitMenuItem.BackColor = Color.FromArgb(0xFF, 0x29, 0x2A, 0x2D); // 设置背景色为指定的 ARGB 颜色值
            exitMenuItem.ForeColor = Color.White;
            exitMenuItem.Click += ExitMenuItem_Click;

            ToolStripMenuItem supportMenuItem = new ToolStripMenuItem("支持作者");
            supportMenuItem.Font = new Font("Segoe UI", 10);
            supportMenuItem.BackColor = Color.FromArgb(0xFF, 0x29, 0x2A, 0x2D); // 设置背景色为指定的 ARGB 颜色值
            supportMenuItem.ForeColor = Color.White;
            supportMenuItem.Click += SupportMenuItem_Click;

            contextMenuStrip.Items.Add(supportMenuItem);
            contextMenuStrip.Items.Add(autoStartMenuItem);
            contextMenuStrip.Items.Add(openMenuItem);
            contextMenuStrip.Items.Add(exitMenuItem);


            // 将ContextMenuStrip关联到NotifyIcon
            notifyIconMain.ContextMenuStrip = contextMenuStrip;
            // 添加通知图标单击事件处理程序
            notifyIconMain.MouseClick += notifyIconMain_MouseClick;

            // 设置ContextMenuStrip的属性以使其更美观
            contextMenuStrip.Renderer = new ToolStripProfessionalRenderer(new DarkColor());
            contextMenuStrip.Font = new Font("Segoe UI", 10);
            contextMenuStrip.BackColor = Color.White;
            contextMenuStrip.ForeColor = Color.Black;
        }

        private void notifyIconMain_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (!isFormVisible)
                {
                    // 显示窗体并带有动画效果
                    Animation.AnimateWindow(this.Handle, 200, Animation.AW_SLIDE | Animation.AW_VER_NEGATIVE | Animation.AW_ACTIVATE);
                    this.Show();
                    this.WindowState = FormWindowState.Normal;
                    this.Activate(); // 确保窗体被激活
                }
                else
                {
                    // 隐藏窗体并带有动画效果
                    Animation.AnimateWindow(this.Handle, 200, Animation.AW_SLIDE | Animation.AW_VER_NEGATIVE | Animation.AW_ACTIVATE | 0x00010000);
                    this.Hide();
                }
            }
        }
        private void frmMain_VisibleChanged(object sender, EventArgs e)
        {
            // 更新窗体可见状态
            isFormVisible = this.Visible && this.WindowState != FormWindowState.Minimized;
        }

        private void frmMain_SizeChanged(object sender, EventArgs e)
        {
            // 更新窗体可见状态
            isFormVisible = this.Visible && this.WindowState != FormWindowState.Minimized;
        }
        private void autoStartMenuItem_click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
            if (menuItem != null)
            {
                menuItem.Checked = !menuItem.Checked; // 切换选中状态
                autoStartMenuItemChecked = menuItem.Checked;
            }
            if (autoStartMenuItemChecked)
            {
                notify.ShowDialog("已设置开机启动");
                Regedit.RegisterStartup(appTitle);
            }
            else
            {
                notify.ShowDialog("已关闭开机启动");
                Regedit.UnregisterStartup(appTitle);
            }
            SaveAppConfig();
        }

        private void OpenMenuItem_Click(object sender, EventArgs e)
        {
            if (!isFormVisible)
            {
                // 显示窗体并带有动画效果
                Animation.AnimateWindow(this.Handle, 200, Animation.AW_SLIDE | Animation.AW_VER_NEGATIVE | Animation.AW_ACTIVATE);
                this.Show();
                this.WindowState = FormWindowState.Normal;
                this.Activate(); // 确保窗体被激活
            }
        }
        private void SupportMenuItem_Click(object sender, EventArgs e)
        {
            if (supportForm == null || supportForm.IsDisposed)
            {
                supportForm = new SupportForm(this);
            }
            supportForm.Show();
        }
        private void ExitMenuItem_Click(object sender, EventArgs e)
        {
            // 处理退出菜单项的点击事件
            closeApplication();
        }

        private void closeApplication()
        {
            SaveAppConfig();
            Application.Exit();
        }

        private void SaveAppConfig()
        {
            // 修改配置
            System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["autoStartMenuItemChecked"].Value = autoStartMenuItemChecked.ToString();
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings"); // 重新加载新的配置文件 
        }
        private void InitializeFormMain()
        {
            appTitle = AppInfo.name + AppInfo.version;
            messageBox = new LoadingMessageBox(this, "正在识别..", MessageBoxIcon.Information);
            this.ShowInTaskbar = true;
            this.Text = appTitle;
            this.Icon = Properties.Resources.appicon;
            this.MaximizeBox = false; // 取消最大化按钮
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            notifyIconMain = new NotifyIcon();
            notifyIconMain.Text = appTitle;
            notifyIconMain.Icon = Properties.Resources.appicon;
            this.StartPosition = FormStartPosition.CenterScreen;
            // 在这里添加事件绑定代码
            this.Resize += FormMain_Resize;
            // 初始化窗体事件处理
            this.VisibleChanged += frmMain_VisibleChanged;
            this.SizeChanged += frmMain_SizeChanged;
            // 订阅键盘按下事件
            this.KeyPreview = true; //确保键盘事件在窗体中被捕获
                                    // 创建并设置全局键盘钩子
            this.KeyDown += FormMain_PreviewKeyDown;
            this.FormClosing += FormMain_FormClosing;
            notify = new NotifyMessageBox(this);
            supportForm = new SupportForm(this);
            PlayStartupSound();
            LoadAppConfig();
        }

        private void LoadAppConfig()
        {
            // 读取配置
            autoStartMenuItemChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["autoStartMenuItemChecked"]);
        }

        private void PlayStartupSound()
        {
            try
            {
                // 加载音频文件
                soundPlayer = new SoundPlayer("startup.wav"); // 替换为你的音频文件路径

                // 播放音频
                soundPlayer.Play();
            }
            catch (Exception ex)
            {
                notify.ShowFailedDialog("无法播放启动音效！");
            }
        }

        public void FormMain_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // 检查是否按下了 Alt 和 C 键
            if (e.Alt && e.KeyCode == Keys.C)
            {
                // 手动调用 BtnScreenCapture_Click 事件处理程序
                BtnScreenCapture_Click(sender, e);
            }
            // 检查是否按下了 Alt 和 F 键
            if (e.Alt && e.KeyCode == Keys.F)
            {
                // 手动调用 BtnLoadImage_Click 事件处理程序
                BtnLoadImage_Click(sender, e);
            }
            // 检查是否按下了 Ctrl 和 L 键
            if (e.Control && e.KeyCode == Keys.L)
            {
                // 手动调用 ShowLogForm 方法打开日志窗体
                LogForm.ShowLogForm();
            }
        }



        // 最小化事件处理程序
        private void FormMain_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                if (isFormVisible)
                {
                    notify.ShowDialog("已最小化到右下角");
                }
                Animation.AnimateWindow(this.Handle, 500, Animation.AW_SLIDE | Animation.AW_VER_POSITIVE | Animation.AW_ACTIVATE | 0x00010000);
                this.Hide();
                notifyIconMain.Visible = true;
                // 显示托盘消息          
            }
        }


        // 关闭事件处理程序
        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveAppConfig();
            Application.Exit(); // Optionally, exit the application
        }

        private void InitializeLayout()
        {
            // 设置背景图片
            tableLayoutPanelMain.BackgroundImage = Properties.Resources.background; // 替换为你的背景图片资源

            // 设置背景图片布局方式
            tableLayoutPanelMain.BackgroundImageLayout = ImageLayout.Stretch; // 根据 tableLayoutPanelMain 的大小拉伸图片



            // 设置 tableLayoutPanelMain 的布局
            tableLayoutPanelMain.Dock = DockStyle.Fill;
            tableLayoutPanelMain.RowCount = 3;
            tableLayoutPanelMain.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanelMain.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            //tableLayoutPanelMain.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33F));

            // 修改第三行的高度百分比为 20%
            tableLayoutPanelMain.RowStyles[1] = new RowStyle(SizeType.Percent, 20F);

            pictureBoxDisplay = new PictureBox();
            pictureBoxDisplay.Dock = DockStyle.Fill;
            pictureBoxDisplay.BackColor = Color.LightGray;
            pictureBoxDisplay.BackColor = Color.Transparent; // 设置背景色为透明
            pictureBoxDisplay.Image = Properties.Resources._default; // 设置默认图片
            pictureBoxDisplay.SizeMode = PictureBoxSizeMode.StretchImage; // 设置图片显示模式为按比例拉伸
            pictureBoxDisplay.DoubleClick += PictureBoxDisplay_Click; // 添加点击事件处理程序
            ToolTip toolPictureBoxDisplay = new ToolTip();
            toolPictureBoxDisplay.SetToolTip(pictureBoxDisplay, "双击我查看大图");
            tableLayoutPanelMain.Controls.Add(pictureBoxDisplay, 0, 0);

            // 创建 labelDisplay 控件，并将其添加到 tableLayoutPanelMain 中
            labelDisplay = new Label();
            labelDisplay.Dock = DockStyle.Fill;
            labelDisplay.BackColor = Color.Transparent; // 设置背景色为透明
            labelDisplay.ForeColor = Color.Black; // 设置文字颜色
            labelDisplay.Font = new Font("Segoe UI", 10); // 设置字体
            labelDisplay.TextAlign = ContentAlignment.TopLeft; // 设置文字对齐方式为左上角
            labelDisplay.Click += LabelDisplay_Click;
            ToolTip toolTipLabelDisplay = new ToolTip();
            toolTipLabelDisplay.SetToolTip(labelDisplay, "点我进行编辑");

            //  tableLayoutPanelMain.Controls.Add(labelDisplay, 0, 1);

            pannelOperation = new Panel();
            pannelOperation.Dock = DockStyle.Fill;
            pannelOperation.BackColor = Color.LightBlue;

            // 使用嵌套的 TableLayoutPanel 来放置按钮
            tableLayoutPanelButtons = new TableLayoutPanel();
            tableLayoutPanelButtons.Dock = DockStyle.Fill;
            tableLayoutPanelButtons.ColumnCount = 2;
            tableLayoutPanelButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanelButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

            btnLoadImage = new Button();
            btnLoadImage.Dock = DockStyle.Fill;
            // 设置图片居中显示
            btnLoadImage.ImageAlign = ContentAlignment.MiddleCenter;
            // 添加图片到按钮
            btnLoadImage.Image = Properties.Resources.uploadimg; // 替换 YourImageName 为你的图片资源名称
                                                                 // 设置按钮的鼠标悬浮手势为手掌
            btnLoadImage.Cursor = Cursors.Hand;

            // 创建并设置 ToolTip 提示
            ToolTip toolTipLoadImage = new ToolTip();
            toolTipLoadImage.SetToolTip(btnLoadImage, "读取图片(alt+f)");

            btnLoadImage.Click += BtnLoadImage_Click;
            tableLayoutPanelButtons.Controls.Add(btnLoadImage, 0, 0);

            btnScreenCapture = new Button();
            btnScreenCapture.Dock = DockStyle.Fill;
            // 设置图片居中显示
            btnScreenCapture.ImageAlign = ContentAlignment.MiddleCenter;
            // 添加图片到按钮
            btnScreenCapture.Image = Properties.Resources.capture; // 替换 YourImageName 为你的图片资源名称
            btnScreenCapture.Cursor = Cursors.Hand;
            btnLoadImage.TabStop = false;
            btnScreenCapture.TabStop = false;

            // 创建并设置 ToolTip 提示
            ToolTip toolTipCapture = new ToolTip();
            toolTipCapture.SetToolTip(btnScreenCapture, "截屏取字(alt+c)");
            btnScreenCapture.Click += BtnScreenCapture_Click;
            tableLayoutPanelButtons.Controls.Add(btnScreenCapture, 1, 0);

            pannelOperation.Controls.Add(tableLayoutPanelButtons);

            // 只需将 pannelOperation 添加到 tableLayoutPanelMain 一次
            tableLayoutPanelMain.Controls.Add(pannelOperation, 0, 1);

            // 将 panelMain 添加到 FormMain 的 Controls 中
            this.Controls.Add(tableLayoutPanelMain);
        }

        private void LabelDisplay_Click(object sender, EventArgs e)
        {
            // 检查编辑框是否已经实例化，如果没有，则实例化一个新的编辑框
            if (editForm == null || editForm.IsDisposed)
            {
                editForm = new EditForm(labelDisplay.Text, this);
                editForm.StartPosition = FormStartPosition.CenterScreen;
            }

            // 显示编辑框
            editForm.Show();
        }

        private async void FormMain_Load(object sender, EventArgs e)
        {
            await Task.Delay(500); // 等待 500 毫秒
            notify.ShowDialog("欢迎使用");
        }

        private void BtnLoadImage_Click(object sender, EventArgs e)
        {
            // 显示文件选择对话框
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files (*.jpg, *.png, *.bmp)|*.jpg;*.png;*.bmp|All Files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = true
            };
            DialogResult dialogResult = openFileDialog.ShowDialog();
            if (dialogResult == DialogResult.OK)
            { // 在后台线程中显示消息框
                Task.Run(() =>
                {
                    // 创建消息框实例
                    LoadingMessageBox messageBox = null;
                    // 在UI线程上创建消息框
                    this.Invoke(new Action(() =>
                    {
                        messageBox = new LoadingMessageBox(this, "正在识别..", MessageBoxIcon.Information);
                        messageBox.Show();
                    }));

                    // 执行文本识别任务
                    filepath = openFileDialog.FileName;
                    DisplayImage(filepath);
                    RecognizeTextUsingEmbeddedPython();

                    // 在UI线程上关闭消息框
                    this.Invoke(new Action(() =>
                    {
                        messageBox.Close();
                        messageBox.Dispose(); // 释放资源
                    }));
                });
            }
            if (dialogResult == DialogResult.Cancel)
            {
                notify.ShowDialog("您已取消操作");
            }

        }

        public void BtnScreenCapture_Click(object sender, EventArgs e)
        {
            if (!(editForm == null || editForm.IsDisposed))
            {
                editForm.Hide();
            }
            this.Hide(); // Hide the current form to capture the screen
            System.Threading.Thread.Sleep(100); // Wait for the form to be hidden

            using (var screenCaptureForm = new ScreenCaptureForm())
            {
                DialogResult dialogResult = screenCaptureForm.ShowDialog();
                if (dialogResult == DialogResult.OK)
                {
                    Task.Run(() =>
                    {
                        filepath = screenCaptureForm.CapturedImagePath;

                        // Execute the image recognition on a background thread
                        RecognizeTextUsingEmbeddedPython();

                        // Update UI controls after text recognition
                        Invoke((Action)(() =>
                        {
                            DisplayImage(filepath);
                            messageBox.Close();
                            messageBox.Dispose();
                            this.Show();
                        }));
                    });

                    // Show the message box on the UI thread
                    this.Invoke(new Action(() =>
                    {
                        messageBox = new LoadingMessageBox(this, "正在识别..", MessageBoxIcon.Information);
                        messageBox.Show();
                    }));
                }
                if (dialogResult == DialogResult.Cancel)
                {
                    this.Show();
                    notify.ShowDialog("您已取消操作");
                    if (!(editForm == null || editForm.IsDisposed))
                    {
                        // 显示编辑框
                        editForm.Show();
                    }
                }

            }
        }




        private void RecognizeTextUsingEmbeddedPython()
        {
            try
            {
                LogForm.AppendLog("开始启用OCR引擎");
                string resultText = "";
                FullOcrModel model = LocalFullModels.ChineseV3;

                using (PaddleOcrAll all = new PaddleOcrAll(model, PaddleDevice.Mkldnn())
                {
                    AllowRotateDetection = true,
                    Enable180Classification = false,
                })
                {
                    LogForm.AppendLog("识别中..");
                    using (Mat src2 = Cv2.ImRead(filepath))
                    {
                        LogForm.AppendLog("正在获取结果..");
                        PaddleOcrResult result = all.Run(src2);
                        resultText = result.Text;
                    }
                }

                // Update UI controls on the UI thread
                Invoke((Action)(() =>
                {
                    labelDisplay.Text = resultText;
                    messageBox.Close();
                    if (string.IsNullOrEmpty(resultText))
                    {
                        notify.ShowFailedDialog("识别失败！");
                    }
                    else
                    {
                        notify.ShowSuccessDialog("识别成功！");
                        // 检查编辑框是否已经实例化，如果没有，则实例化一个新的编辑框
                        if (editForm == null || editForm.IsDisposed)
                        {
                            editForm = new EditForm(labelDisplay.Text, this);
                            editForm.StartPosition = FormStartPosition.CenterScreen;
                        }
                        else
                        {
                            editForm.AppendText(resultText);
                        }
                        // 显示编辑框
                        editForm.Show();
                    }
                }));
            }
            catch (Exception ex)
            {
                // 捕获到异常时，将异常信息添加到日志中
                LogForm.AppendLog($"初始化 Sdcb.PaddleInference.Native.PaddleNative 失败: {ex.Message}\n{ex.StackTrace}");
            }
        }




        private void PictureBoxDisplay_Click(object sender, EventArgs e)
        {
            if (pictureBoxDisplay.Image != null)
            {
                // 创建并显示放大图像的窗体
                ZoomedImageForm zoomedImageForm = new ZoomedImageForm(pictureBoxDisplay.Image);
                zoomedImageForm.ShowDialog();
            }
        }


        private void DisplayImage(string imagePath)
        {
            try
            {
                // 将图像加载到 PictureBox 中显示
                pictureBoxDisplay.Image = System.Drawing.Image.FromFile(imagePath);
                pictureBoxDisplay.SizeMode = PictureBoxSizeMode.Zoom;
            }
            catch (Exception ex)
            {
                MessageBox.Show("发生错误: " + ex.Message);
            }
        }


    }
}
