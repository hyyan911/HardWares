using HardWares.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HardWares.APIS
{
    public class ExternalDlls
    {
        public static void LoadDlls()
        {
            DllImporter importer;
            PropertyInfo[] infos = typeof(Properties.Resources).GetRuntimeProperties().ToArray();
            foreach (PropertyInfo info in infos)
            {
                if (info.Name == "Culture") continue;
                if (info.Name == "ResourceManager") continue;
                importer = new DllImporter();
                importer.ExtractEmbeddedDlls(info.Name + ".dll", (byte[])Resources.ResourceManager.GetObject(info.Name));
            }
        }
    }
}
