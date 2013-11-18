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
        public void ChangeWorkingDirectory(string path) {
            if (Directory.Exists(path)) Environment.CurrentDirectory = path;
        }

        /// <summary>
        /// Gets the current working directory
        /// </summary>
        public string WorkingDirectory {
            get { return Environment.CurrentDirectory; }
        }

        /// <summary>
        /// Gets the absolute path for a file or directory
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string Absolute(string path)
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
        public string[] List(string path)
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
        public long Size(string path)
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
        public bool Exists(string path) {
            return IsFile(path) || IsDirectory(path);
        }

        /// <summary>
        /// Returns true if path is a file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool IsFile(string path) {
            return File.Exists(path);
        }

        /// <summary>
        /// Returns true if path is a directory
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool IsDirectory(string path) {
            return Directory.Exists(path);
        }

        /// <summary>
        /// Reads the contents of a file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string Read(string path)
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
        public void Remove(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

    }
}
