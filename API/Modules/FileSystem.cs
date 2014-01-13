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
        /// <summary>
        /// Changes the current working directory
        /// </summary>
        /// <param name="path"></param>
        public void changeWorkingDirectory(string path) {
            if (Directory.Exists(path)) Environment.CurrentDirectory = path;
        }

        /// <summary>
        /// Gets the current working directory
        /// </summary>
        public string workingDirectory {
            get { return Environment.CurrentDirectory; }
        }

        /// <summary>
        /// Gets the absolute path for a file or directory
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string absolute(string path)
        {
            if (Directory.Exists(path))
            {
                return new DirectoryInfo(path).FullName;
            }
            else if (File.Exists(path))
            {
                return new FileInfo(path).FullName;
            }
            return String.Empty;
        }

        /// <summary>
        /// Gets a list of files in a directory path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string[] list(string path)
        {
            if (Directory.Exists(path))
            {
                return Directory.GetFiles(path);
            }
            return new string[] { };
        }

        /// <summary>
        /// Gets the size of a file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public long size(string path)
        {
            if (File.Exists(path))
            {
                return new FileInfo(path).Length;
            }
            return -1;
        }

        /// <summary>
        /// Returns true if path exists
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool exists(string path) {
            return isFile(path) || isDirectory(path);
        }

        /// <summary>
        /// Returns true if path is a file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool isFile(string path) {
            return File.Exists(path);
        }

        /// <summary>
        /// Returns true if path is a directory
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool isDirectory(string path) {
            return Directory.Exists(path);
        }

        /// <summary>
        /// Reads the contents of a file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string read(string path)
        {
            if (File.Exists(path))
            {
                return File.ReadAllText(path);
            }
            return String.Empty;
        }

        /// <summary>
        /// Deletes a file
        /// </summary>
        /// <param name="path"></param>
        public void remove(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

    }
}
