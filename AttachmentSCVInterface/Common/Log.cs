using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AttachmentSCVInterface.Common
{
    public static class Log
    {
        static Semaphore _sem = new Semaphore(1, 1);
        static string FileLocation = "C:\\errorlog.txt";
        public static void LoadInfo(string info)
        {
            _sem.WaitOne();
            FileStream fs = new FileStream(FileLocation, FileMode.Append);
            StreamWriter sw = new StreamWriter(fs, Encoding.Default);
            sw.Write( DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") +  "  #######  "  );
            sw.Write(info + "\r\n");
            sw.Close();
            fs.Close();
            _sem.Release();
        }
    }
}
