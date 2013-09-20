using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrifleJS
{
    public class Callback
    {
        public static bool execute(string id, params object[] arguments) {
            try
            {
                Program.context.Run(String.Format(@"triflejs.callbacks['{0}'].execute({1});", id, String.Join(",", parse(arguments))));
            }
            catch (Noesis.Javascript.JavascriptException ex) {
                Host.Handle(ex);
            }
            return true;
        }

        public static string[] parse(params object[] arguments) {
            List<string> input = new List<string>();
            foreach (object argument in arguments)
            {
                switch (argument.GetType().Name)
                {
                    case "Int32":
                    case "Double":
                    case "Boolean":
                        input.Add(argument.ToString());
                        break;
                    default:
                        input.Add(String.Format("\"{0}\"", argument.ToString()));
                        break;
                }
            }
            return input.ToArray();
        }
    }
}
