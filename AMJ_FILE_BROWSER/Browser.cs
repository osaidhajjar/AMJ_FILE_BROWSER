using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using AMJ_FILE_BROWSER.Data;

namespace AMJ_FILE_BROWSER
{
    public partial class Browser : Form
    {
        private AMJ_Folder folder;
        List<String> selectedAttr;
        int curAttrView;

        public Browser()
        {
            folder = new AMJ_Folder();
            selectedAttr = new List<String>();

            InitializeComponent();
            lblStatus.Text = "Source Folder is not specified. Go to (File -> Setup) to setup your AMJ Browser";
        }

        public Browser(String folderPath)
        {
            folder = new AMJ_Folder();
            folder.setupFolder(folderPath);
            folder.setupFolder(folderPath);
            selectedAttr = new List<String>();

            InitializeComponent();

            lblStatus.Text = "Source Folder: " + folder.folderPath;
            this.Text = "AMJ (" + folder.folderPath + ")";
            setView(0);
        }

        private void setView(int attributeIndex)
        {
            if (folder == null) return;
            if (folder.getAttributes().Count == 0)
                attributeIndex = -1;

            curAttrView = attributeIndex;
            this.listView1.Items.Clear();
            this.listView1.Columns.Clear();
            this.listView1.SmallImageList = null;
            this.listView1.Clear();
            this.listView1.View = View.Details;

            if (attributeIndex == -1)
            {
                listView1.Columns.Add("Name", 300);
                foreach (String attr in folder.getAttributes())
                    listView1.Columns.Add(attr);

                List<bool> mask = new List<bool>();
                List<String> filter = new List<string>();

                for (int i = 0; i < selectedAttr.Count; i++)
                {
                    mask.Add(true);
                    filter.Add(selectedAttr[i]);
                }

                ImageList il = new ImageList();
                listView1.SmallImageList = il;
                foreach (AMJ_File f in folder.getFiles(selectedAttr, mask))
                {
                    ListViewItem fItem = new ListViewItem(f.Name);
                    fItem.Tag = f;
                    foreach (String attr in folder.getAttributes())
                        fItem.SubItems.Add(f.getAttribute(attr));
                    il.Images.Add(f.filePath, System.Drawing.Icon.ExtractAssociatedIcon(f.filePath));
                    fItem.ImageKey = f.filePath;
                    this.listView1.Items.Add(fItem);
                }
                
            }
            else if (attributeIndex < this.folder.getAttributes().Count)
            {
                if (attributeIndex == 0)
                {
                    selectedAttr.Clear();
                    for (int i = 0; i < this.folder.getAttributes().Count; i++)
                        selectedAttr.Add("");
                }

                String attr = this.folder.getAttributes()[attributeIndex];

                this.listView1.View = View.Details;
                listView1.Columns.Add(attr, 100);
                listView1.Columns.Add("Number of Files", 100);

                ListViewItem cItem = new ListViewItem("Unknown");
                
                List<bool> mask = new List<bool>();
                List<String> filter = new List<string>();

                for (int i = 0; i < selectedAttr.Count; i++)
                {
                    mask.Add(false);
                    filter.Add("");
                }

                for (int i = 0; i <= attributeIndex; i++)
                    mask[i] = true;
                for (int i = 0; i < attributeIndex; i++)
                    filter[i] = selectedAttr[i];
                filter[attributeIndex] = "";

                int c = folder.getFiles(filter, mask).Count;
                cItem.SubItems.Add(c.ToString());
                this.listView1.Items.Add(cItem);

                foreach (String attrVal in this.folder.getAttrValues(attr))
                {
                    cItem = new ListViewItem(attrVal);
                
                    mask = new List<bool>();
                    filter = new List<string>();

                    for (int i = 0; i < selectedAttr.Count; i++)
                    {
                        mask.Add(false);
                        filter.Add("");
                    }

                    for (int i = 0; i <= attributeIndex; i++)
                        mask[i] = true;
                    for (int i = 0; i < attributeIndex; i++)
                        filter[i] = selectedAttr[i];
                    
                    filter[attributeIndex] = attrVal;

                    c = folder.getFiles(filter, mask).Count;
                    cItem.SubItems.Add(c.ToString());
                    this.listView1.Items.Add(cItem);
                }
            }

            this.txtPath.Text = "/";
            int max = curAttrView;
            if (max == -1) max = folder.getAttributes().Count;
            for (int i = 0; i < max; i++)
            {
                //String attr = folder.getAttributes()[i];
                String attrValue = selectedAttr[i] == "" ? "Unknown" : selectedAttr[i];
                this.txtPath.Text += attrValue + "/";
            }
        }


        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (curAttrView == -1)
            {
                AMJ_File f = (AMJ_File)listView1.SelectedItems[0].Tag;
                System.Diagnostics.Process.Start(f.filePath);
            }
            else
            {
                if (listView1.SelectedItems[0].Text != "Unknown")
                    selectedAttr[curAttrView] = listView1.SelectedItems[0].Text;
                else
                    selectedAttr[curAttrView] = "";

                
                if (curAttrView == folder.getAttributes().Count - 1)
                    setView(-1);
                else
                    setView(curAttrView + 1);
            }
        }

        private void listView1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Back)
            {
                if (curAttrView == -1)
                    setView(this.folder.getAttributes().Count - 1);
                else if (curAttrView == 0)
                    return;
                else
                    setView(curAttrView - 1);
            }

            else if (e.KeyCode == Keys.F2 && curAttrView == -1)
            {
                AMJ_File f = (AMJ_File)listView1.SelectedItems[0].Tag;
                FileEditor fe = new FileEditor(f, folder);
                fe.ShowDialog();
                setView(-1);
            }
        }

        private void attributesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigForm cf = new ConfigForm(folder);
            if (cf.ShowDialog() == DialogResult.OK)
            {
                setView(0);
                lblStatus.Text = "Source Folder: " + folder.folderPath;
                this.Text = "AMJ (" + folder.folderPath + ")";
            }
        }

        private void sourceFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(System.Windows.Forms.Application.MessageLoop)
                System.Windows.Forms.Application.Exit();
            else
                System.Environment.Exit(1);
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (folder.folderPath.Length > 0)
            {
                folder.setupFolder(folder.folderPath);
                setView(0);
            }
        }

    
        private void btnBack_Click(object sender, EventArgs e)
        {
            if (curAttrView == -1)
                setView(this.folder.getAttributes().Count - 1);
            else if (curAttrView == 0)
                return;
            else
                setView(curAttrView - 1);
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (listView1.FocusedItem.Bounds.Contains(e.Location) == true)
                {
                    contextMenuStrip1.Show(Cursor.Position);
                }
            }
        }

        private void menuItemOpen_Click(object sender, EventArgs e)
        {
            if (curAttrView == -1)
            {
                AMJ_File f = (AMJ_File)listView1.SelectedItems[0].Tag;
                System.Diagnostics.Process.Start(f.filePath);
            }
            else
            {
                if (listView1.SelectedItems[0].Text != "Unknown")
                    selectedAttr[curAttrView] = listView1.SelectedItems[0].Text;
                else
                    selectedAttr[curAttrView] = "";


                if (curAttrView == folder.getAttributes().Count - 1)
                    setView(-1);
                else
                    setView(curAttrView + 1);
            }
        }

        private void menuItemEdit_Click(object sender, EventArgs e)
        {
            if (curAttrView == -1)
            {
                AMJ_File f = (AMJ_File)listView1.SelectedItems[0].Tag;
                FileEditor fe = new FileEditor(f, folder);
                fe.ShowDialog();
                setView(-1);
            }
        }
    }
}
