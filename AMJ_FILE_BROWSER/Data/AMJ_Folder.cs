using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AMJ_FILE_BROWSER.Data
{
    public class AMJ_Folder
    {
        public String folderPath;
        public String folderName;
        private List<AMJ_File> filesList;
        private Dictionary<String, List<String>> uniqueAttrValues;

        private AMJ_AttrListFile attrListFile;
        private AMJ_FilesAttrFile filesAttrFile;
        
        public AMJ_Folder()
        {
            folderPath = "";
            filesList = new List<AMJ_File>();
            uniqueAttrValues = new Dictionary<string, List<string>>();
        }

        public void setupFolder(String folPath)
        {
            //Reset
            folderPath = "";
            filesList.Clear();
            uniqueAttrValues.Clear();

            this.folderPath = folPath;
            this.folderName = Path.GetFileName(folPath);
            attrListFile = new AMJ_AttrListFile(folPath + "\\amj.attrList");
            filesAttrFile = new AMJ_FilesAttrFile(folPath + "\\amj.filesAttr");


            //Load Files and populate attrList and uniqueAttrValues
            attrListFile.loadAttrList();
            filesAttrFile.loadFilesAttr();
            foreach (String attr in attrListFile.attributes)
                uniqueAttrValues.Add(attr, new List<String>());

            loadFiles(this.folderPath);
        }

        public List<String> getAttrValues(String attr)
        {
            if (uniqueAttrValues.ContainsKey(attr))
                return uniqueAttrValues[attr];
            else
                return new List<String>();
        }

        public List<String> getAttributes()
        {
            return this.attrListFile.attributes;
        }

        public List<AMJ_File> getFiles(List<String> attrFilter, List<bool> attrMask)
        {
            if (attrFilter == null) return filesList;

            if (attrFilter.Count != attrMask.Count || 
                attrFilter.Count != getAttributes().Count()) return new List<AMJ_File>();

            List<AMJ_File> files = new List<AMJ_File>();
            foreach(AMJ_File file in filesList)
            {
                bool add = true;
                for (int i=0; i<attrMask.Count; i++)
                {
                    if (!attrMask[i]) continue;
                    if (file.getAttribute(getAttributes()[i]) != attrFilter[i])
                    {
                        add = false;
                        break;
                    }
                }
                if (add)
                    files.Add(file);
            }

            return files;
        }

        public void addAttributesList(List<String> attrs)
        {
            List<String> list = this.attrListFile.attributes;
            foreach (String attr in attrs)
                if (!list.Contains(attr))
                    list.Add(attr);

            this.attrListFile.addAttrList(list);
        }

        public void addNewFileAttr(AMJ_File file, String attr, String attrVal)
        {
            this.filesAttrFile.addNewFileAttr(file.filePath, attr, attrVal);
        }

        private void loadFiles(String folder)
        {
            foreach (String fPath in Directory.GetFiles(folder))
            {
                if (Path.GetFileName(fPath) == "amj.attrList" ||
                    Path.GetFileName(fPath) == "amj.filesAttr")
                    continue;

                AMJ_File f = new AMJ_File(fPath);
                if (filesAttrFile.filesAttributes.ContainsKey(fPath))
                    foreach (String attrPair in filesAttrFile.filesAttributes[fPath])
                    {
                        String attr = attrPair.Split(':')[0];
                        String value = attrPair.Split(':')[1];
                        f.addAttribute(attr, value);
                        if (!uniqueAttrValues[attr].Contains(value))
                            uniqueAttrValues[attr].Add(value);
                    }
                filesList.Add(f);
            }
            foreach (String dir in Directory.GetDirectories(folder))
                loadFiles(dir);
        }
    }
}
