using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AMJ_FILE_BROWSER.Data;

namespace AMJ_FILE_BROWSER
{
    public partial class FileEditor : Form
    {
        AMJ_File f;
        AMJ_Folder folder;
        public FileEditor(AMJ_File f, AMJ_Folder folder)
        {
            InitializeComponent();
            this.f = f;
            this.folder = folder;
            foreach (String attr in folder.getAttributes())
                textBox1.Text += attr + ":" + f.getAttribute(attr) + Environment.NewLine;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach (String line in textBox1.Lines)
            {
                if (!line.Contains(":")) continue;
                String attr = line.Split(':')[0];
                String attrVal = line.Split(':')[1];
                if (attrVal.Length > 0) folder.addNewFileAttr(f, attr, attrVal);
            }
            folder.setupFolder(folder.folderPath);
            this.Close();
        }
    }
}
