using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace M3WDNMD
{
    public static class KeyConfig
    { 
        private static readonly string[] Key_Name = { "LEFT_A" , "LEFT_B", "LEFT_C",  "LEFT_SIDE", "LEFT_MENU", "RIGHT_A", "RIGHT_B", "RIGHT_C" , "RIGHT_SIDE" , "RIGHT_MENU" , "TEST", "SERVICE" };
        public static void Read()
        {
            var location = typeof(MU3IO).Assembly.Location;
            string directoryName = Path.GetDirectoryName(location);
            string segatoolsIniPath = Path.Combine(directoryName, "Keyboard.ini");
            if (File.Exists(segatoolsIniPath))
            {
                Console.WriteLine(segatoolsIniPath);
                StringBuilder sb = new StringBuilder(250);
                for (int i = 0; i < Key_Name.Length; i++)
                {
                    GetPrivateProfileString("dinput", Key_Name[i],null,sb, 250, segatoolsIniPath);
                    try
                    {
                        int code = Convert.ToInt32(sb.ToString(), 16);
                        Hook.V_Code[i] = code;
                    }
                    catch (Exception ex) {
                        Console.WriteLine("Chank your Keyboard.ini");
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }
        [DllImport("kernel32")]//返回取得字符串缓冲区的长度
        private static extern long GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

    }
}
