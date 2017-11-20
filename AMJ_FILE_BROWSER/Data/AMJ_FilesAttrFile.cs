using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AMJ_FILE_BROWSER.Data
{
    class AMJ_FilesAttrFile
    {
        private String filePath;
        public Dictionary<String, List<String>> filesAttributes;

        public AMJ_FilesAttrFile(String filePath)
        {
            this.filePath = filePath;

            if (!File.Exists(this.filePath))
                File.WriteAllText(this.filePath, "");

            File.SetAttributes(this.filePath, FileAttributes.Hidden);
            filesAttributes = new Dictionary<string, List<string>>();
        }

        public void loadFilesAttr()
        {
            foreach (String line in File.ReadAllLines(this.filePath))
            {
                if (line.Split(',').Count() != 3) continue;

                String fPath = line.Split(',')[0];
                String attr = line.Split(',')[1];
                String attrVal = line.Split(',')[2];

                if (filesAttributes.ContainsKey(fPath))
                {
                    List<String> list = filesAttributes[fPath];
                    list.Add(attr + ":" + attrVal);
                }
                else
                {
                    List<String> list = new List<string>();
                    list.Add(attr + ":" + attrVal);
                    filesAttributes.Add(fPath, list);
                }
            }
        }

        public void addNewFileAttr(String fPath, String attr, String attrVal)
        {
            File.SetAttributes(this.filePath, FileAttributes.Normal);
            StreamWriter file = new StreamWriter(this.filePath, true);
            file.WriteLine(fPath + "," + attr + "," + attrVal);
            file.Close();
            File.SetAttributes(this.filePath, FileAttributes.Hidden);

            if (filesAttributes.ContainsKey(fPath))
            {
                List<String> list = filesAttributes[fPath];
                list.Add(attr + ":" + attrVal);
            }
            else
            {
                List<String> list = new List<string>();
                list.Add(attr + ":" + attrVal);
                filesAttributes.Add(fPath, list);
            }
        }
    }
}
