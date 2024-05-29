using Microsoft.Win32;
using System;
using System.IO;
using System.Reflection;

namespace ScreenCaptureOCR.system
{
    internal static class Regedit
    {
        public static void RegisterStartup(string appName)
        {
            try
            {
                // 获取当前应用程序的目录
                string appDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                // 获取应用程序的路径
                string appPath = Path.Combine(appDirectory, appName + ".exe"); // 使用应用程序名称构造路径

                // 打开注册表项
                RegistryKey regKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

                // 创建启动项
                regKey.SetValue(appName, appPath);

                // 关闭注册表项
                regKey.Close();

            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("添加程序到启动项时出错：" + ex.Message);
            }
        }

        public static void UnregisterStartup(string appName)
        {
            try
            {
                // 打开注册表项
                RegistryKey regKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

                // 删除启动项
                regKey.DeleteValue(appName, false);

                // 关闭注册表项
                regKey.Close();

            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("从启动项中移除程序时出错：" + ex.Message);
            }
        }
    }
}
