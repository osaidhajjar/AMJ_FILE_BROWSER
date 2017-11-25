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
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            folder = Path.Combine(folder, "AMJ_FILE_BROWSER");
            Directory.CreateDirectory(folder);
            SettingsFile = Path.Combine(folder, "amj.settings");

            if (!File.Exists(SettingsFile))
                File.WriteAllText(SettingsFile, "");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Browser());
        }
        public static String SettingsFile;
        
    }
}
