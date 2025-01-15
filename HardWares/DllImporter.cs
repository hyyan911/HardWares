using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HardWares
{
    /// <summary>
    /// 预加载指定的非托管DLL文件
    /// </summary>
    internal class DllImporter
    {
        internal string tempFolder { get; private set; } = "";

        /// <summary>
        /// Extract DLLs from resources to temporary folder
        /// </summary>
        /// <param name="savedllPath">name of DLL file to create (including dll suffix)</param>
        /// <param name="resourceBytes">The resource name (fully qualified)</param>
        public void ExtractEmbeddedDlls(string savedllPath, byte[] resourceBytes)
        {
            Assembly assem = Assembly.GetExecutingAssembly();
            string[] names = assem.GetManifestResourceNames();
            AssemblyName an = assem.GetName();

            // See if the file exists, avoid rewriting it if not necessary
            bool rewrite = true;
            if (File.Exists(savedllPath))
            {
                byte[] existing = File.ReadAllBytes(savedllPath);
                if (resourceBytes.SequenceEqual(existing))
                {
                    rewrite = false;
                }
            }
            if (rewrite)
            {
                File.WriteAllBytes(savedllPath, resourceBytes);
            }
        }

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
        static extern IntPtr LoadLibrary(string lpFileName);

        /// <summary>
        /// managed wrapper around LoadLibrary
        /// </summary>
        /// <param name="dllName"></param>
        public void LoadDll(string dllName)
        {
            if (tempFolder == "")
            {
                throw new Exception("Please call ExtractEmbeddedDlls before LoadDll");
            }
            IntPtr h = LoadLibrary(dllName);
            if (h == IntPtr.Zero)
            {
                Exception e = new Win32Exception();
                throw new DllNotFoundException("Unable to load library: " + dllName + " from " + tempFolder, e);
            }
        }

    }
}
