using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMJ_FILE_BROWSER.Data
{
    public class AmjAttribute : AmjObject
    {
        public String name;
        public String value;

        public AmjAttribute(String name, String value)
        {
            this.name = name;
            this.value = value;
        }
    }
}
