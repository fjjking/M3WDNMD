using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Interop;

namespace M3WDNMD
{
    [StructLayout(LayoutKind.Sequential)]
    public class KeyBoardHookStruct
    {
        public int vkCode;
        public int scanCode;
        public int flags;
        public int time;
        public int dwExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    public class POINT
    {
        public int x;
        public int y;
    }

    [StructLayout(LayoutKind.Sequential)]
    public class MouseHookStruct
    {
        public POINT pt;
        public int mouseData;
        public int flags;
        public int time;
        public int dwExtraInfo;
    }
    public static class Hook
    {
        //记录Hook编号
        public static IntPtr Next_KeyHookPtr;
        public static IntPtr Next_MouseHookPtr;

        //常量
        private const int WM_MOUSEMOVE = 0x0200;
        private const int WM_LBUTTONDOWN = 0x0201;
        private const int WM_RBUTTONDOWN = 0x0204;
        private const int WM_LBUTTONUP = 0x0202;
        private const int WM_RBUTTONUP = 0x0205;
        private const int KEYDOWN = 0x0100;
        private const int KEYUP = 0x0101;
        private static int HalfWinWidth;
        public static int[] V_Code = { 0x53, 0x44, 0x46, 0x41, 0x51, 0x61, 0x62, 0x63, 0x66, 0x50, 0x7A, 0x7B };

        //记录摇杆、按钮、测试键值
        public static short Lever = 0;
        public static byte[] Buttons = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public static byte OptButtons = 0;


        public delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);

        /*    LButton1 = 0
              LButton2 = 1
              LButton3 = 2
              LSide = 3
              LMenu = 4
              RButton1 = 5
              RButton2 = 6
              RButton3 = 7
              RSide = 8
              RMenu = 9
        */
        public static byte LeftButton =>
            (byte)(Buttons[0] << 0
                    | Buttons[1] << 1
                    | Buttons[2] << 2
                    | Buttons[3] << 3
                    | Buttons[4] << 4);

        public static byte RightButton =>
            (byte)(Buttons[5] << 0
                    | Buttons[6] << 1
                    | Buttons[7] << 2
                    | Buttons[8] << 3
                    | Buttons[9] << 4);

        //获取句柄
        [DllImport("Kernel32.dll")]
        internal extern static IntPtr GetModuleHandle(String name);

        [DllImport("User32.dll")]
        public static extern bool PeekMessageA(out MSG msg, int hWnd, uint wFilterMin, uint wFilterMax, uint wFlag);

        //注册钩子
        [DllImport("User32.dll")]
        internal extern static IntPtr SetWindowsHookEx(int idHook, HookProc Ipfn, IntPtr hanstance, int threadID);

        //获取下一个钩子
        [DllImport("User32.dll")]
        internal extern static IntPtr CallNextHookEx(IntPtr handle, int code, IntPtr wparam, IntPtr Iparam);

        //设置鼠标位置
        [DllImport("user32.dll")]
        internal static extern int SetCursorPos(int x, int y);

        //键盘回调
        public static IntPtr KeyHook(int Code, IntPtr wParam, IntPtr IParam)
        {
            if (Code >= 0)
            {
                int W_Param = wParam.ToInt32();
                if (W_Param == KEYDOWN)
                {
                    KeyBoardHookStruct kbh = (KeyBoardHookStruct)Marshal.PtrToStructure(IParam, typeof(KeyBoardHookStruct));
                    int vcode = kbh.vkCode;
                    for (int i = 0; i < V_Code.Length; i++)
                    {
                        if (vcode == V_Code[i])
                        {
                            if (i == 10) OptButtons = 0b01;
                            else if (i == 11) OptButtons = 0b10;
                            else Buttons[i] = 1;
                        }
                    }
                }
                else if (W_Param == KEYUP)
                {
                    KeyBoardHookStruct kbh = (KeyBoardHookStruct)Marshal.PtrToStructure(IParam, typeof(KeyBoardHookStruct));
                    int vcode = kbh.vkCode;
                    for (int i = 0; i < V_Code.Length; i++)
                    {
                        if (vcode == V_Code[i])
                        {
                            if (i > 9) OptButtons = 0;
                            else Buttons[i] = 0;
                        }
                    }
                }
            }
            return CallNextHookEx(Next_KeyHookPtr, Code, wParam, IParam);
        }

        //鼠标回调
        public static IntPtr MouseHook(int Code, IntPtr wParam, IntPtr IParam)
        {
            if (Code >= 0)
            {
                int W_Param = wParam.ToInt32();
                switch (W_Param)
                {
                    case WM_MOUSEMOVE:
                        MouseHookStruct mh = (MouseHookStruct)Marshal.PtrToStructure(IParam, typeof(MouseHookStruct));
                        int x = mh.pt.x;
                        Lever = (short)((float)(x - HalfWinWidth + 20) / (float)HalfWinWidth * 32766);
                        break;
                    case WM_LBUTTONDOWN: Buttons[3] = 1; break;
                    case WM_LBUTTONUP: Buttons[3] = 0; break;
                    case WM_RBUTTONDOWN: Buttons[8] = 1; break;
                    case WM_RBUTTONUP: Buttons[8] = 0; break;
                    default: return CallNextHookEx(Next_MouseHookPtr, Code, wParam, IParam);
                }
            }
            return CallNextHookEx(Next_MouseHookPtr, Code, wParam, IParam);
        }


        public static void SetHook()
        {
            HookProc KeyHookProc = new HookProc(KeyHook);
            Next_KeyHookPtr = SetWindowsHookEx(13, KeyHookProc, GetModuleHandle("M3WDNMD.dll"), 0);
            HookProc MouseHookProc = new HookProc(MouseHook);
            Next_KeyHookPtr = SetWindowsHookEx(14, MouseHookProc, GetModuleHandle("M3WDNMD.dll"), 0);
            HalfWinWidth = (Screen.PrimaryScreen.WorkingArea.Width) / 2;
        }
    }


}
/*KeyBoardHookStruct kbh = (KeyBoardHookStruct)Marshal.PtrToStructure(IParam, typeof(KeyBoardHookStruct));
Keys k = (Keys)Enum.Parse(typeof(Keys), kbh.vkCode.ToString());
Console.WriteLine(k);
if (k == Keys.B)
{
    return (IntPtr)1;
}*/