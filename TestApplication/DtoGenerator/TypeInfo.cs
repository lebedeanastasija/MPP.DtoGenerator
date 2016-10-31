using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DtoGenerator
{
    public class TypeInfo
    {
        public string name { get; set; }
        public string format { get; set; }

        public TypeInfo(string name, string format)
        {
            this.name = name;
            this.format = format;
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
            {
                return true;
            }
            if (this is TypeInfo)
            {
                TypeInfo typeInfo = (TypeInfo)obj;
                if (typeInfo.name != name)
                {
                    return false;
                }
                if (typeInfo.format != format)
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return 31 * name.GetHashCode() + format.GetHashCode();
        }
    }
}
