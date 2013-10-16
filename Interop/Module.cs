using System;

namespace TrifleJS.Interop
{
    /// <summary>
    /// Defines a set of C# mid-tier classes that can be instantiated inside the javascript engine.
    /// </summary>
    public class Module
    {
        public Modules.WebPage WebPage()
        {
            return new Modules.WebPage();
        }
        public Modules.FileSystem FileSystem()
        {
            return new Modules.FileSystem();
        }
        public Modules.System System()
        {
            return new Modules.System();
        }
    }
}
