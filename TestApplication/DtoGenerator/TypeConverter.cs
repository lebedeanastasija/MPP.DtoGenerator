using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DtoGenerator
{
    class TypeConverter
    {
        private static readonly TypeConverter converter = new TypeConverter();

        private Dictionary<TypeInfo, string> typeDictionary;

        private TypeConverter()
        {
            typeDictionary = new Dictionary<TypeInfo, string>();
            typeDictionary.Add(new TypeInfo("integer", "int32"), "System.Int32");
            typeDictionary.Add(new TypeInfo("integer", "int64"), "System.Int64");
            typeDictionary.Add(new TypeInfo("number", "float"), "System.Single");
            typeDictionary.Add(new TypeInfo("number", "double"), "System.Double");
            typeDictionary.Add(new TypeInfo("string", "byte"), "System.Byte");
            typeDictionary.Add(new TypeInfo("boolean", ""), "System.Boolean");
            typeDictionary.Add(new TypeInfo("string", "date"), "System.DateTime");
            typeDictionary.Add(new TypeInfo("string", "string"), "System.String");
        }

        public static TypeConverter Instance
        {
            get
            {
                return converter;
            }
        }

        public string TryGetTypeName(TypeInfo typeInfo)
        {
            string result = null;
            if (typeDictionary.ContainsKey(typeInfo))
            {
                result = typeDictionary[typeInfo];
            }
            return result;
        }
    }
}
