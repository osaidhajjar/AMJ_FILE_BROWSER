using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AMJ_FILE_BROWSER.Data
{
    public class AmjFolder : RegularFolder
    {
        public List<String> attributesList;
        private Dictionary<int, List<String>> uniqueAttrValues;
        private Dictionary<String, List<AmjAttribute>> filesAttributes;
        
        private String attrListFile;
        private String filesAttrsFile;

        public AmjFolder(String path) : base(path)
        {
            uniqueAttrValues = new Dictionary<int, List<string>>();
            filesAttributes = new Dictionary<String, List<AmjAttribute>>();
            attributesList = new List<string>();

            attrListFile = path + "\\amj.attrList";
            filesAttrsFile = path + "\\amj.filesAttr";

            if (!File.Exists(attrListFile))
                File.WriteAllText(attrListFile, "");
            File.SetAttributes(this.attrListFile, FileAttributes.Hidden);

            if (!File.Exists(filesAttrsFile))
                File.WriteAllText(filesAttrsFile, "");
            File.SetAttributes(filesAttrsFile, FileAttributes.Hidden);
        }

        public override void loadContents()
        {
            contents.Clear();
            uniqueAttrValues.Clear();

            loadAttributesListFromDB();
            loadFilesAttributesFromDB();

            loadContentsFromFolder(path);
            updateFilesAttributesFile();
        }

        public List<AmjFile> getFiles(List<String> attrFilter)
        {
            if (attrFilter == null) return contents.Cast<AmjFile>().ToList();

            List<AmjFile> list = new List<AmjFile>();
            foreach (AmjFile f in contents)
            {
                bool include = true;
                for (int i = 0; i < attrFilter.Count; i++)
                {
                    if (attrFilter[i] != f.getAttribute(attributesList[i]))
                    {
                        include = false;
                        break;
                    }
                }
                if (include) list.Add(f);
            }
            return list;
        }
        public List<String> getAttributeValues(int attrIndex)
        {
            if (uniqueAttrValues.ContainsKey(attrIndex))
                return uniqueAttrValues[attrIndex];
            else
                return new List<string>();
        }
        private void loadContentsFromFolder(String folder)
        {
            foreach(String fp in Directory.GetFiles(folder))
            {
                String fPath = Path.GetFullPath(fp).ToLower();
                if (string.Equals(Path.GetFileName(fPath), Path.GetFileName(attrListFile), StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(Path.GetFileName(fPath), Path.GetFileName(filesAttrsFile), StringComparison.OrdinalIgnoreCase))
                    continue;

                AmjFile f = new AmjFile(fPath, this);
                if (filesAttributes.ContainsKey(fPath))
                    foreach (AmjAttribute attr in filesAttributes[fPath])
                    {
                        int attrIndex = getAttrIndex(attr.name);
                        if (attrIndex == -1) continue;

                        f.addAttribute(attr.name, attr.value);

                        if (!uniqueAttrValues.ContainsKey(attrIndex))
                            uniqueAttrValues.Add(attrIndex, new List<string>());
                        if (!uniqueAttrValues[attrIndex].Contains(attr.value))
                            uniqueAttrValues[attrIndex].Add(attr.value);
                    }
                contents.Add(f);
            }
            foreach (String dir in Directory.GetDirectories(folder))
                loadContentsFromFolder(dir);
        }

        private int getAttrIndex(String attr)
        {
            for (int i = 0; i < attributesList.Count; i++)
                if (attributesList[i] == attr) return i;
            return -1;
        }

        private void loadAttributesListFromDB()
        {
            attributesList.Clear();
            foreach (String attr in File.ReadAllLines(attrListFile))
                if (attr.Length > 0) attributesList.Add(attr);
        }

        private void loadFilesAttributesFromDB()
        {
            filesAttributes.Clear();
            foreach (String line in File.ReadAllLines(filesAttrsFile))
            {
                if (line.Split(',').Count() != 3) continue;

                String fPath = Path.GetFullPath(line.Split(',')[0]).ToLower();
                String attr = line.Split(',')[1];
                String attrVal = line.Split(',')[2];

                if (!attributesList.Contains(attr)) continue;

                if (filesAttributes.ContainsKey(fPath))
                {
                    foreach (AmjAttribute fAttr in filesAttributes[fPath])
                        if (fAttr.name == attr)
                        {
                            fAttr.value = attrVal;
                            continue;
                        }
                    AmjAttribute a = filesAttributes[fPath].Find(s => s.name == attr);
                    if (a != null) a.value = attrVal;
                    else filesAttributes[fPath].Add(new AmjAttribute(attr, attrVal));
                }
                else
                {
                    List<AmjAttribute> list = new List<AmjAttribute>();
                    list.Add(new AmjAttribute(attr, attrVal));
                    filesAttributes.Add(fPath, list);
                }
            }
        }

        public void revertAsRegularFolder()
        {
            File.Delete(attrListFile);
            File.Delete(filesAttrsFile);
        }

        private void updateFilesAttributesFile()
        {
            List<String> lines = new List<string>();
            foreach (String key in filesAttributes.Keys)
                foreach (AmjAttribute attr in filesAttributes[key])
                    lines.Add(key + "," + attr.name + "," + attr.value);
            File.SetAttributes(this.filesAttrsFile, FileAttributes.Normal);
            File.WriteAllLines(filesAttrsFile, lines);
            File.SetAttributes(this.filesAttrsFile, FileAttributes.Hidden);
        }

        private void updateAttributesFile()
        {
            List<String> lines = new List<string>();
            foreach (String attr in attributesList)
                lines.Add(attr);
            File.SetAttributes(this.attrListFile, FileAttributes.Normal);
            File.WriteAllLines(attrListFile, lines);
            File.SetAttributes(this.attrListFile, FileAttributes.Hidden);
        }

        public void updateAttributes(List<String> newAttributes)
        {
            attributesList.Clear();
            attributesList = newAttributes;
            updateAttributesFile();
            loadContents();
        }
        public void updateFile(AmjFile file)
        {
            if (!contents.Contains(file)) return;

            List<AmjAttribute> values;
            if (file.nameChanged)
            {
                if (filesAttributes.ContainsKey(file.oldPath))
                {
                    values = filesAttributes[file.oldPath];
                    filesAttributes.Remove(file.oldPath);
                    filesAttributes.Add(file.path, values);
                }
            }

            if (file.attributesChanged)
            {
                values = new List<AmjAttribute>();
                foreach (String attr in attributesList)
                {
                    String attrVal = file.getAttribute(attr);
                    if (attrVal.Length > 0) values.Add(new AmjAttribute(attr, attrVal));
                }
                if (filesAttributes.ContainsKey(file.oldPath))
                    filesAttributes[file.path] = values;
                else
                    filesAttributes.Add(file.path, values);
            }
            updateFilesAttributesFile();
            loadContents();
        }
    }
}
