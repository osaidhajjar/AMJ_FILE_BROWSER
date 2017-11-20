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
using System.IO;

namespace AMJ_FILE_BROWSER
{
    public partial class ConfigForm : Form
    {
        private AMJ_Folder folder;
        public ConfigForm(AMJ_Folder folder)
        {
            this.folder = folder;
            InitializeComponent();
        }

        private void ConfigForm_Load(object sender, EventArgs e)
        {
            populateFolderData();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(txtFolder.Text))
            {
                MessageBox.Show("Source path you specified doesn't exist");
                return;
            }
            //folder.setupFolder(txtFolder.Text);

            List<String> attrList = new List<string>();
            foreach (ListViewItem attrItem in lstAttr.Items)
                if (attrItem.Text.Length > 0) attrList.Add(attrItem.Text);

            this.folder.addAttributesList(attrList);
            folder.setupFolder(txtFolder.Text);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (this.folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                txtFolder.Text = this.folderBrowserDialog1.SelectedPath;
                folder.setupFolder(txtFolder.Text);
                populateFolderData();
            }
        }

        private void txtFolder_Leave(object sender, EventArgs e)
        {
            if (txtFolder.Text .Length > 0 && Directory.Exists(this.txtFolder.Text))
            {
                folder.setupFolder(txtFolder.Text);
                populateFolderData();
            }
        }

        private void populateFolderData()
        {
            lstAttr.Items.Clear();
            if (folder.folderPath.Length > 0)
                foreach (String attr in this.folder.getAttributes())
                    lstAttr.Items.Add(attr);
            txtFolder.Text = this.folder.folderPath;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            ListViewItem item = lstAttr.Items.Add(String.Empty);
            item.BeginEdit();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (lstAttr.SelectedItems.Count > 0)
                lstAttr.SelectedItems[0].BeginEdit();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (lstAttr.SelectedItems.Count > 0)
                lstAttr.Items.Remove(lstAttr.SelectedItems[0]);
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            if (lstAttr.SelectedItems.Count > 1)
                if (lstAttr.SelectedItems[0].Index > 0)
                {
                    ListViewItem item = lstAttr.SelectedItems[0];
                    int index = lstAttr.SelectedItems[0].Index;
                    lstAttr.Items.Remove(lstAttr.SelectedItems[0]);
                    lstAttr.Items.Insert(index-1, item);
                }
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            if (lstAttr.SelectedItems.Count > 1)
                if (lstAttr.SelectedItems[0].Index < lstAttr.SelectedItems.Count-1)
                {
                    ListViewItem item = lstAttr.SelectedItems[0];
                    int index = lstAttr.SelectedItems[0].Index;
                    lstAttr.Items.Remove(lstAttr.SelectedItems[0]);
                    lstAttr.Items.Insert(index + 1, item);
                }
        }
    }
}
