using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AttachmentSCVInterface.Common;
using AttachmentSCVInterface.Timer;

namespace AttachmentSCVInterface
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "AttachmentSCVInterface";
            DisableCloseButton("AttachmentSCVInterface");
            IntPtr ParenthWnd = new IntPtr(0);
            IntPtr et = new IntPtr(0);
            ParenthWnd = FindWindow(null, "AttachmentSCVInterface");
            ShowWindow(ParenthWnd, 2);  //隐藏本dos窗体, 0: 后台执行；1:正常启动；2:最小化到任务栏；3:最大化

            Console.WriteLine("------------------------------");
            Console.WriteLine("程序已启动，请不要关闭！");
            Console.WriteLine("退出程序请输入'exit'后按回车");
            Console.WriteLine("------------------------------");
            Log.LoadInfo("程序启动");
            PVTimer p = new PVTimer();
            p.StartPVTimer();
            var key = Console.ReadLine();
            while (key != "exit")
            {
                key = Console.ReadLine();
            }
            Log.LoadInfo("程序退出");
            Console.WriteLine("程序退出");
        }

        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", EntryPoint = "FindWindowEx")]   //找子窗体   
        static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);
        [DllImport("User32.dll", EntryPoint = "SendMessage")]   //用于发送信息给窗体   
        static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, string lParam);
        [DllImport("User32.dll", EntryPoint = "ShowWindow")]   //
        private static extern bool ShowWindow(IntPtr hWnd, int type);
        [DllImport("user32.dll", EntryPoint = "GetSystemMenu")]
        static extern IntPtr GetSystemMenu(IntPtr hWnd, IntPtr bRevert);
        [DllImport("user32.dll", EntryPoint = "RemoveMenu")]
        static extern IntPtr RemoveMenu(IntPtr hMenu, uint uPosition, uint uFlags);

        static void DisableCloseButton(string title)
        {
            //线程睡眠，确保closebtn中能够正常FindWindow，否则有时会Find失败。。
            Thread.Sleep(100);
            IntPtr windowHandle = FindWindow(null, title);
            IntPtr closeMenu = GetSystemMenu(windowHandle, IntPtr.Zero);
            uint SC_CLOSE = 0xF060;
            RemoveMenu(closeMenu, SC_CLOSE, 0x0);
        }
    }
}

