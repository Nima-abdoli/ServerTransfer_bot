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
        private bool isLinux;
        private string DefultPath;
        public string CurrentPath { get; set; }

        #region Class Constructor
        public FileExplorer()
        {
            CurrentPath = SetCurretnPath(); 
        }
        #endregion

        #region Os Check
        /// <summary>
        /// Check os Type for system file path
        /// </summary>
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
        }// end of OsCheck
        #endregion

        #region Defult path 

        /// <summary>
        /// Check To make sure Defult path file that save user choosed defult path exist or not
        /// </summary>
        bool DefultPathFileCheck()
        {
            if (File.Exists("DefultPath.txt"))
            {
                Console.WriteLine("Defult path holder File Exist");
                return true;
            }
            else
            {
                Console.WriteLine("Defult path holder File don't Exist");
                return false;
            }
        }//end of Defult Path File Check


        string SetCurretnPath()
        {
            if (DefultPathFileCheck())
            {
                return "";
            }
            else
            {
                return OsDefultPath();
            }
        }

        /// <summary>
        /// return path for specified os that this program run in it. this will be called when user don't set defult path.
        /// </summary>
        /// <returns>path of specified by running os </returns>
        string OsDefultPath()
        {
            if (isLinux)
            {
                return "/home";
            }
            else
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }
        }//end of OsDefultPath

        #endregion

        public string[] FilesInPath()
        {
            return new string[] { "file1", "file2", "file3", "file4", "Folder5", };
        }

        #region File List

        static void FileLookup()
        {
            if (Directory.Exists(@"D:\Developing\C#\Under Developing\ServerTransfer_bot"))
            {

                string[] subdir = Directory.GetDirectories(@"D:\Developing\C#\Under Developing\ServerTransfer_bot");
                foreach (var item in subdir)
                {
                    Console.WriteLine("#" + Path.GetFileName(item));
                    Console.WriteLine("@" + item);
                }

                Console.WriteLine("--- Files---");
                string[] files = Directory.GetFiles(@"D:\Developing\C#\Under Developing\ServerTransfer_bot");

                foreach (var item in files)
                {

                    Console.WriteLine(Path.GetFileName(item));
                }
            }
        }// End of File LookUp

        #endregion


    }
}
