using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AMJ_FILE_BROWSER.Data
{
    public class RegularFolder : AmjObject
    {
        public String name;
        public String path;
        public String oldPath;
        public List<AmjObject> contents;

        public RegularFolder(String path)
        {
            if (!Directory.Exists(path) && path != "")
                throw new FileNotFoundException("Folder doesn't exist", name);

            this.path = path;
            this.oldPath = this.path;
            this.name = path == ""?"My Computer":Path.GetFileName(path);
            this.name = this.name == "" ? this.path : this.name;
            contents = new List<AmjObject>();
        }

        public void rename(String newName)
        {
            if (newName == name) return;
            Directory.Move(path, Directory.GetParent(path).FullName + "\\" + newName);
            this.oldPath = this.path;
            this.path = Directory.GetParent(path).FullName + "\\" + newName;
            this.name = Path.GetFileName(this.path);
        }
        public void delete()
        {
            Directory.Delete(path, true);
        }

        public virtual void loadContents()
        {
            contents.Clear();

            if (path == "")
                foreach (DriveInfo d in DriveInfo.GetDrives())
                    contents.Add(new RegularFolder(d.Name));
            else
            {
                foreach (String f in Directory.GetFiles(path))
                    contents.Add(new RegularFile(f));

                foreach (String d in Directory.GetDirectories(path))
                {
                    if (File.Exists(d + "\\amj.attrList"))
                        contents.Add(new AmjFolder(d));
                    else
                        contents.Add(new RegularFolder(d));
                }
            }
        }
    }
}
