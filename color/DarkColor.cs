using System.Drawing;
using System.Windows.Forms;

namespace ScreenCaptureOCR.color
{
    public class DarkColor: ProfessionalColorTable
    {
        // 选中菜单项背景色
        public override Color MenuItemSelected => Color.FromArgb(63, 64, 66);

        // 菜单项选中时的边框颜色
        public override Color MenuItemBorder => Color.FromArgb(48, 48, 48);

        // 菜单项按下时的渐变开始颜色
        public override Color MenuItemPressedGradientBegin => Color.FromArgb(61, 61, 61);

        // 菜单项按下时的渐变结束颜色
        public override Color MenuItemPressedGradientEnd => Color.FromArgb(61, 61, 61);

        // 选中菜单项的渐变开始颜色
        public override Color MenuItemSelectedGradientBegin => Color.FromArgb(51, 51, 51);

        // 选中菜单项的渐变结束颜色
        public override Color MenuItemSelectedGradientEnd => Color.FromArgb(51, 51, 51);

        // 菜单项按下时的渐变中间颜色
        public override Color MenuItemPressedGradientMiddle => Color.FromArgb(61, 61, 61);

    }
}
