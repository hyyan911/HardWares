using HardWares.数据处理;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace HardWares.APIS.PI
{
    /// <summary>
    /// 定义所有可用串口的PIAPI
    /// </summary>
    internal class PIAPI2 : PIAPI
    {

        static PIAPI2()
        {
            if (!Environment.Is64BitOperatingSystem)
            {
                LoadGCSDLL("PI_GCS2_DLL.dll", Properties.Resources.PI_GCS2_DLL_x64);
                return;
            }
            LoadGCSDLL("PI_GCS2_DLL.dll", Properties.Resources.PI_GCS2_DLL);
        }

        /// <summary>
        /// 加载GCSDLL
        /// </summary>
        /// <param name="gcsName"></param>
        /// <param name="resourceBytes"></param>
        /// <returns></returns>
        internal static string LoadGCSDLL(string gcsName, byte[] resourceBytes)
        {
            DllImporter importer = new DllImporter();
            importer.ExtractEmbeddedDlls(gcsName, resourceBytes);
            return gcsName;
        }

        #region 设备连接部分

        /// <summary>
        /// Cancels connecting thread with given ID
        /// </summary>
        /// <param name="thread"></param>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_CancelConnect", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_CancelConnect(int thread);

        /// <summary>
        /// Closes connection to the controller
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_CloseConnection", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_CloseConnection(int ID);

        /// <summary>
        /// Closes all daisy chain connections
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_CloseAllDaisyChains", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern void PI_CloseAllDaisyChains();

        /// <summary>
        /// Closes the connection to a daisy chain port
        /// </summary>
        /// <param name="iPortId"></param>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_CloseDaisyChain", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern void PI_CloseDaisyChain(int iPortId);

        /// <summary>
        /// Opens a daisy chain device 
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_ConnectDaisyChainDevice", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern int PI_ConnectDaisyChainDevice(int iPortId, int iDeviceNumber);

        /// <summary>
        /// Opens a USB connection to a controller using an identification string
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_ConnectUSB", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern int PI_ConnectUSB(string szDescription);

        /// <summary>
        /// Opens a USB connection to a controller using an identification string
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_ConnectUSBWithBaudRate", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern int PI_ConnectUSBWithBaudRate(string szDescription, int iBaudRate);

        /// <summary>
        /// Enables reconnecting to a controller
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_EnableReconnect", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_EnableReconnect(int ID, bool bEnable);

        /// <summary>
        /// Switches off the internal baud rate scan when connecting
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_EnableBaudRateScan", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern void PI_EnableBaudRateScan(bool enableBaudRateScan);

        /// <summary>
        /// Lists the identification strings of all controllers available viaUSB interfaces
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_EnumerateUSB", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern int PI_EnumerateUSB(IntPtr szBuffer, int iBufferSize, string szFilter);

        /// <summary>
        /// Gets ID of a connected controller
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_GetControllerID", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern int PI_GetControllerID(int threadID);

        /// <summary>
        /// Gets the ID of a daisy chain connection
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_GetDaisyChainID", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern int PI_GetDaisyChainID(int threadID);

        /// <summary>
        /// Gets the ID of connected daisy chains
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_GetConnectedDaisyChains", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern int PI_GetConnectedDaisyChains(IntPtr daisyChainIds, int nrDaisyChainsIds);

        /// <summary>
        /// Checks for connected daisy chains and devices in connected daisy chains
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_GetDevicesInDaisyChain", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern int PI_GetDevicesInDaisyChain(int portId, IntPtr numberOfDevices, IntPtr buffer, int bufferSize);

        /// <summary>
        /// Gets the error status of the DLL of the controller
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_GetError", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern int PI_GetError(int ID);

        /// <summary>
        /// Queries for errors in certain operational areas
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_GetErrorStatus", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_GetErrorStatus(int ID, IntPtr pbIsReferencedArray, IntPtr pbIsReferencing, IntPtr pbIsMovingArray, IntPtr pbIsMotionErrorArray);

        /// <summary>
        /// Queries the error code for an unsuccessful connection
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_GetInitError", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern int PI_GetInitError();

        /// <summary>
        /// Closes all daisy chain connections
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_GetInterfaceDescription", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_GetInterfaceDescription(int ID, IntPtr szBuffer, int iBufferSize);

        /// <summary>
        /// Queries the number of connected daisy chains
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_GetNrConnectedDaisyChains", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern int PI_GetNrConnectedDaisyChains();

        /// <summary>
        /// Gets the current DLL version
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_GetDllVersionInformation", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_GetDllVersionInformation(int ID, IntPtr dllVersionsInformationBuffer, int bufferSize);

        /// <summary>
        /// Lists the controllers supported by the DLL
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_GetSupportedControllers", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_GetSupportedControllers(IntPtr szBuffer, int iBufferSize);

        /// <summary>
        /// Lists the parameters supported by the controller
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_GetSupportedParameters", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_GetSupportedParameters(int ID, IntPtr piParameterIdArray, IntPtr piCommandLevelArray, IntPtr piMemoryLocationArray, IntPtr piDataTypeArray, IntPtr piNumberOfItems, int iiBufferSize, IntPtr szParameterName, int iMaxParameterNameSize);

        /// <summary>
        /// Closes all daisy chain connections
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_InterfaceSetupDlg", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern int PI_InterfaceSetupDlg(string szRegKeyName);

        /// <summary>
        /// Checks whether a certain controller is connected
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_IsConnected", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_IsConnected(int ID);

        /// <summary>
        /// Checks whether a certain thread is trying to establish communication
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_IsConnecting", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_IsConnecting(int threadID, IntPtr bCOnnecting);

        /// <summary>
        /// Opens an RS-232 (“COM”) interface to a daisy chain
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_OpenRS232DaisyChain", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern int PI_OpenRS232DaisyChain(int iPortNumber, int iBaudRate, IntPtr pNumberOfConnectedDaisyChainDevices, IntPtr szDeviceIDNs, int iBufferSize);

        /// <summary>
        /// Opens a USB interface to a daisy chain
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_OpenUSBDaisyChain", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern int PI_OpenUSBDaisyChain(string szDescription, IntPtr pNumberOfConnectedDaisyChainDevices, IntPtr szDeviceIDNs, int iBufferSize);

        /// <summary>
        /// Closes all daisy chain connections and resets all saved data
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_ResetDaisyChainState", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern void PI_ResetDaisyChainState();

        /// <summary>
        /// Changes the internal timeout of all PI_Connect…() functions
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_SetConnectTimeout", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern void PI_SetConnectTimeout(int timeoutInMS);

        /// <summary>
        /// Sets the maximum device ID for the next daisy chain scan
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_SetDaisyChainScanMaxDeviceID", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern int PI_SetDaisyChainScanMaxDeviceID(int maxID);

        /// <summary>
        /// Sets the error-check mode for the library
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_SetErrorCheck", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_SetErrorCheck(int ID, bool bErrorCheck);

        /// <summary>
        /// Sets the number of timeouts before the connection is closed
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_SetNrTimeoutsBeforeClose", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern int PI_SetNrTimeoutsBeforeClose(int ID, int nrTimeoutsBeforeClose);

        /// <summary>
        /// Sets the timeout for function calls
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_SetTimeout", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern int PI_SetTimeout(int ID, int timeoutInMS);

        /// <summary>
        /// Starts the scanning process for busy addresses in daisychain that is connected via RS-232 
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_StartDaisyChainScanRS232", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern int PI_StartDaisyChainScanRS232(int iPortNumber, int iBaudRate);

        /// <summary>
        /// Starts the scanning process for busy addresses in daisychain that is connected via USB
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_StartDaisyChainScanUSB", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern int PI_StartDaisyChainScanUSB(string szDescription);

        /// <summary>
        /// Stops a daisy chain scan immediately
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_StopDaisyChainScan", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern int PI_StopDaisyChainScan(int threadId);

        /// <summary>
        /// Translates an error numbe into an error message
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_TranslateError", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_TranslateError(int errNr, IntPtr szBuffer, int iBufferSize);

        /// <summary>
        /// Starts a background thread to establish connection to a controller via RS232
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_TryConnectRS232", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern int PI_TryConnectRS232(int nPortNr, int iBaudRate);

        /// <summary>
        /// Starts a background thread to establish connection to a controller via RS232
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_ConnectRS232", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern int PI_ConnectRS232(int nPortNr, int iBaudRate);

        /// <summary>
        /// Starts a background thread to establish connection to a controller via USB
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_TryConnectUSB", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern int PI_TryConnectUSB(string szDescription);

        #endregion


        #region 通信部分

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_GcsCommandset", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_GcsCommandset(int ID, string szCommand);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_GcsGetAnswer", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_GcsGetAnswer(int ID, IntPtr szAnswer, int iBufferSize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_GcsGetAnswerSize", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_GcsGetAnswerSize(int ID, IntPtr iAnswerSize);

        #endregion

        #region 指令部分
        /// <summary>
        /// Starts a scanning procedure for a better determination of the maximum intensity of an analog input signal.
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_AAP", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_AAP(int ID, string szAxis1, double dLength1, IntPtr szAxis2, double dLength2, double dAlignStep, int iNrRepeatedPositions, int iAnalogInput);

        /// <summary>
        /// Sets the closed-loop acceleration
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_ACC", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_ACC(int ID, string szAxes, IntPtr pdValueArray);

        /// <summary>
        /// Adds 2 values and saves the result to a variable
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_ADD", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_ADD(int ID, string szVariable, double value1, double value2);

        /// <summary>
        /// Sets an offset to the analog input
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_AOS", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_AOS(int ID, string szAxes, IntPtr pdValueArray);

        /// <summary>
        /// Starts the auto piezo gain calibration.
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_APG", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_APG(int ID, int[] piPIEZOWALKChannelsArray, int iArraySize);

        /// <summary>
        /// Starts an automatic calibration. 
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_ATC", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_ATC(int ID, string piChannels, int[] piValueArray, int iArraySize);

        /// <summary>
        /// Starts an automatic zero-point calibration.
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_ATZ", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_ATZ(int ID, string szAxes, double[] pdLowVoltageArray, bool[] pfUseDefaultArray);

        /// <summary>
        /// Sets the number of values for an average
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_AVG", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_AVG(int ID, int iAverageTime);

        /// <summary>
        /// Sets the brake state 
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_BRA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_BRA(int ID, string szAxes, IntPtr pbValueArray);

        /// <summary>
        /// Sets the command level of the controller
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_CCL", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_CCL(int ID, int iCommandLevel, string szPassWord);

        /// <summary>
        /// Switches the communication protocol
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_CCT", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_CCT(int ID, int iCommandType);

        /// <summary>
        /// Selects the closed-loop control mode
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_CMO", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_CMO(int ID, string szAxes, int[] piValueArray);

        /// <summary>
        /// Copies a command response into a variable
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_CPY", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_CPY(int ID, string szVariable, string szCommand);


        /// <summary>
        /// Loads parameter values from a positioner database
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_CST", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_CST(int ID, string szAxes, string szNames);

        /// <summary>
        /// Configures the trigger input conditions
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_CloseAllDaisyChains", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_CTI(int ID, int[] piTriggerIdsArray, int[] piTriggerParameterArray, string szValueArray, int iArraySize);

        /// <summary>
        /// Configures the trigger output conditions
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_CTO", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_CTO(int ID, int[] piTriggerOutputIdsArray, int[] piTriggerParameterArray, double[] pdValueArray, int iArraySize);

        /// <summary>
        /// Configures the trigger output conditions
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_CTOString", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_CTOString(int ID, int[] piTriggerOutputIdsArray, int[] piTriggerParameterArray, string szValueArray, int iArraySize);

        /// <summary>
        /// Sets the target relative to a current closed-loop target
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_CTR", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_CTR(int ID, string szAxes, double[] pdValueArray);

        /// <summary>
        /// Sets an absolute closed-loop target
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_CTV", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_CTV(int ID, string szAxes, double[] pdValueArray);

        /// <summary>
        /// Sets the drift compensation mode for given axes
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_DCO", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_DCO();

        /// <summary>
        /// Sets the closed-loop deceleration
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_DEC", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_DEC(int ID, string szAxes, IntPtr pdValueArray);

        /// <summary>
        /// Delays the command interpreter
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_DEL", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_DEL(int ID, int iMilliSeconds);

        /// <summary>
        /// Sets digital output lines 
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_DIO", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_DIO(int ID, int[] piChannelsArray, bool[] pbValueArray, int iArraySize);

        /// <summary>
        /// Deletes a data file that was saved in the nonvolatile memory using 
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_DLT", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_DLT(int ID, string name);

        /// <summary>
        /// Resets parameters or settings to default values
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_DPA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_DPA(int ID, string szPassword, string szItems,
uint[] iParameterArray);

        /// <summary>
        /// Sets the data recorder configuration
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_DRC", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_DRC(int ID, int[] piRecordTableIdsArray, string szRecordSourceIds, int[] piRecordOptionArray);

        /// <summary>
        /// Sets the data recorder trigger source
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_DRT", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_DRT(int ID, int[] piRecordChannelIdsArray, int[] piTriggerSourceArray, string szValues, int iArraySize);

        /// <summary>
        /// Enables an axis 
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_EAX", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_EAX(int ID, string szAxes, bool[]
pbValueArray);

        /// <summary>
        /// Fast alignment: Defines a fast alignment gradient search routine
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_FDG", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_FDG(int ID, string szScanRoutineName, string
szScanAxis, string szStepAxis, string szParameters);

        /// <summary>
        /// Fast alignment: Defines a fast alignment area scan routine.
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_FDR", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_FDR(int ID, string szScanRoutineName, string
szScanAxis, double dScanAxisRange, string szStepAxis,
double dStepAxisRange, string szParameters);

        /// <summary>
        /// Find an edge 
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_FED", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_FED(int ID, string szAxes, int[] piEdgeArray, int[] piParamArray);

        /// <summary>
        /// Fast alignment: Changes the center position of gradient search routine
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_FGC", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_FGC(int ID, string szProcessIds, double[] pdScanAxisCenterValueArray, double[] pdStepAxisCenterValueArray);

        /// <summary>
        /// Starts a fast input-output alignment procedure
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_FIO", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_FIO(int ID, string szAxis1, double dLength1, string szAxis2, double dLength2, double dThreshold, double dLinearStep, double dAngleScan, int iAnalogInput);

        /// <summary>
        /// Starts a fast line scan to maximum
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_FLM", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_FLM(int ID, string szAxis, double dLength, double dThreshold, int iAnalogInput, int iDirection);

        /// <summary>
        /// Starts a fast line scan
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_FLS", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_FLS(int ID, string szAxis, double dLength, double dThreshold, int iAnalogInput, int iDirection);

        /// <summary>
        /// Starts a referencing move to the negative limit
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_FNL", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_FNL(int ID, string szAxes);

        /// <summary>
        /// Starts a phase finding process 
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_FPH", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_FPH(int ID, string szAxes);

        /// <summary>
        /// Starts a reference move to the positive limit
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_FPL", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_FPL(int ID, string szAxes);

        /// <summary>
        /// Fast alignment: Couples fast alignment routines to each other
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_FRC", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_FRC(int ID, string szProcessIdBase, string szProcessIdsCoupled);

        /// <summary>
        /// Starts a referencing move to the reference point
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_FRF", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_FRF(int ID, string szAxes);

        /// <summary>
        /// Fast alignment: Stops, pauses, or resumes a fast alignment routine
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_FRP", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_FRP(int ID, string szScanRoutineNames, int[] piOptionsArray);

        /// <summary>
        /// Fast alignment: Starts a fast alignment routine
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_FRS", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_FRS(int ID, string szScanRoutineNames);

        /// <summary>
        /// Starts a fast scan with automated alignment
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_FSA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_FSA(int ID, string szAxis1, double dLength1, string szAxis2, double dLength2, double dThreshold, double dDistance, double dAlignStep, int iAnalogInput);

        /// <summary>
        /// Starts a scanning procedure for a specified area with a defined threshold
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_FSC", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_FSC(int ID, string szAxis1, double dLength1, string szAxis2, double dLength2, double dThreshold, double dDistance, int iAnalogInput);

        /// <summary>
        /// Starts a find-surface procedure 
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_FSF", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_FSF(int ID, string szAxis, double forceValue1, double positionOffset, bool useForceValue2, double forceValue2);

        /// <summary>
        /// Starts a scanning procedure to determine the global maximum intensity
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_FSM", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_FSM(int ID, string szAxis1, double dLength1, string szAxis2, double dLength2, double dThreshold, double dDistance, int iAnalogInput);

        /// <summary>
        /// Gets the address of the internal buffer
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_GetAsyncBuffer", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_GetAsyncBuffer(int ID, IntPtr pdValueArray);

        /// <summary>
        /// Gets the index that is used for the internal buffer
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_GetAsyncBufferIndex", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_GetAsyncBufferIndex(int ID);

        /// <summary>
        /// Gets the free memory space for trajectory points
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_GetDynamicMoveBufferSize", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_GetDynamicMoveBufferSize(int ID, IntPtr iSize);

        /// <summary>
        /// Moves axes to their home position
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_GOH", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_GOH(int ID, string szAxes);

        /// <summary>
        /// Queries if axis positions changed
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_HasPosChanged", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_HasPosChanged(int ID, string szAxes, IntPtr pbValueArray);

        /// <summary>
        /// Assigns a lookup table to an HID device
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_HDT", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_HDT(int ID, int[] iDeviceIDsArray, int[] iAxisIDsArray, int[] piValueArray, int iArraySize);

        /// <summary>
        /// Configures the control by HID axes
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_HIA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_HIA(int ID, string szAxes, int[] iFunctionArray, int[] iDeviceIDsArray, int[] iAxesIDsArray);

        /// <summary>
        /// Sets the state of the HID output unit or characteristic
        /// </summary>
        /// <returns></returns>
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_HIL", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_HIL(int ID, int[] iDeviceIDsArray, int[] iLED_IDsArray, int[] pnValueArray, int iArraySize);


        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_HIN", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_HIN(int ID, string szAxes, bool[] pbValueArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_HIS", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_HIS(int ID, int[] iDeviceIDsArray, int[] iItemIDsArray, int[] iPropertyIDArray, string szValues, int iArraySize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_HIT", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_HIT(int ID, int[] piTableIdsArray, int[] piPointNumberArray, double[] pdValueArray, int iArraySize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_HLT", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_HLT(int ID, string szAxes);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_IFC", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_IFC(int ID, string szParameters, string szValues);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_IFS", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_IFS(int ID, string szPassword, string szParameters, string szValues);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_IMP", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_IMP(int ID, string szAxes, double[] pdImpulseSize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_IMP_PulseWidth", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_IMP_PulseWidth(int ID, char cAxis, double dOffset, int iPulseWidth);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_INI", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_INI(int ID, string szAxes);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_IsAvailable", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_IsAvailable(int ID);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_IsControllerReady", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_IsControllerReady(int ID, IntPtr piControllerReady);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_IsMoving", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_IsMoving(int ID, string szAxes, IntPtr pbValueArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_IsRunningMacro", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_IsRunningMacro(int ID, IntPtr pbRunningMacro);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_JAX", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_JAX(int ID, int iJoystickID, int iAxesID, string szAxesBuffer);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_JDT", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_JDT(int ID, int[] iJoystickIDsArray, int[] iAxesIDsArray, int[] piValueArray, int iArraySize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_JLT", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_JLT(int ID, int iJoystickID, int iAxisID, int iStartAdress, double[] pdValueArray, int iArraySize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_JON", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_JON(int ID, int[] iJoystickIDsArray, bool[] pbValueArray, int iArraySize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_KCP", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_KCP(int ID, string szSource, string szDestination);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_KEN", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_KEN(int ID, string szNameOfCoordSystem);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_KLD", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_KLD(int ID, string szNameOfCoordSystem, string szAxes, double[] pdValueArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_KLF", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_KLF(int ID, string szNameOfCoordSystem);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_KLN", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_KLN(int ID, string szNameOfChild, string szNameOfParent);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_KRM", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_KRM(int ID, string szNameOfCoordSystem);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_KSB", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_KSB(int ID, string szNameOfCoordSystem, string szAxes, double[] pdValueArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_KSD", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_KSD(int ID, string szNameOfCoordSystem, string szAxes, double[] pdValueArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_KSF", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_KSF(int ID, string szNameOfCoordSystem);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_KST", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_KST(int ID, string szNameOfCoordSystem, string szAxes, double[] pdValueArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_KSW", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_KSW(int ID, string szNameOfCoordSystem, string szAxes, double[] pdValueArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_MAC_BEG", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_MAC_BEG(int ID, string szMacroName);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_MAC_DEF", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_MAC_DEF(int ID, string szMacroName);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_MAC_DEL", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_MAC_DEL(int ID, string szMacroName);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_MAC_END", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_MAC_END(int ID);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_MAC_NSTART", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_MAC_NSTART(int ID, string szMacroName, int nrRuns);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_MAC_NSTART_Args", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_MAC_NSTART_Args(int ID, string szMacroName, int nrRuns, string szArgs);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_MAC_qDEF", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_MAC_qDEF(int ID, IntPtr szBuffer, int iBufferSize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_MAC_qERR", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_MAC_qERR(int ID, IntPtr szBuffer, int iBufferSize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_MAC_qFREE", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_MAC_qFREE(int ID, IntPtr iFreeSpace);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_MAC_START", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_MAC_START(int ID, string szMacroName);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_MAC_START_Args", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_MAC_START_Args(int ID, string szMacroName, string szArgs);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_MEX", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_MEX(int ID, string szCondition);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_MOD", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_MOD(int ID, string szItems, uint[] iModeArray, string szValues);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_MOV", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_MOV(int ID, string szAxes, double[] pdValueArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_MRT", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_MRT(int ID, string szAxes, double[] pdValueArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_MRW", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_MRW(int ID, string szAxes, double[] pdValueArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_MVE", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_MVE(int ID, string szAxes, double[] pdValueArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_MVR", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_MVR(int ID, string szAxes, double[] pdValueArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_NAV", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_NAV(int ID, int[] piAnalogChannelIds, int[] piNrReadingsValues, int iArraySize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_NLM", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_NLM(int ID, string szAxes, double[] pdValueArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_OAC", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_OAC(int ID, int[] piPIEZOWALKChannelsArray, double[] pdValueArray, int iArraySize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_OAD", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_OAD(int ID, int[] piPIEZOWALKChannelsArray, double[] pdValueArray, int iArraySize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_OCD", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_OCD(int ID, int[] piPIEZOWALKChannelsArray, double[] pdValueArray, int iArraySize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_OCV", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_OCV(int ID, string axisContainerUnitsArray, double[] controlValueArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_ODC", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_ODC(int ID, int[] piPIEZOWALKChannelsArray, double[] pdValueArray, int iArraySize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_OMA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool VPI_OMA(int ID, string szAxes, double[] pdValueArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_OMR", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_OMR(int ID, string szAxes, double[] pdValueArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_ONL", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_ONL(int ID, int[] iPiezoChannels, int[] piValueArray, int iArraySize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_OSM", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_OSM(int ID, int[] piPIEZOWALKChannelsArray, int[] piValueArray, int iArraySize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_OSMf", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_OSMf(int ID, int[] piPIEZOWALKChannelsArray, double[] pdValueArray, int iArraySize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_OSMstringIDs", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_OSMstringIDs(int ID, string szAxisOrChannelIdy, double[] pdValueArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_OVL", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_OVL(int ID, int[] piPIEZOWALKChannelsArray, double[] pdValueArray, int iArraySize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_PGS", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_PGS(int ID, int[] piPIEZOWALKChannelsArray, int iArraySize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_PLM", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_PLM(int ID, string szAxes, double[] pdValueArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_POL", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_POL(int ID, string szAxes, int[] iValueArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_POS", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_POS(int ID, string szAxes, double[] pdValueArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qACC", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qACC(int ID, string szAxes, IntPtr pdValueArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qAOS", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qAOS(int ID, string szAxes, IntPtr pdValueArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qAPG", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qAPG(int ID, int[] piPIEZOWALKChannelsArray, IntPtr piValueArray, int iArraySize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qATC", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qATC(int ID, int[] piChannels, IntPtr piValueArray, int iArraySize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qATS", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qATS(int ID, int[] piChannels, int[] piOptions, IntPtr piValueArray, int iArraySize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qATZ", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qATZ(int ID, string szAxes, IntPtr piAtzResultArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qAVG", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qAVG(int ID, IntPtr iAverageTime);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qBRA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qBRA(int ID, string szAxes, IntPtr pbValueArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qCAV", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qCAV(int ID, string szAxes, IntPtr pdValueArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qCCL", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qCCL(int ID, IntPtr piComandLevel);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qCCT", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qCCT(int ID, IntPtr iCommandType);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qCCV", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qCCV(int ID, string szAxes, IntPtr pdValueArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qCMN", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qCMN(int ID, string szAxes, IntPtr pdValueArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qCMO", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qCMO(int ID, string szAxes, IntPtr piValueArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qCMX", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qCMX(int ID, string szAxes, IntPtr pdValueArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qCOV", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qCOV(int ID, int[] piChannelsArray, IntPtr pdValueArray, int iArraySize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qCST", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qCST(int ID, string szAxes, IntPtr szNames, int iBufferSize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qCSV", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qCSV(int ID, IntPtr pdCommandSyntaxVersion);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qCTI", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qCTI(int ID, int[] piTriggerIdsArray, int[] piTriggerParameterArray, IntPtr szValueArray, int iArraySize, int maxBufLen);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qCTO", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qCTO(int ID, int[] piTriggerOutputIdsArray, int[] piTriggerParameterArray, IntPtr pdValueArray, int iArraySize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qCTOString", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qCTOString(int ID, int[] piTriggerOutputIds, int[] piTriggerParameterArray, IntPtr szValueArray, int iArraySize, int maxBufLen);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qCTV", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qCTV(int ID, string szAxes, IntPtr pdValueArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qDCO", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qDCO(int ID, string szAxes, IntPtr pbValueArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qDEC", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qDEC(int ID, string szAxes, IntPtr pdValueArray);


        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qDFH", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qDFH(int ID, string szAxes, IntPtr pdValueArray);


        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qDIA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qDIA(int ID, uint[] iIDArray, IntPtr szValues, int iBufferSize, int iArraySize);


        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qDIO", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qDIO(int ID, int[] piChannelsArray, IntPtr pbValueArray, int iArraySize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qDRC", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qDRC(int ID, int[] piRecordTableIdsArray, IntPtr szRecordSourceIds, IntPtr piRecordOptionArray, int iRecordSourceIdsBufferSize, int iRecordOptionArraySize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qDRL", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qDRL(int ID, int[] piRecordChannelldsArray, IntPtr piNumberOfRecordedValuesArray, int iArraySize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qDRR", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qDRR(int ID, int[] piRecTableIdIdsArray, int iNumberOfRecChannels, int iOffsetOfFirstPointInRecordTable, int iNumberOfValues, IntPtr pdValueArray, IntPtr szGcsArrayHeader, int iGcsArrayHeaderMaxSize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qDRR_SYNC", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qDRR_SYNC(int ID, int iRecordTableId, int iOffsetOfFirstPointInRecordTable, int iNumberOfValues, IntPtr pdValueArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qDRT", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qDRT(int ID, int[] piRecordChannelIdsArray, IntPtr piTriggerSourceArray, IntPtr szValues, int iArraySize, int iValueBufferLength);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qEAX", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qEAX(int ID, string szAxes, IntPtr pbValueArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qECO", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qECO(int ID, string szSendString, IntPtr szValues, int iBufferSize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qERR", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qERR(int ID, IntPtr pnError);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qFGC", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qFGC(int ID, string szProcessIds, IntPtr pdScanAxisCenterValueArray, IntPtr pdStepAxisCenterValueArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qFPH", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qFPH(int ID, string szAxes, IntPtr pbValueArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qFRC", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qFRC(int ID, string szProcessIdsBase, IntPtr szBuffer, int iBufferSize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qFRF", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qFRF(int ID, string szAxes, IntPtr pbValueArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qFRH", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qFRH(int ID, IntPtr szBuffer, int iBufferSize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qFRP", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qFRP(int ID, string szScanRoutineNames, IntPtr piOptionsArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qFRR", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qFRR(int ID, string szScanRoutineNames, int iResultId, IntPtr szResult, int iBufferSize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qFRRArray", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qFRRArray(int ID, string szScanRoutineNames, int[] iResultIds, IntPtr szResult, int iBufferSize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qFSF", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qFSF(int ID, string szAxes, IntPtr pdForceValue1Array, IntPtr pdPositionOffsetArray, IntPtr pdForceValue2Array);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qFSR", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qFSR(int ID, string szAxes, IntPtr pbValueArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qFSS", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qFSS(int ID, IntPtr piResult);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qHAR", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qHAR(int ID, string szAxes, IntPtr pbValueArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qHDI", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qHDI(int ID, IntPtr szBuffer, int iBufferSize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qHDR", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qHDR(int ID, IntPtr szBuffer, int iBufferSize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qHDT", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qHDT(int ID, int[] iDeviceIDsArray, int[] iAxisIDsArray, IntPtr piValueArray, int iArraySize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qHIA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qHIA(int ID, string szAxes, int[] iFunctionArray, IntPtr iDeviceIDsArray, IntPtr iAxesIDsArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qHIB", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qHIB(int ID, int[] iDeviceIDsArray, int[] iButtonIDsArray, IntPtr pbValueArray, int iArraySize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qHIE", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qHIE(int ID, int[] iDeviceIDsArray, int[] iAxesIDsArray, IntPtr pdValueArray, int iArraySize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qHIL", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qHIL(int ID, int[] iDeviceIDsArray, int[] iLED_IDsArray, IntPtr pnValueArray, int iArraySize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qHIN", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qHIN(int ID, string szAxes, IntPtr pbValueArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qHIS", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qHIS(int ID, IntPtr szBuffer, int iBufferSize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qHIT", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qHIT(int ID, int[] piTableIdsArray, int iNumberOfTables, int iOffsetOfFirstPointInTable, int iNumberOfValues, IntPtr pdValueArray, IntPtr szGcsArrayHeader, int iGcsArrayHeaderMaxSize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qHLP", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qHLP(int ID, IntPtr szBuffer, int iBufferSize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qHPA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qHPA(int ID, IntPtr szBuffer, int iBufferSize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qHPV", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qHPV(int ID, IntPtr szBuffer, int iBufferSize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qIDN", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qIDN(int ID, IntPtr szBuffer, int iBufferSize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qIFC", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qIFC(int ID, string szParameters, IntPtr szBuffer, int iBufferSize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qIFS", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qIFS(int ID, string szParameters, IntPtr szBuffer, int iBufferSize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qIMP", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qIMP(int ID, string szAxes, IntPtr pdValueArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qIPR", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qIPR(int ID, IntPtr szBuffer, int iBufferSize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qJAS", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qJAS(int ID, int[] iJoystickIDsArray, int[] iAxesIDsArray, IntPtr pdValueArray, int iArraySize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qJAX", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qJAX(int ID, int[] iJoystickIDsArray, int[] iAxesIDsArray, int iArraySize, IntPtr szAxesBuffer, int iBufferSize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qJBS", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qJBS(int ID, int[] iJoystickIDsArray, int[] iButtonIDsArray, IntPtr pbValueArray, int iArraySize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qJLT", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qJLT(int ID, int[] iJoystickIDsArray, int[] iAxisIDsArray, int iNumberOfTables, int iOffsetOfFirstPointInTable, int iNumberOfValues, IntPtr pdValueArray, IntPtr szGcsArrayHeader, int iGcsArrayHeaderMaxSize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qJON", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qJON(int ID, int[] iJoystickIDsArray, IntPtr pbValarray, int iArraySize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qLST", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qLST(int ID, IntPtr buffer, int bufsize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qKEN", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qKEN(int ID, string szNamesOfCoordSystems, IntPtr buffer, int bufsize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qKET", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qKET(int ID, string szTypes, IntPtr buffer, int bufsize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qKLC", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qKLC(int ID, string szNameOfCoordSystem1, string szNameOfCoordSystem2, string szItem1, string szItem2, IntPtr buffer, int bufsize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qKLN", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qKLN(int ID, string szNamesOfCoordSystems, IntPtr buffer, int bufsize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qKLS", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qKLS(int ID, string szNameOfCoordSystem, string szItem1, string szItem2, IntPtr buffer, int bufsize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qKLT", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qKLT(int ID, string szStartCoordSystem, string szEndCoordSystem, IntPtr buffer, int bufsize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qLIM", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qLIM(int ID, string szAxes, IntPtr pbValueArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qLOG", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qLOG(int ID, int startIndex, IntPtr errorLog, int bufSize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qMAC", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qMAC(int ID, string szMacroName, IntPtr szBuffer, int iBufferSize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qMAN", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qMAN(int ID, string szCommand, IntPtr szBuffer, int iBufferSize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qMOD", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qMOD(int ID, string szItems, uint[] iModeArray, IntPtr szValues, int iMaxValuesSize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qMOV", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qMOV(int ID, string szAxes, IntPtr pdValueArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qNAV", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qNAV(int ID, int[] piAnalogChannelIds, IntPtr piNrReadingsValues, int iArraySize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qNLM", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qNLM(int ID, string szAxes, IntPtr pdValueArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qOAC", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qOAC(int ID, int[] piPIEZOWALKChannelsArray, IntPtr pdValueArray, int iArraySize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qOAD", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qOAD(int ID, int[] piPIEZOWALKChannelsArray, IntPtr pdValueArray, int iArraySize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qOCD", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qOCD(int ID, int[] piPIEZOWALKChannelsArray, IntPtr pdValueArray, int iArraySize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qOCV", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qOCV(int ID, string axisContainerUnit, IntPtr controlValueArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qODC", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qODC(int ID, int[] piPIEZOWALKChannelsArray, IntPtr pdValueArray, int iArraySize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qOMA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qOMA(int ID, string szAxes, IntPtr pdValueArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qONL", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qONL(int ID, int[] iPiezoChannels, IntPtr piValueArray, int iArraySize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qONT", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qONT(int ID, string szAxes, IntPtr pbValueArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qOSM", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qOSM(int ID, int[] piPIEZOWALKChannelsArray, IntPtr piValueArray, int iArraySize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qOSMf", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qOSMf(int ID, int[] piPIEZOWALKChannelsArray, IntPtr pdValueArray, int iArraySize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qOSMstringIDs", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qOSMstringIDs(int ID, string szAxisOrChannelId, IntPtr pdValueArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qOSN", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qOSN(int ID, int[] piPiezoWalkChannelsArray, IntPtr piValueArray, int iArraySize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qOSNstringIDs", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qOSNstringIDs(int ID, string szAxisOrChannelIds, IntPtr piValueArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qOVF", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qOVF(int ID, string szAxes, IntPtr piValueArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qOVL", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qOVL(int ID, int[] piPIEZOWALKChannelsArray, IntPtr pdValueArray, int iArraySize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qPLM", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qPLM(int ID, string szAxes, IntPtr pdValueArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qPOS", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qPOS(int ID, string szAxes, IntPtr pdValueArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qPUN", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qPUN(int ID, string szAxes, IntPtr szUnit, int iBufferSize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qREC_DAT", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qREC_DAT(int ID, string recorderId, string dataFormat, int offset, int numberOfValue, IntPtr traceIndices, int numberOfTraceIndices, IntPtr dataValues, IntPtr gcsArrayHeaderBuffer, int gcsArrayHeaderBufferSize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qREC_NUM", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qREC_NUM(int ID, string recorderIds, IntPtr numDataValues);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qREC_RATE", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qREC_RATE(int ID, string recorderIds, IntPtr rateValues);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qREC_STATE", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qREC_STATE(int ID, string recorderIds, IntPtr statesBuffer, int statesBufferSize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qREC_TRACE", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qREC_TRACE(int ID, string recorderId, int traceIndex, IntPtr traceConfigurationBuffer, int traceConfigurationBufferSize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qREC_TRG", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qREC_TRG(int ID, string recorderIds, IntPtr triggerConfigurationBuffer, int triggerConfigurationBufferSize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qRMC", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qRMC(int ID, IntPtr szBuffer, int iBufferSize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qRON", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qRON(int ID, string szAxes, IntPtr pbValueArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qRTD", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qRTD(int ID, int tableType, int tableID, int infoID, IntPtr buffer, int bufsize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qRTO", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qRTO(int ID, string szAxes, IntPtr piValueArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qRTR", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qRTR(int ID, IntPtr piRecordTableRate);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qSAI", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qSAI(int ID, IntPtr szAxes, int iBufferSize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qSAI_ALL", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qSAI_ALL(int ID, IntPtr szAxes, int iBufferSize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qSAM", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qSAM(int ID, string axisContainerUnit, IntPtr axesOperationModesArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qSCN", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qSCN(int ID, int[] piSensorsChannelsArray, IntPtr piValueArray, int iArraySize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qSCT", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qSCT(int ID, IntPtr pdCycleTime);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qSEP", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qSEP(int ID, string szItems, IntPtr iParameterArray, IntPtr pdValueArray, IntPtr szStrings, int iMaxNameSize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qSEP_int64", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qSEP_int64(int ID, string szItems, IntPtr iParameterArray, IntPtr piValueArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qSEP_String", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qSEP_String(int ID, string szItems, IntPtr iParameterArray, IntPtr szStrings, int iMaxNameSize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qSGA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qSGA(int ID, int[] piAnalogChannelIds, IntPtr piGainValues, int iArraySize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qSIC", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qSIC(int ID, int[] piFastAlignmentInputIdsArray, int iNumberOfInputIds, IntPtr szBuffer, int iBufferSize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qSMO", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qSMO(int ID, string szAxes, IntPtr piValueArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qSMR", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qSMR(int ID, string axisContainerUnit, IntPtr remainingSteps);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qSMV", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qSMV(int ID, string axisContainerUnit, IntPtr commandedSteps);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qSPA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qSPA(int ID, string szItems, IntPtr iParameterArray, IntPtr pdValueArray, IntPtr szStrings, int iMaxNameSize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qSPA_int64", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qSPA_int64(int ID, string szItems, IntPtr iParameterArray, IntPtr piValueArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qSPA_String", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qSPA_String(int ID, string szItems, uint[] iParameterArray, IntPtr szStrings, int iMaxNameSize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qSPI", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qSPI(int ID, string szAxes, IntPtr pdValueArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qSPV", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qSPV(int ID, string memType, string containerUnit, string functionUnit, string parameter, IntPtr answer, int bufsize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qSPV_Double", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qSPV_Double(int ID, string memType, string containerUnit, string functionUnit, string parameter, IntPtr value);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qSPV_Int32", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qSPV_Int32(int ID, string memType, string containerUnit, string functionUnit, string parameter, IntPtr value);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qSPV_Int64", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qSPV_Int64(int ID, string memType, string containerUnit, string functionUnit, string parameter, IntPtr value);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qSPV_String", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qSPV_String(int ID, string memType, string containerUnit, string functionUnit, string parameter, IntPtr value, int bufSize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qSPV_Uint32", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qSPV_Uint32(int ID, string memType, string containerUnit, string functionUnit, string parameter, IntPtr value);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qSPV_Uint64", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qSPV_Uint64(int ID, string memType, string containerUnit, string functionUnit, string parameter, IntPtr value);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qSRG", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qSRG(int ID, string szAxes, int[] iRegisterArray, IntPtr iValArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qSSA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qSSA(int ID, int[] piPIEZOWALKChannels, IntPtr pdValueArray, int iArraySize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qSSL", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qSSL(int ID, string szAxes, IntPtr pbValueArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qSSN", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qSSN(int ID, IntPtr szSerialNumber, int iBufferSize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qSST", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qSST(int ID, string szAxes, IntPtr pdValueArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qSTE", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qSTE(int ID, string szAxes, IntPtr pdValueArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qSTV", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qSTV(int ID, string containerUnit, IntPtr statusArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qSVA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qSVA(int ID, string szAxes, IntPtr pdValueArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qSVO", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qSVO(int ID, string szAxes, IntPtr pbValueArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qTAC", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qTAC(int ID, IntPtr pnNrChannels);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qTAD", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qTAD(int ID, int[] piSensorsChannelsArray, IntPtr piValueArray, int iArraySize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qTAV", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qTAV(int ID, int[] piChannelsArray, IntPtr pdValueArray, int iArraySize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qTCI", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qTCI(int ID, int[] piFastAlignmentInputIDsArray, IntPtr pdCalculatedInputValueArray, int iArraySize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qTCV", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qTCV(int ID, string szAxes, IntPtr pdValueArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qTGL", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qTGL(int ID, int[] piTrajectoriesArray, IntPtr iTrajectorySizesArray, int iArraySize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qTGT", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qTGT(int ID, IntPtr iTrajectoryTiming);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qTIM", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qTIM(int ID, IntPtr pdTimer);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qTIO", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qTIO(int ID, IntPtr piInputNr, IntPtr piOutputNr);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qTMN", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qTMN(int ID, string szAxes, IntPtr pdValueArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qTMX", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qTMX(int ID, string szAxes, IntPtr pdValueArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qTNR", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qTNR(int ID, IntPtr piNumberOfRecordChannels);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qTNS", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qTNS(int ID, int[] piSensorsChannelsArray, IntPtr pdValueArray, int iArraySize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qTPC", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qTPC(int ID, IntPtr piNumberOfPiezoChannels);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qTRA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qTRA(int ID, string szAxes, double[] pdComponents, IntPtr pdValueArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qTRI", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qTRI(int ID, int[] piTriggerChannelIds, IntPtr pbTriggerChannelEnabel, int iArraySize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qTRO", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qTRO(int ID, int[] piTriggerChannelIds, IntPtr pbTriggerChannelEnabel, int iArraySize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qTRS", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qTRS(int ID, string szAxes, IntPtr pbValueArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qTSC", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qTSC(int ID, IntPtr piNumberOfSensorChannels);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qTSP", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qTSP(int ID, int[] piSensorsChannelsArray, IntPtr pdValueArray, int iArraySize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qTVI", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qTVI(int ID, IntPtr szBuffer, int iBufferSize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qUCL", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qUCL(int ID, IntPtr userCommandLevel, int bufSize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qUSG", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qUSG(int ID, IntPtr usg, int bufSize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qUSG_CMD", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qUSG_CMD(int ID, string chapter, IntPtr usg, int bufSize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qUSG_HW", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qUSG_HW(int ID, string chapter, IntPtr usg, int bufSize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qUSG_PAM", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qUSG_PAM(int ID, string chapter, IntPtr usg, int bufSize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qUSG_PROP", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qUSG_PROP(int ID, string chapter, IntPtr usg, int bufSize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qUSG_SYS", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qUSG_SYS(int ID, string chapter, IntPtr usg, int bufSize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qVAR", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qVAR(int ID, string szVariables, IntPtr szValues, int iBufferSize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qVCO", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qVCO(int ID, IntPtr szAxes, IntPtr pbValueArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qVEL", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qVEL(int ID, string szAxes, IntPtr pdValueArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qVER", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qVER(int ID, IntPtr szBuffer, int iBufferSize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qVLS", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qVLS(int ID, IntPtr pdSystemVelocity);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qVMA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qVMA(int ID, int[] piPiezoChannelsArray, IntPtr pdValueArray, int iArraySize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qVMI", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qVMI(int ID, int[] piPiezoChannelsArray, IntPtr pdValueArray, int iArraySize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qVMO", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qVMO(int ID, string szAxes, double[] pdValarray, IntPtr pbMovePossible);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qVOL", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qVOL(int ID, int[] piPiezoChannelsArray, IntPtr pdValueArray, int iArraySize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_qVST", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_qVST(int ID, IntPtr szBuffer, int iBufferSize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_RBT", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_RBT(int ID);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_REC_RATE", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_REC_RATE(int ID, string recorderId, int rate);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_REC_START", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_REC_START(int ID, string recorderIds);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_REC_STOP", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_REC_STOP(int ID, string recorderIds);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_REC_TRACE", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_REC_TRACE(int ID, string recorderId, int traceId, string containerUnitId, string functionUnitId, string parameterId);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_REC_TRG", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_REC_TRG(int ID, string recorderId, string triggerMode, string triggerOption1, string triggerOption2);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_REP", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_REP(int ID);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_RES", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_RES(int ID, string axisContainerUnit);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_RNP", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_RNP(int ID, int[] piPIEZOWALKChannelsArray, double[] pdValueArray, int iArraySize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_RON", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_RON(int ID, string szAxes, bool[] pbValueArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_RPA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_RPA(int ID, string szItems, uint[] iParameterArray);
        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_RTD", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_RTD(int ID, int tableType, int tableID, string name);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_RTO", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_RTO(int ID, string szAxes);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_RTR", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_RTR(int ID, int piRecordTableRate);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_SAI", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_SAI(int ID, string szOldAxes, string szNewAxes);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_SAM", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_SAM(int ID, string axisContainerUnit, uint axisOperationMode);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_SCN", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_SCN(int ID, int[] piSensorsChannelsArray, int[] piValueArray, int iArraySize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_SCT", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_SCT(int ID, double dCycleTime);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_SEP", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_SEP(int ID, string szPassword, string szItems, uint[] iParameterArray, double[] pdValueArray, string szStrings);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_SEP_int64", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_SEP_int64(int ID, string szPassword, string szItems, uint[] iParameterArray, Int64[] piValueArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_SEP_String", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_SEP_String(int ID, string szPassword, string szItems, uint[] iParameterArray, string szStrings);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_SGA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_SGA(int ID, int[] piAnalogChannelIds, int[] piGainValues, int iArraySize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_SIC", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_SIC(int ID, int iFastAlignmentInputId, int iCalcType, double[] pdParameters, int iNumberOfParameters);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_SMO", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_SMO(int ID, string szAxes, int[] piValueArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_SMV", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_SMV(int ID, string axisContainerUnitsArray, double[] numberOfStepsArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_SPA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_SPA(int ID, string szItems, uint[] iParameterArray, double[] pdValueArray, string szStrings);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_SPA_int64", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_SPA_int64(int ID, string szItems, uint[] iParameterArray, Int64[] piValueArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_SPA_String", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_SPA_String(int ID, string szItems, uint[] iParameterArray, string szStrings);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_SPI", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_SPI(int ID, string szAxes, double[] pdValueArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_SPV", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_SPV(int ID, string memType, string containerUnit, string functionUnit, string parameter, int value);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_SPV_Double", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_SPV_Double(int ID, string memType, string containerUnit, string functionUnit, string parameter, double value);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_SPV_Int32", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_SPV_Int32(int ID, string memType, string containerUnit, string functionUnit, string parameter, int value);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_SPV_Int64", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_SPV_Int64(int ID, string memType, string containerUnit, string functionUnit, string parameter, int value);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_SPV_String", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_SPV_String(int ID, string memType, string containerUnit, string functionUnit, string parameter, string value);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_SPV_Uint32", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_SPV_Uint32(int ID, string memType, string containerUnit, string functionUnit, string parameter, uint value);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_SPV_Uint64", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_SPV_Uint64(int ID, string memType, string containerUnit, string functionUnit, string parameter, ulong value);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_SSA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_SSA(int ID, int[] piPIEZOWALKChannelsArray, double[] pdValueArray, int iArraySize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_SSL", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_SSL(int ID, string szAxes, bool[] pbValueArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_SST", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_SST(int ID, string szAxes, double[] pdValueArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_STD", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_STD(int ID, int tableType, int tableID, string data);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_STE", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_STE(int ID, string szAxes, double[] dOffsetArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_STF", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_STF(int ID);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_StopAll", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_StopAll(int ID);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_STP", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_STP(int ID);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_SVA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_SVA(int ID, string szAxes, IntPtr pdValueArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_SVO", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_SVO(int ID, string szAxes, bool[] pbValueArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_SVR", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_SVR(int ID, string szAxes, double[] pdValueArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_TGA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_TGA(int ID, int[] piTrajectoriesArray, double[] pdValueArray, int iArraySize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_TGC", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_TGC(int ID, int[] piTrajectoriesArray, int iArraySize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_TGF", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_TGF(int ID, int[] piTrajectoriesArray, int iArraySize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_TGS", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_TGS(int ID, int[] piTrajectoriesArray, int iArraySize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_TGT", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_TGT(int ID, int iTrajectoryTiming);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_TIM", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_TIM(int ID, double dTimer);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_TRI", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_TRI(int ID, int[] piTriggerChannelIds, bool[] pbTriggerChannelEnabel, int iArraySize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_TRO", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_TRO(int ID, int[] piTriggerChannelIds, bool[] pbTriggerChannelEnabel, int iArraySize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_TSP", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_TSP(int ID, int[] piSensorsChannelsArray, double[] pdValueArray, int iArraySize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_UCL", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_UCL(int ID, string userCommandLevel, string password);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_VAR", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_VAR(int ID, string szVariables, string szValues);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_VCO", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_VCO(int ID, string szAxes, bool[] pbValueArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_VEL", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_VEL(int ID, string szAxes, double[] pdValueArray);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_VLS", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_VLS(int ID, double dSystemVelocity);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_VMA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_VMA(int ID, int[] piPiezoChannelsArray, double[] pdValueArray, int iArraySize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_VMI", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_VMI(int ID, int[] piPiezoChannelsArray, double[] pdValueArray, int iArraySize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_VOL", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_VOL(int ID, int[] piPiezoChannelsArray, double[] pdValueArray, int iArraySize);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_WAC", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_WAC(int ID, IntPtr szCondition);

        [DllImport("PI_GCS2_DLL.dll", EntryPoint = "PI_WPA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PI_WPA(int ID, string szPassword, string szItems, uint[] iParameterArray);

        #endregion


        #region C#接口

        /// <summary>
        /// 获取结果
        /// </summary>
        /// <returns></returns>
        public static List<byte> GetAnswer(int ID, out bool state)
        {
            int count = GetAnswerSize(ID);
            if (count == 0) { state = true; return new List<byte>(); }
            byte[] buffer = new byte[count];
            state = PI_GcsGetAnswer(ID, Marshal.UnsafeAddrOfPinnedArrayElement(buffer, 0), count);
            return buffer.ToList();
        }

        /// <summary>
        /// 获取结果
        /// </summary>
        /// <returns></returns>
        unsafe public static int GetAnswerSize(int ID)
        {
            int[] size = new int[1];
            PI_GcsGetAnswerSize(ID, Marshal.UnsafeAddrOfPinnedArrayElement(size, 0));
            return size[0];
        }

        /// <summary>
        /// 枚举可用的USB设备
        /// </summary>
        /// <returns></returns>
        public static List<string> EnumerateUSB()
        {
            byte[] size = new byte[10000];
            PI_EnumerateUSB(Marshal.UnsafeAddrOfPinnedArrayElement(size, 0), 10000, "");
            Encoding coder = Encoding.ASCII;
            string result = coder.GetString(size);
            result = result.Replace("\0", "");
            string[] lis = result.Split(new char[] { '\n', '\t', '\r' });
            List<string> values = new List<string>();
            foreach (var item in lis)
            {
                if (item.Trim() != "")
                {
                    values.Add(item.Trim());
                }
            }
            return values;
        }

        #endregion
    }
}
