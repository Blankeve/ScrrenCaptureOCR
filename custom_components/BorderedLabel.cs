using System.Drawing;
using System.Windows.Forms;

public class BorderedLabel : UserControl
{
    private Label label;

    public BorderedLabel()
    {
        // 创建 Label 控件
        label = new Label();
        label.Dock = DockStyle.Fill; // 填充父容器
        label.BackColor = Color.Transparent; // 背景透明
        label.ForeColor = Color.Black; // 设置文字颜色
        label.Font = new Font("Segoe UI", 10); // 设置字体
        label.TextAlign = ContentAlignment.TopLeft; // 文字左上对齐
        this.Controls.Add(label); // 将 Label 添加到控件中
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        // 创建一个更粗的画笔
        Pen borderPen = new Pen(Color.White, 2);

        // 绘制边框
        e.Graphics.DrawRectangle(borderPen, 0, 0, this.Width - 1, this.Height - 1);

        // 释放资源
        borderPen.Dispose();
    }


    // 添加 Text 属性来设置文字内容
    public string Text
    {
        get { return label.Text; }
        set { label.Text = value; }
    }

    // 添加 TextAlign 属性来设置文字对齐方式
    public ContentAlignment TextAlign
    {
        get { return label.TextAlign; }
        set { label.TextAlign = value; }
    }
}
