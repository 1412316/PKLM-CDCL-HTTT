using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhanLop
{
    class DocumentClass
    {
        String name { get; set; }
        int count { get; set; }

        DocumentClass(String name, int count)
        {
            this.name = name;
            this.count = count;
        }
    }
}
