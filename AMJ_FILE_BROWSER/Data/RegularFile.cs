using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AMJ_FILE_BROWSER.Data
{
    public class RegularFile : AmjObject
    {
        public String name;
        public String path;
        public String oldPath;

        public RegularFile(String path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("File doesn't exist");

            this.path = Path.GetFullPath(path).ToLower();
            this.name = Path.GetFileName(path);
            this.oldPath = this.path;
        }

        public void open()
        {
            System.Diagnostics.Process.Start(path);
        }
        public virtual void rename(String newName)
        {
            if (newName == name) return;
            File.Move(path, Directory.GetParent(path).FullName + "\\" + newName);
            this.oldPath = this.path;
            this.path = Path.GetFullPath(Directory.GetParent(path).FullName + "\\" + newName).ToLower();
            this.name = Path.GetFileName(this.path);
        }
        public virtual void delete()
        {
            File.Delete(path);
        }
    }
}
