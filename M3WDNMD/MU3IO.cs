using System.Runtime.InteropServices;
using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading;
using System.Windows.Interop;
using System.Text;
using System.IO;

namespace M3WDNMD
{
    public static class MU3IO
    {

        [DllExport(ExportName = "mu3_io_get_api_version")]
        public static ushort GetVersion()
        {
            return 0x0102;
        }

        //初始化
        [DllExport(CallingConvention.Cdecl, ExportName = "mu3_io_init")]
        public static uint Init()
        {
            if (Process.GetCurrentProcess().ProcessName != "amdaemon" &&
                Process.GetCurrentProcess().ProcessName != "Debug" &&
                Process.GetCurrentProcess().ProcessName != "Test")
                return 0;
            KeyConfig.Read();
            Console.WriteLine("M3WDNMD: INIT");
            return 1;
        }

        //外部轮询
        [DllExport(CallingConvention.Cdecl, ExportName = "mu3_io_poll")]
        public static uint Poll()
        {
            if (Hook.Next_KeyHookPtr == (IntPtr)0)
            {
                Hook.SetHook();
                Console.WriteLine("Hook Done");
            }
            MSG msg;
            uint PM_REMOVE = 0x0001;
            Hook.PeekMessageA(out msg, 0, 0, 0, PM_REMOVE);
            return 0;
        }

        //按钮
        [DllExport(CallingConvention.Cdecl, ExportName = "mu3_io_get_gamebtns")]
        public static void GetGameButtons(out byte left, out byte right)
        {
            left = Hook.LeftButton;
            right = Hook.RightButton;
        }

        //摇杆
        [DllExport(CallingConvention.Cdecl, ExportName = "mu3_io_get_lever")]
        public static void GetLever(out short pos)
        {
            pos = Hook.Lever;
            return;
        }

        //设置LED
        [DllExport(CallingConvention.Cdecl, ExportName = "mu3_io_set_led")]
        public static void SetLed(uint data)
        {
            Console.WriteLine(data);
        }

        //测试按键
        [DllExport(CallingConvention.Cdecl, ExportName = "mu3_io_get_opbtns")]
        public static void GetOpButtons(out byte opbtn)
        {
            opbtn = Hook.OptButtons;
        }
    }
}
