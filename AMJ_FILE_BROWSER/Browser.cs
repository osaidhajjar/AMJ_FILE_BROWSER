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
        // Class Members
        List<String> selectedAttributes;
        List<String> selectedDirs;
        RegularFolder curFolder;

        // Constructors
        public Browser()
        {
            selectedAttributes = new List<String>();
            selectedDirs = new List<string>();

            InitializeComponent();

            bool initFromRecentPath = false;
            foreach (String line in File.ReadAllLines(Program.SettingsFile))
                if (line.Contains("RecentPath"))
                {
                    String recentPath = line.Split('=')[1];
                    if (recentPath[recentPath.Length - 1] == ':') recentPath += "\\";
                    if (File.Exists(recentPath + "\\amj.attrlist"))
                    {
                        AmjFolder folder = new AmjFolder(recentPath);
                        folder.loadContents();
                        populateAmjFolder(folder, 0);
                        curFolder = folder;
                    }
                    else
                    {
                        RegularFolder folder = new RegularFolder(recentPath);
                        folder.loadContents();
                        populateRegularFolder(folder);
                        curFolder = folder;
                    }
                    foreach (String dir in recentPath.Split('\\'))
                        if (dir.Length > 0)
                        {
                            if (dir.Contains(":"))
                                selectedDirs.Add(dir + "\\");
                            else
                                selectedDirs.Add(dir);
                        }
                    this.txtPath.Text = recentPath;
                    initFromRecentPath = true;
                    break;
                }

            if (!initFromRecentPath)
            {
                RegularFolder rootFolder = new RegularFolder("");
                rootFolder.loadContents();
                populateRegularFolder(rootFolder);
                curFolder = rootFolder;
            }
        }
      
        // ListView Population Functions
        private void populateRegularFolder(RegularFolder folder)
        {
            listView1.Columns.Clear();
            listView1.Items.Clear();
            ImageList il = new ImageList();
            listView1.SmallImageList = il;

            listView1.Columns.Add("Name", 300);
            listView1.Columns.Add("Type", 300);
            
            foreach (AmjObject obj in folder.contents)
            {
                ListViewItem item;
                if (obj is AmjFolder)
                {
                    AmjFolder f = (AmjFolder)obj;
                    il.Images.Add(f.name, IconReader.GetFolderIcon(IconReader.IconSize.Small, IconReader.FolderType.Open));
                    item = new ListViewItem(f.name);
                    item.ForeColor = Color.Green;
                    item.ImageKey = f.name;
                    //item.UseItemStyleForSubItems = false;
                    ListViewItem.ListViewSubItem subItem = new ListViewItem.ListViewSubItem(item, "Amj Folder");
                    //subItem.ForeColor = Color.Green;
                    item.SubItems.Add(subItem);
                }
                else if (obj is RegularFolder)
                {
                    RegularFolder f = (RegularFolder)obj;
                    il.Images.Add(f.name, IconReader.GetFolderIcon(IconReader.IconSize.Small,IconReader.FolderType.Open));
                    item = new ListViewItem(f.name);
                    item.ImageKey = f.name;
                    item.SubItems.Add("Folder");
                }
                
                else if (obj is RegularFile)
                {
                    RegularFile f = (RegularFile)obj;
                    //il.Images.Add(f.name, Icon.ExtractAssociatedIcon(f.path));
                    il.Images.Add(f.name, IconReader.GetFileIcon(f.path,IconReader.IconSize.Small,true));
                    item = new ListViewItem(f.name);
                    item.ImageKey = f.name;
                    item.SubItems.Add("File");
                }
                else
                    continue;
                item.Tag = obj;
                this.listView1.Items.Add(item);
            }
        }
        private void populateAmjFolder(AmjFolder folder, int attrIndex)
        {
            listView1.Columns.Clear();
            listView1.Items.Clear();
            ImageList il = new ImageList();
            listView1.SmallImageList = il;

            if (attrIndex == folder.attributesList.Count) // populate AmjFiles here
            {
                listView1.Columns.Add("Name", 300);
                foreach (String attr in folder.attributesList)
                    listView1.Columns.Add(attr);

                foreach (AmjObject obj in folder.getFiles(selectedAttributes))
                {
                    if (!(obj is AmjFile)) continue;

                    AmjFile f = (AmjFile)obj;

                    ListViewItem item = new ListViewItem(f.name);
                    il.Images.Add(f.name, Icon.ExtractAssociatedIcon(f.path));
                    item.ImageKey = f.name;
                    foreach (String attr in folder.attributesList)
                        item.SubItems.Add(f.getAttribute(attr));
                    item.Tag = f;
                    listView1.Items.Add(item);
                }
            }
            else // Populate current attribute list
            {
                listView1.Columns.Add(folder.attributesList[attrIndex], 300);
                listView1.Columns.Add("Number of Files",100);

                AmjAttribute attrObj;
                ListViewItem item;
                List<String> list;
                foreach (String attrVal in folder.getAttributeValues(attrIndex))
                {
                    attrObj = new AmjAttribute(folder.attributesList[attrIndex], attrVal);
                    item = new ListViewItem(attrVal);

                    list = new List<string>(selectedAttributes);
                    list.Add(attrVal);
                    item.SubItems.Add(folder.getFiles(list).Count.ToString());
                    item.Tag = attrObj;
                    listView1.Items.Add(item);
                }
                attrObj = new AmjAttribute(folder.attributesList[attrIndex], "Unknown");
                item = new ListViewItem("Unknown");

                list = new List<string>(selectedAttributes);
                list.Add("");
                item.SubItems.Add(folder.getFiles(list).Count.ToString());
                item.Tag = attrObj;
                listView1.Items.Add(item);
            }
        }
        
        // ListView Events
        private void listView1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Back)
                goBack();
            else if (e.KeyCode == Keys.F2)
                doEdit();
        }
        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            doOpen();
        }
        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (listView1.FocusedItem.Bounds.Contains(e.Location) == true)
                {
                    setupContextMenu();
                    contextMenuStrip1.Show(Cursor.Position);
                }
            }
        }

        // Context Menu Events
        private void setupContextMenu()
        {
            if (listView1.SelectedItems.Count == 0) return;

            contextMenuStrip1.Items[2].Text = "";
            contextMenuStrip1.Items[3].Text = "";

            AmjObject obj = listView1.SelectedItems[0].Tag as AmjObject;
            if (obj is AmjFolder)
            {
                contextMenuStrip1.Items[0].Visible = true;
                contextMenuStrip1.Items[1].Visible = true;
                contextMenuStrip1.Items[2].Visible = false;
                contextMenuStrip1.Items[3].Visible = true;

                contextMenuStrip1.Items[3].Text = "Revert Back As Regular Folder";
            }
            else if (obj is RegularFolder)
            {
                contextMenuStrip1.Items[0].Visible = true;
                contextMenuStrip1.Items[1].Visible = true;
                contextMenuStrip1.Items[2].Visible = true;
                contextMenuStrip1.Items[3].Visible = false;

                contextMenuStrip1.Items[2].Text = "Setup As Amj Folder";
            }
            else if (obj is AmjFile)
            {
                contextMenuStrip1.Items[0].Visible = true;
                contextMenuStrip1.Items[1].Visible = true;
                contextMenuStrip1.Items[2].Visible = false;
                contextMenuStrip1.Items[3].Visible = false;
            }
            else if (obj is RegularFile)
            {
                contextMenuStrip1.Items[0].Visible = true;
                contextMenuStrip1.Items[1].Visible = true;
                contextMenuStrip1.Items[2].Visible = false;
                contextMenuStrip1.Items[3].Visible = false;
            }
            else if (obj is AmjAttribute)
            {
                contextMenuStrip1.Items[0].Visible = true;
                contextMenuStrip1.Items[1].Visible = false;
                contextMenuStrip1.Items[2].Visible = false;
                contextMenuStrip1.Items[3].Visible = false;
            }
            else
            {
                contextMenuStrip1.Items[0].Enabled = false;
                contextMenuStrip1.Items[1].Enabled = false;
                contextMenuStrip1.Items[2].Enabled = false;
                contextMenuStrip1.Items[3].Enabled = false;
            }
            contextMenuStrip1.PerformLayout();

        }
        private void menuItemOpen_Click(object sender, EventArgs e)
        {
            doOpen();   
        }
        private void menuItemEdit_Click(object sender, EventArgs e)
        {
            doEdit();   
        }
        private void menuItemSetupAsAmjFolder_Click(object sender, EventArgs e)
        {
            doSetupAsAmjFolder();
        }
        private void menuItemRevertBackAsRegularFolder_Click(object sender, EventArgs e)
        {
            doRrevertBackAsRegularFolder();
        }

        // Other Controls Events
        private void btnBack_Click(object sender, EventArgs e)
        {
            goBack();
        }

        // Content Operations
        private void goBack()
        {
            if (curFolder is AmjFolder && selectedAttributes.Count != 0)
            {
                selectedAttributes.RemoveAt(selectedAttributes.Count - 1);
                populateAmjFolder((AmjFolder)curFolder, selectedAttributes.Count);
            }
            else
            {
                if (curFolder.path == "") return;
                DirectoryInfo parent = Directory.GetParent(curFolder.path);
                RegularFolder backFolder = parent == null ? new RegularFolder("") : new RegularFolder(parent.FullName);
                backFolder.loadContents();
                populateRegularFolder(backFolder);
                curFolder = backFolder;
            }
            selectedDirs.RemoveAt(selectedDirs.Count - 1);
            this.txtPath.Text = "";
            foreach (String dir in selectedDirs)
                this.txtPath.Text += dir.Contains("\\") ? dir : dir + "\\";

            if (curFolder is RegularFolder && selectedAttributes.Count == 0)
                if (this.txtPath.Text.Length > 0)
                    File.WriteAllText(Program.SettingsFile, "RecentPath=" + this.txtPath.Text.Substring(0, this.txtPath.Text.Length - 1));
                else
                    File.WriteAllText(Program.SettingsFile, "RecentPath=" + this.txtPath.Text);
        }
        private void doOpen()
        {
            if (listView1.SelectedItems.Count == 0) return;

            AmjObject obj = (AmjObject)listView1.SelectedItems[0].Tag;
            if (obj is AmjFolder)
            {
                AmjFolder f = (AmjFolder)obj;
                curFolder = f;
                f.loadContents();
                populateAmjFolder(f, 0);
                selectedDirs.Add(curFolder.name);
            }
            else if (obj is RegularFolder)
            {
                RegularFolder f = (RegularFolder)obj;
                curFolder = f;
                f.loadContents();
                populateRegularFolder(f);
                selectedDirs.Add(curFolder.name);
            }
            else if (obj is RegularFile)
            {
                RegularFile f = (RegularFile)obj;
                f.open();
            }
            else if (obj is AmjAttribute)
            {
                AmjAttribute a = (AmjAttribute)obj;
                selectedAttributes.Add(a.value=="Unknown"?"":a.value);
                populateAmjFolder((AmjFolder)curFolder, selectedAttributes.Count);
                selectedDirs.Add("[" + a.value + "]");
            }
            this.txtPath.Text = "";
            foreach (String dir in selectedDirs)
                this.txtPath.Text += dir.Contains("\\") ? dir : dir + "\\";

            if (obj is RegularFolder)
                File.WriteAllText(Program.SettingsFile, "RecentPath=" + this.txtPath.Text.Substring(0, this.txtPath.Text.Length - 1));
        }
        private void doEdit()
        {
            if (listView1.SelectedItems.Count == 0) return;

            AmjObject obj = (AmjObject)listView1.SelectedItems[0].Tag;
            if (obj is AmjFolder)
            {
                ConfigForm cf = new ConfigForm((AmjFolder)obj);
                cf.ShowDialog();   
            }
            else if (obj is RegularFolder)
            {
                listView1.SelectedItems[0].BeginEdit();
            }
            else if (obj is AmjFile)
            {
                FileEditor fe = new FileEditor((AmjFile)obj);
                fe.ShowDialog();
                curFolder.loadContents();
                selectedAttributes.Clear();
                populateAmjFolder((AmjFolder)curFolder, 0);
            }
            else if (obj is RegularFile)
            {
                listView1.SelectedItems[0].BeginEdit();
            }
            else if (obj is AmjAttribute)
            {
                
            }
            File.WriteAllText(Program.SettingsFile, "RecentPath=" + this.txtPath.Text);
        }
        private void doRrevertBackAsRegularFolder()
        {
            if (listView1.SelectedItems.Count == 0) return;

            AmjObject obj = (AmjObject)listView1.SelectedItems[0].Tag;
            if (!(obj is AmjFolder)) return;

            AmjFolder folder = (AmjFolder)obj;
            folder.revertAsRegularFolder();
            curFolder.loadContents();
            populateRegularFolder(curFolder);
            MessageBox.Show(this,"Done", "Info");
        }

        private void doSetupAsAmjFolder()
        {
            if (listView1.SelectedItems.Count == 0) return;
            RegularFolder obj = (RegularFolder)listView1.SelectedItems[0].Tag;
            if (!(obj is RegularFolder)) return;

            RegularFolder folder = (RegularFolder)obj;
            AmjFolder amjFolder = new AmjFolder(folder.path);

            curFolder.loadContents();
            populateRegularFolder(curFolder);
            MessageBox.Show(this, "Done", "Info");
        }

        private void listView1_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            if (e.Label == "" || e.Label == null) return;
            
            AmjObject obj = (AmjObject)listView1.SelectedItems[0].Tag;

            if (obj is RegularFile)
                (obj as RegularFile).rename(e.Label.Trim());
            else if (obj is RegularFolder)
                (obj as RegularFolder).rename(e.Label.Trim());
            
            if (curFolder is AmjFolder) populateAmjFolder((AmjFolder)curFolder, selectedAttributes.Count);
            else populateRegularFolder(curFolder);
        }
    }
}
