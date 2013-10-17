using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TrifleJS.API.Modules
{
    /// <summary>
    /// Defines a set of methods for interaction with the filesystem
    /// </summary>
    public class FileSystem
    {
        public void ChangeWorkingDirectory(string path) {
            if (Directory.Exists(path)) Environment.CurrentDirectory = path;
        }

        public string WorkingDirectory {
            get { return Environment.CurrentDirectory; }
        }
    }
}
