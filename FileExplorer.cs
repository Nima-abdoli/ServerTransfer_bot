using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerTransfer_bot
{
    public class FileExplorer
    {
        bool isLinux;

        public void OsCheck()
        {
            if (OperatingSystem.IsLinux())
            {
                isLinux = true;
            }
            if (OperatingSystem.IsWindows())
            {
                isLinux = false;
            }
        }


    }
}
