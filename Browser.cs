using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;


namespace TrifleJS
{
    public class Browser : System.Windows.Forms.WebBrowser
    {
        public void Render(string fileName) {
            this.DocumentCompleted += delegate
            {
                Console.WriteLine("WebBrowser#DocumentCompleted");
                using (var pic = new Bitmap(this.Width, this.Height))
                {
                    this.DrawToBitmap(pic, new Rectangle(0, 0, pic.Width, pic.Height));
                    pic.Save(fileName);
                }
            };
        }
    }
}
