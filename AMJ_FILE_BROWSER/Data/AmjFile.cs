using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AMJ_FILE_BROWSER.Data
{
    public class AmjFile : RegularFile
    {
        private Dictionary<String, AmjAttribute> attributes;
        public AmjFolder parent;
        public bool nameChanged, attributesChanged;

        public AmjFile(String path, AmjFolder parent) : base(path)
        {
            this.attributes = new Dictionary<String, AmjAttribute>();
            this.parent = parent;
            nameChanged = false;
            attributesChanged = false;
        }
        
        public void addAttribute(String attr, String value)
        {
            if (attributes.ContainsKey(attr))
                attributes[attr].value = value;
            else
                attributes.Add(attr, new AmjAttribute(attr, value));
            attributesChanged = true;
        }
        public void removeAttribute(String attr)
        {
            if (attributes.ContainsKey(attr))
                attributes.Remove(attr);
            attributesChanged = true;
        }
        public String getAttribute(String attr)
        {
            if (attributes.ContainsKey(attr))
                return attributes[attr].value;
            else
                return "";
        }

        public override void rename(string newName)
        {
            base.rename(newName);
            nameChanged = true;
        }
    }
}
