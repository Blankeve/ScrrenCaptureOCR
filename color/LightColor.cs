using System.Drawing;
using System.Windows.Forms;

namespace ScreenCaptureOCR.color
{
    public class LightColor : ProfessionalColorTable
    {
        // 菜单背景色
        public override Color ToolStripDropDownBackground => Color.White;

        // 菜单边框颜色
        public override Color MenuBorder => Color.LightGray;

        // 选中菜单项背景色
        public override Color MenuItemSelected => Color.FromArgb(173, 216, 230);

        // 菜单项选中时的边框颜色
        public override Color MenuItemBorder => Color.LightBlue;

        // 菜单项按下时的渐变开始颜色
        public override Color MenuItemPressedGradientBegin => Color.LightSkyBlue;

        // 菜单项按下时的渐变结束颜色
        public override Color MenuItemPressedGradientEnd => Color.DeepSkyBlue;

        // 选中菜单项的渐变开始颜色
        public override Color MenuItemSelectedGradientBegin => Color.FromArgb(135, 206, 250);

        // 选中菜单项的渐变结束颜色
        public override Color MenuItemSelectedGradientEnd => Color.FromArgb(135, 206, 250);

        // 菜单项按下时的渐变中间颜色
        public override Color MenuItemPressedGradientMiddle => Color.SkyBlue;

        // 菜单背景的渐变开始颜色
        public override Color ToolStripGradientBegin => Color.White;

        // 菜单背景的渐变中间颜色
        public override Color ToolStripGradientMiddle => Color.WhiteSmoke;

        // 菜单背景的渐变结束颜色
        public override Color ToolStripGradientEnd => Color.Gainsboro;
    }
}
