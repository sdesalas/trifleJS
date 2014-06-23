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
        /// Gets the file separator for windows OS
        /// </summary>
        public string separator { get { return Path.DirectorySeparatorChar.ToString(); } }

        /// <summary>
        /// Gets the absolute path for a file or directory
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string absolute(string path)
        {
            return Path.GetFullPath(path);
        }

        /// <summary>
        /// Returns true if the path is absolute
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool isAbsolute(string path) {
            return Path.IsPathRooted(path);
        }

        /// <summary>
        /// Returns true if the specified file can be executed.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool isExecutable(string path) {
            string ext = Path.GetExtension(path).ToLower();
            if (ext == "exe" || ext == "bat" || ext == "com") {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns true if the specified path is a symbolic link.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool isLink(string path)
        {
            FileAttributes attr = FileAttributes.Normal;
            if (File.Exists(path)) {
                attr = File.GetAttributes(path);
            }
            if (Directory.Exists(path)) {
                attr = new DirectoryInfo(path).Attributes;
            }
            if ((attr & FileAttributes.ReparsePoint) == FileAttributes.ReparsePoint)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns true if a file or a directory is readable.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool isReadable(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    using (FileStream fs = File.Open(path, FileMode.Open, FileAccess.Read))
                    {
                        return true;
                    }
                }
                if (Directory.Exists(path)) {
                    Directory.GetFiles(path);
                    return true;
                }
            }
            catch { }
            return false;
        }

        /// <summary>
        /// Returns true if a file or a directory is writable.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool isWritable(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    using (FileStream fs = File.Open(path, FileMode.Open, FileAccess.Write))
                    {
                        return true;
                    }
                }
                if (Directory.Exists(path))
                {
                    string file = new FileInfo(path + separator + Guid.NewGuid().ToString().Substring(0, 6)).FullName;
                    using (FileStream fs = File.Open(path, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        return true;
                    }
                }
            }
            catch { }
            return false;
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
            return File.Exists(path) || Directory.Exists(path);
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
        /// Returns a stream object representing the 
        /// stream interface to the specified file 
        /// (mode can be 'r' for read, 'w' for write, 'a' for append, 
        /// 'rb' for binary read or 'wb' for binary write).
        /// </summary>
        /// <param name="path"></param>
        /// <param name="contents"></param>
        /// <param name="opt"></param>
        public Stream open(string path, string mode) {
            return new Stream(path, mode);
        }

        /// <summary>
        /// Writes content to a file 
        /// (mode can be 'w' for write, 'a' for append or 'wb' for binary write).
        /// </summary>
        /// <param name="path"></param>
        /// <param name="content"></param>
        /// <param name="mode"></param>
        public void write(string path, string content, string mode) {
            Stream stream = new Stream(path, mode);
            stream.write(content);
            stream.close();
        }

        /// <summary>
        /// Writes text content to a file
        /// </summary>
        /// <param name="path"></param>
        /// <param name="content"></param>
        public void write(string path, string content) {
            write(path, content, "w");
        }

        /// <summary>
        /// Deletes a file
        /// </summary>
        /// <param name="path"></param>
        public void remove(string path)
        {
            File.Delete(path);
        }

        /// <summary>
        /// Copies a file to another.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        public void copy(string source, string destination) {
            File.Copy(source, destination);
        }

        /// <summary>
        /// Moves a file to another, effectively renaming it.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        public void move(string source, string destination)
        {
            File.Move(source, destination);
        }

        /// <summary>
        /// Touches a file, changing its access timestamp.
        /// </summary>
        /// <param name="path"></param>
        public void touch(string path) {
            File.AppendText(String.Empty);
        }

        /// <summary>
        /// Defines a stream class for handling files
        /// </summary>
        public class Stream {

            private FileStream file = null;
            private StreamReader reader = null;
            private StreamWriter writer = null;
            private string path;
            private string mode;

            /// <summary>
            /// Initialises a stream with a path and write mode
            /// </summary>
            /// <param name="path"></param>
            /// <param name="mode"></param>
            public Stream(string path, string mode) {
                this.path = path;
                this.mode = mode;
                switch(mode.ToLower()) {
                    case "r":
                        file = File.Open(path, FileMode.Open, FileAccess.Read);
                        reader = new StreamReader(file, Encoding.UTF8);
                        break;
                    case "w":
                        file = File.Open(path, FileMode.OpenOrCreate, FileAccess.Write);
                        writer = new StreamWriter(file, Encoding.UTF8);
                        break;
                    case "rb":
                        file = File.OpenRead(path);
                        reader = new StreamReader(file);
                        break;
                    case "wb":
                        file = File.OpenWrite(path);
                        writer = new StreamWriter(file);
                        break;
                    case "rw":
                        file = File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                        reader = new StreamReader(file, Encoding.UTF8);
                        writer = new StreamWriter(file, Encoding.UTF8);
                        break;
                    case "rwb":
                        file = File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                        reader = new StreamReader(file);
                        writer = new StreamWriter(file);
                        break;
                    case "a": 
                    case "+":
                        file = File.Open(path, FileMode.Append, FileAccess.Write);
                        writer = new StreamWriter(file, Encoding.UTF8);
                        break;
                    default:
                        throw new Exception("Unknown mode '" + mode + "' for fs.open(path, mode)");
                }
                // Appending? (ie rw+) Seek to end.
                if (mode.IndexOf("a") > 0 || mode.IndexOf("+") > 0) {
                    file.Seek(0, SeekOrigin.End);
                }
            }

            /// <summary>
            /// Returns the content of the stream.
            /// </summary>
            /// <returns></returns>
            public string read()
            {
                if (file == null || reader == null)
                {
                    throw new Exception("Cannot read from file: " + path);
                }
                if (file.CanWrite) { flush(); }
                file.Seek(0, SeekOrigin.Begin);
                return reader.ReadToEnd();
            }

            /// <summary>
            /// Reads only a line from the stream and return it.
            /// </summary>
            /// <returns></returns>
            public string readLine()
            {
                if (file == null || reader == null)
                {
                    throw new Exception("Cannot read from file: " + path);
                }
                if (file.CanWrite) { flush(); } 
                return reader.ReadLine();
            }

            /// <summary>
            /// Writes the string to the stream.
            /// </summary>
            /// <param name="data"></param>
            public void write(string data)
            {
                if (file == null || writer == null) {
                    throw new Exception("Cannot write to file: " + path);
                }
                writer.AutoFlush = true;
                writer.Write(data);
            }

            /// <summary>
            /// Writes the data as a line to the stream.
            /// </summary>
            /// <param name="data"></param>
            public void writeLine(string data)
            {
                write(data + Environment.NewLine);
            }

            /// <summary>
            /// Flushes all pending input/output.
            /// </summary>
            public void flush() 
            {
                if (file != null)
                {
                    file.Flush();
                }
            }

            /// <summary>
            /// Moves to a certain position into a stream.
            /// </summary>
            /// <param name="pos"></param>
            public void seek(long pos) 
            {
                if (file != null) file.Seek(pos, SeekOrigin.Begin);
            }

            /// <summary>
            /// Completes the stream operation.
            /// </summary>
            public void close() { 
                file.Close();
                file.Dispose();
                reader = null; 
                writer = null; 
            }

            /// <summary>
            /// Returns true if the end of the file was reached.
            /// </summary>
            /// <returns></returns>
            public bool atEnd() { 
                if (reader != null) return reader.EndOfStream; 
                return false; 
            }
        }

    }
}
