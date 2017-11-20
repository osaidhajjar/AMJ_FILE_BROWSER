using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AMJ_FILE_BROWSER
{
    public class AMJ_File
    {
        public String Name;
        public String filePath;
        private Dictionary<String, String> attributes;

        public AMJ_File(String filePath)
        {
            this.filePath = filePath;
            this.Name = Path.GetFileName(filePath);
            this.attributes = new Dictionary<string, string>();
        }        
        
        public void addAttribute(String attr, String value)
        {
            if (attributes.ContainsKey(attr))
                this.attributes[attr] = value;
            else
                this.attributes.Add(attr, value);
        }

        public String getAttribute(String attr)
        {
            if (attributes.ContainsKey(attr))
                return attributes[attr];
            else
                return "";
        }
    }
}
