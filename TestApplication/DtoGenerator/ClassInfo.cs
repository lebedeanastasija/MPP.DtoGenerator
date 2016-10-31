using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DtoGenerator
{
    public class ClassInfo
    {
        public string ClassName { get; set; }
        public List<PropertyInfo> Properties { get; set; }
    }
}
