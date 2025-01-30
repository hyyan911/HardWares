using HardWares.数据处理;
using HardWares.端口基类;
using HardWares.端口基类.COM串口;
using HardWares.端口基类部分;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardWares.源表.KEITHLEY_2400
{
    public partial class PowerSource : PowerSourceBase, COMInternalInterface, COMOuterInterface
    {
        public bool ConnectCOM(string COMName, int baudrate, out Exception exc)
        {
            return Connect(PortType.COM, out exc, Encoding.UTF8, COMName, baudrate);
        }

        void COMInternalInterface.CloseCOMPort()
        {
            (Instance as SerialPort).Close();
        }

        void COMInternalInterface.COMInitAction(object PortInstance)
        {
        }

        byte[] COMInternalInterface.COMPortRead()
        {
            int count = (Instance as SerialPort).ReadBufferSize;
            byte[] result = new byte[count];
            (Instance as SerialPort).Read(result, 0, count);
            return result;
        }

        void COMInternalInterface.COMPortWrite(byte[] value)
        {
            (Instance as SerialPort).Write(value, 0, value.Length);
        }

        void COMInternalInterface.ConnectedCOMAction()
        {
        }

        bool COMInternalInterface.IsCOMOpen()
        {
            return (Instance as SerialPort).IsOpen;
        }

        object COMInternalInterface.OpenCOMPort(List<object> param)
        {
            SerialPort port = new SerialPort(param[0] as string, (int)param[1]);
            port.Open();
            return port;
        }

        void COMInternalInterface.ReceiveCOMAct()
        {
            List<List<byte>> retu = DataProcess.ProcessReceivedSerialData('\r', ReceiveBuffer, out List<byte> result);
            ReceiveBuffer = result;
            if (retu.Count != 0)
            {
                QueryState = true;
                //只取第一条指令
                QueryReturnedData = new List<string>() { PortArranger.Coder.GetString(retu[0].ToArray()) };
            }
        }

        bool COMInternalInterface.TestCOMAction()
        {
            string res = ThreadSafeQuery("*IDN?\r", 1000);
            if (res.Contains("KEITHLEY") && res.Contains("MODEL 2400"))
            {
                //获取序列号
                string strsegs = res.Split(',')[2];
                ProductName = "KEITHLEY 2400 " + strsegs;
                return true;
            }
            return false;
        }
    }
}
