using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using DtoGenerator;


namespace TestApplication
{
    public class JsonClassReader
    {
        public static List<ClassInfo> ReadClassInfo(string fileName)
        {
            List<ClassInfo> classInfo;
            JsonReader reader = new JsonTextReader(new StreamReader(fileName));
            JsonSerializer serializer = new JsonSerializer();
            classInfo = serializer.Deserialize<List<ClassInfo>>(reader);
            return classInfo;
        }
    }
}
