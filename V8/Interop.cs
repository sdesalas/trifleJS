using System;

namespace TrifleJS.V8
{
    /// <summary>
    /// Defines a set of C# mid-tier classes that can be instantiated inside the javascript engine.
    /// </summary>
    public class Interop
    {
        public V8.Module.WebPage WebPage()
        {
            return new V8.Module.WebPage();
        }
        public V8.Module.FileSystem FileSystem()
        {
            return new V8.Module.FileSystem();
        }
        public V8.Module.System System()
        {
            return new V8.Module.System();
        }
    }
}
