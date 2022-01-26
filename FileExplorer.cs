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
        #region Private Property

        private bool isLinux;
        //private string DefultPath;

        #endregion

        #region Public Property

        // Current path that user start or choose to be in it.
        public string CurrentPath { get; set; }
        // Hold full path of file or folder in current given path
        public List<string> CurrentFilesPath = new List<string>();
        // Hold only name of file or folder in current given path
        public List<string> CurrentFilesName = new List<string>();

        #endregion


        #region Class Constructor

        public FileExplorer()
        {
            // set current path based on default path(Rather user set or not there is always a default path)
            CurrentPath = SetCurretnPath();
            PathLookup(CurrentPath);

        }// End of Constructor

        #endregion

        #region Os Check
        /// <summary>
        /// Check OS Type for system file path
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

        #region Default path 

        /// <summary>
        /// Check To make sure Default path file that save user chose default path exist or not
        /// </summary>
        bool DefultPathFileCheck()
        {
            if (File.Exists("DefultPath.txt"))
            {
                Console.WriteLine("Default path holder File Exist");
                return true;
            }
            else
            {
                Console.WriteLine("Default path holder File don't Exist");
                return false;
            }
        }//end of Default Path File Check

        /// <summary>
        /// Set Current path based on OS that program running on it.
        /// </summary>
        /// <returns>Default path that set by Dev.</returns>
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
        /// return path for specified OS that this program run in it. this will be called when user don't set default path.
        /// </summary>
        /// <returns>path of specified by running OS </returns>
        string OsDefultPath()
        { 
            if (isLinux)
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                //return "/home";
            }
            else
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }
        }//end of OsDefultPath

        #endregion

        #region Path lookup

        /// <summary>
        /// show list of subfiles and folders and put them in list to be accessible in other part of program.
        /// </summary>
        /// <param name="path">Path that user in it and want to see subfiles and folders</param>
        void PathLookup(string path)
        {

            if (Directory.Exists(path))
            {
                // Clear both list in case new path is given and don't append new files and folder.
                ClearCurrentLists();

                // get all of folder in given path
                string[] subdir = Directory.GetDirectories(path);
                foreach (var item in subdir)
                { 
                    CurrentFilesPath.Add(item);
                    CurrentFilesName.Add(Path.GetFileName(item));
                }

                //get all files in given path
                string[] files = Directory.GetFiles(path);
                foreach (var item in files)
                {
                    CurrentFilesPath.Add(item);
                    CurrentFilesName.Add(Path.GetFileName(item));
                }
            }
        }// End of File LookUp

        /// <summary>
        /// Clear current list in case new path is given.
        /// </summary>
        void ClearCurrentLists()
        {
            if (CurrentFilesName != null)
            {
                CurrentFilesName.Clear();
            }
            if (CurrentFilesPath != null)
            {
                CurrentFilesPath.Clear();
            }
        }//end of Clear Current Lists

        /// <summary>
        /// go back in current directory
        /// </summary>
        public void BackinPath()
        {
            CurrentPath = Directory.GetParent(CurrentPath).FullName;
        }// end of BackinPath

        #endregion


    }
}
