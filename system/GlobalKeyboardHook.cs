using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

public class GlobalKeyboardHook
{
    // 定义键盘钩子处理程序委托
    public delegate int KeyboardHookProc(int nCode, IntPtr wParam, IntPtr lParam);

    // 导入 Windows API 函数
    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook, KeyboardHookProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern int CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);

    // 键盘事件枚举
    private const int WH_KEYBOARD_LL = 13;
    private const int WM_KEYDOWN = 0x0100;

    // 定义键盘钩子句柄
    private IntPtr _hookID = IntPtr.Zero;

    // 键盘钩子处理程序
    private KeyboardHookProc _hookCallback;

    // 键按下事件
    public event KeyEventHandler KeyDown;

    // 构造函数
    public GlobalKeyboardHook()
    {
        _hookCallback = HookCallback;
        _hookID = SetHook(_hookCallback);
    }

    // 设置钩子
    private IntPtr SetHook(KeyboardHookProc proc)
    {
        using (ProcessModule curModule = Process.GetCurrentProcess().MainModule)
        {
            return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
        }
    }

    // 取消钩子
    public void Dispose()
    {
        UnhookWindowsHookEx(_hookID);
    }

    // 键盘钩子回调函数
    private int HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        MessageBox.Show("Test");
        if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
        {
            int vkCode = Marshal.ReadInt32(lParam);
            Keys key = (Keys)vkCode;

            // 触发键按下事件
            OnKeyDown(new KeyEventArgs(key));
        }
        return CallNextHookEx(_hookID, nCode, wParam, lParam);
    }

    // 触发键按下事件
    protected virtual void OnKeyDown(KeyEventArgs e)
    {
        KeyDown?.Invoke(this, e);
    }
}
