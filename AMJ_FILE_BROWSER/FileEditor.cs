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
        AmjFile f;
        public FileEditor(AmjFile f)
        {
            InitializeComponent();
            this.f = f;
            foreach (String attr in f.parent.attributesList)
                textBox1.Text += attr + ":" + f.getAttribute(attr) + Environment.NewLine;

            this.txtName.Text = f.name;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            f.rename(txtName.Text);
            foreach (String line in textBox1.Lines)
            {
                if (!line.Contains(":")) continue;
                String attr = line.Split(':')[0];
                String attrVal = line.Split(':')[1];
                if (attrVal.Length > 0)
                {
                    f.addAttribute(attr, attrVal);
                }
            }
            f.parent.updateFile(f);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
