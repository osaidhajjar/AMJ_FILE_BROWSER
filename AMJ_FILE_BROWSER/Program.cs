using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace AMJ_FILE_BROWSER
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Browser b = null;
            
            if (args.Count() > 0)
            {
                String sourcePath = args[0];
                if (!Directory.Exists(sourcePath))
                    MessageBox.Show("Source path you specified doesnt exist.");
                else
                    b = new Browser(sourcePath);
            }

            if (b == null) b = new Browser();
            
            Application.Run(b);
        }
        
    }
}
