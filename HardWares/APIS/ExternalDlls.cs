using HardWares.Properties;
using System;
using System.Collections.Generic;
using System.IO;
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
            DllImporter importer = new DllImporter();
            importer.ExtractEmbeddedDlls(Path.Combine(Environment.CurrentDirectory, "Thorlabs.MotionControl.DeviceManager" + ".dll"), Resources.Thorlabs_MotionControl_DeviceManager);

            importer = new DllImporter();
            importer.ExtractEmbeddedDlls(Path.Combine(Environment.CurrentDirectory, "Thorlabs.MotionControl.FilterFlipper" + ".dll"), Resources.Thorlabs_MotionControl_FilterFlipper);

            if (Environment.Is64BitOperatingSystem)
            {
                importer = new DllImporter();
                importer.ExtractEmbeddedDlls(Path.Combine(Environment.CurrentDirectory, "PI_GCS2_DLL" + ".dll"), Resources.PI_GCS2_DLL);
            }
            else
            {
                importer = new DllImporter();
                importer.ExtractEmbeddedDlls(Path.Combine(Environment.CurrentDirectory, "PI_GCS2_DLL" + ".dll"), Resources.PI_GCS2_DLL);
            }
        }
    }
}
