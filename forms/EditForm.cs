using System.Windows.Forms;
using System;
using ScreenCaptureOCR.forms;

public partial class EditForm : Form
{
    private TextBox textBoxEdit;
    private Button buttonOK;
    private Button buttonCancel;
    public string EditedText { get; private set; }

    public EditForm(string initialText, Form parentForm)
    {
        InitializeComponent();
        textBoxEdit.Text = (initialText.Replace("\n", Environment.NewLine));
        this.MinimizeBox = false;
        this.Owner = parentForm; // 设置主窗体为当前窗体的所有者
        // 其他初始化操作...
    }

    private void InitializeComponent()
    {
        this.Icon = ScreenCaptureOCR.Properties.Resources.editor;
        this.Text = "识别结果";
        this.textBoxEdit = new System.Windows.Forms.TextBox();
        this.buttonOK = new System.Windows.Forms.Button();
        this.buttonCancel = new System.Windows.Forms.Button();
        this.SuspendLayout();
        // 
        // textBoxEdit
        // 
        this.textBoxEdit.Dock = DockStyle.Fill; // 填充整个窗体
        this.textBoxEdit.WordWrap = true; // 自动换行显示文本
        this.textBoxEdit.Location = new System.Drawing.Point(0, 0);
        this.textBoxEdit.Multiline = true;
        this.textBoxEdit.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
        this.textBoxEdit.TabIndex = 0;
        this.textBoxEdit.TextChanged += new System.EventHandler(this.textBoxEdit_TextChanged);
        // 
        // buttonOK
        // 
        this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
        this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
        this.buttonOK.Location = new System.Drawing.Point(0, 0); // 将确定按钮移动到底部
        this.buttonOK.Name = "buttonOK";
        this.buttonOK.Size = new System.Drawing.Size(75, 23);
        this.buttonOK.TabIndex = 1;
        this.buttonOK.Text = "确定";
        // 
        // buttonCancel
        // 
        this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
        this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        this.buttonCancel.Location = new System.Drawing.Point(0, 0); // 将取消按钮移动到底部
        this.buttonCancel.Name = "buttonCancel";
        this.buttonCancel.Size = new System.Drawing.Size(75, 23);
        this.buttonCancel.TabIndex = 2;
        this.buttonCancel.Text = "取消";
        // 
        // EditForm
        // 
        int screenWidth = Screen.PrimaryScreen.Bounds.Width;
        int screenHeight = Screen.PrimaryScreen.Bounds.Height;
        LogForm.AppendLog("当前计算机屏幕宽度:" + screenWidth);
        LogForm.AppendLog("当前计算机屏幕高度:" + screenHeight);
        this.ClientSize = new System.Drawing.Size(screenWidth / 3, screenHeight / 3);
        this.Controls.Add(this.textBoxEdit);
        this.Controls.Add(this.buttonOK);
        this.Controls.Add(this.buttonCancel);
        this.Name = "EditForm";
        this.Load += new System.EventHandler(this.EditForm_Load);
        this.ResumeLayout(false);
        this.PerformLayout();

    }

    public void AppendText(string text)
    {
        this.textBoxEdit.AppendText(text.Replace("\n", Environment.NewLine));
    }

    private void ButtonOK_Click(object sender, EventArgs e)
    {
        EditedText = textBoxEdit.Text;
        DialogResult = DialogResult.OK;
    }

    private void ButtonCancel_Click(object sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
    }

    private void EditForm_Load(object sender, EventArgs e)
    {
        // 将焦点移到文本框
        textBoxEdit.Focus();

        // 将光标设置到文本末尾
        textBoxEdit.SelectionStart = textBoxEdit.Text.Length;
        textBoxEdit.SelectionLength = 0;
    }

    private void textBoxEdit_TextChanged(object sender, EventArgs e)
    {

    }
}
