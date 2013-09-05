using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrifleJS
{
    public class Module
    {
        public string name;

        public Module(string name) {
            this.name = name;
        }

        public object create() {
            switch (this.name)
            {
                case "webpage":
                    return new WebPage();
                    break;
                case "fs":
                    return new FileSystem();
                    break;
                default:
                    throw new Exception(String.Format("Unknown module [{0}]", name));
            }
        }
    }
}
