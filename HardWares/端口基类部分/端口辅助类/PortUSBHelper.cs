using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HardWares.端口基类;
using HardWares.端口基类.COM串口;
using HardWares.端口基类.TCPIP串口;

namespace HardWares.端口基类部分.VISA资源辅助类
{
    internal class PortResourceUSBHelper : PortResourceHelper
    {
        public PortResourceUSBHelper(Func<string, string> getProductName) : base(getProductName)
        {
        }

        #region COM部分

        public string GetIdentifier(USBInternalInterface device, List<object> param)
        {
            EnumerateDevices(device);

            int ind = DevNames.FindIndex(x => x.Key == param[0] as string);
            if (ind == -1)
            {
                throw new Exception("设备不存在");
            }
            return DevNames[ind].Value;
        }

        /// <summary>
        /// 获取USB设备
        /// </summary>
        public List<string> EnumerateDevices(USBInternalInterface device)
        {
            List<string> res = device.RawEnumerateUSBDevices();
            var names = res.Select(x => GetProductName(x)).ToList();
            if (GetProductName == null)
            {
                DevNames = res.Select(x => new KeyValuePair<string, string>(x, x)).ToList();
                return res.Select(x => x).ToList();
            }
            DevNames = res.Select(x => new KeyValuePair<string, string>(GetProductName(x), x)).ToList();

            return DevNames.Select(x => x.Key).ToList();
        }



        #endregion
    }
}
