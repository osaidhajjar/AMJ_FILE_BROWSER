using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AMJ_FILE_BROWSER.Data
{
    class AMJ_AttrListFile
    {
        private String filePath;
        public List<String> attributes;

        public AMJ_AttrListFile(String filePath)
        {
            this.filePath = filePath;

            if (!File.Exists(this.filePath))
                File.WriteAllText(this.filePath, "");

            File.SetAttributes(this.filePath, FileAttributes.Hidden);
            attributes = new List<string>();
        }

        public void loadAttrList()
        {
            foreach (String attr in System.IO.File.ReadAllLines(this.filePath))
                if (attr.Length > 0) attributes.Add(attr);
        }

        public void addAttrList(List<String> attrList)
        {
            File.SetAttributes(this.filePath, FileAttributes.Normal);
            StreamWriter file = new StreamWriter(this.filePath);
            foreach (String attr in attrList)
                file.WriteLine(attr);
            file.Close();
            File.SetAttributes(this.filePath, FileAttributes.Hidden);
        }
    }
}
