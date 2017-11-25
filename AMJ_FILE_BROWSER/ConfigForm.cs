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
        private AmjFolder folder;
        public ConfigForm(AmjFolder folder)
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
            List<String> attrList = new List<string>();
            foreach (ListViewItem attrItem in lstAttr.Items)
                if (attrItem.Text.Length > 0) attrList.Add(attrItem.Text);

            this.folder.updateAttributes(attrList);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void populateFolderData()
        {
            lstAttr.Items.Clear();
            foreach (String attr in this.folder.attributesList)
                lstAttr.Items.Add(attr);
            txtName.Text = this.folder.name;
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
