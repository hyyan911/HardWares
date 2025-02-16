using HardWares.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

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

            if (DllImporter.GetArchitecture() == Architecture.X64 || DllImporter.GetArchitecture() == Architecture.Arm64)
            {
                importer = new DllImporter();
                importer.ExtractEmbeddedDlls(Path.Combine(Environment.CurrentDirectory, "PI_GCS2_DLL" + ".dll"), Resources.PI_GCS2_DLL_x64);
            }
            else
            {
                importer = new DllImporter();
                importer.ExtractEmbeddedDlls(Path.Combine(Environment.CurrentDirectory, "PI_GCS2_DLL" + ".dll"), Resources.PI_GCS2_DLL);
            }

            if (DllImporter.GetArchitecture() == Architecture.X64 || DllImporter.GetArchitecture() == Architecture.Arm64)
            {
                importer = new DllImporter();
                importer.ExtractEmbeddedDlls(Path.Combine(Environment.CurrentDirectory, "ziDotNETCore-win64" + ".dll"), Resources.ziDotNETCore_win64);
            }
            else
            {
                importer = new DllImporter();
                importer.ExtractEmbeddedDlls(Path.Combine(Environment.CurrentDirectory, "ziDotNETCore-win32" + ".dll"), Resources.ziDotNETCore_win32);
            }

            importer = new DllImporter();
            importer.ExtractEmbeddedDlls(Path.Combine(Environment.CurrentDirectory, "spinapi64.dll"), Resources.spinapi64);
            importer = new DllImporter();
            importer.ExtractEmbeddedDlls(Path.Combine(Environment.CurrentDirectory, "spinapi.dll"), Resources.spinapi);
        }
    }
}
