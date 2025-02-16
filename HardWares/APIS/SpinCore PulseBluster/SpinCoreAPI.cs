using Controls.Shapes;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows;
using NationalInstruments.DAQmx;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using static Thorlabs.MotionControl.PrivateInternal.LiquidCrystal.LiquidCrystal;

namespace HardWares.APIS
{
    internal class SpinCoreAPI
    {
        /// <summary>
        ///  Used to set the clock frequency of the board.  The variable clock_frequency is specified in MHz when no units are entered.Valid units are MHz, kHz, and Hz.
        /// </summary>
        /// <param name="thread"></param>
        /// <returns></returns>
        [DllImport("spinapi.dll", EntryPoint = "pb_core_clock", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern void pb_core_clock(double frequency);

        /// <summary>
        /// Used to initialize the system to receive programming information.  It accepts a parameter referencing the target for the instructions.The only valid value for device is 0.It returns a 0 on success or a negative number on an error.
        /// </summary>
        /// <param name="programtype"></param>
        /// <returns></returns>
        [DllImport("spinapi.dll", EntryPoint = "pb_start_programming", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern int pb_start_programming(int programtype = 0);

        /// <summary>
        ///  Initializes the PulseBlasterESR-PRO board.  Needs to be called before calling any functions using the device.It returns a 0 on success or a negative number on an error.
        /// </summary>
        /// <param name="thread"></param>
        /// <returns></returns>
        [DllImport("spinapi.dll", EntryPoint = "pb_init", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern int pb_init();

        /// <summary>
        /// Releases the PulseBlaster ESR-PRO board.  Needs to be called as last command in pulse program. It returns a 0 on success or a negative number on an error.
        /// </summary>
        /// <param name="thread"></param>
        /// <returns></returns>
        [DllImport("spinapi.dll", EntryPoint = "pb_close", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern int pb_close();

        /// <summary>
        /// Used to send one instruction of the pulse program.  Should only be called after pb_start_programming(PULSE_PROGRAM) has been called.It returns a negative number on an error, or the instruction number upon success.If the function returns –99, an invalid parameter was passed to the function.Instructions are numbered starting at 0.
        /// int flags – determines state of each TTL output bit.  Valid values are 0x000000 to 0xFFFFFF.  For 
        /// example, 0x000010 would correspond to bit 4 being on, and all other bits being off.
        ///int inst – determines which type of instruction is to be executed.Please see Table 8 for details.
        ///int inst_data – data to be used with the previous inst field.Please see Table 8 for details.
        ///double length – duration of this pulse program instruction, specified in nanoseconds(ns), 
        ///microseconds(us) or milliseconds(ms).
        /// </summary>
        /// <returns></returns>
        [DllImport("spinapi.dll", EntryPoint = "pb_inst", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern int pb_inst(int flags, int inst, int inst_data, double length);

        /// <summary>
        ///Return the number of SpinCore boards present in your system.
        ///return The number of boards present is returned. -1 is returned on error.
        /// </summary>
        /// <param name="flags"></param>
        /// <param name="inst"></param>
        /// <param name="inst_data"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        [DllImport("spinapi.dll", EntryPoint = "pb_count_boards", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern int pb_count_boards();

        /// <summary>
        /// If multiple boards from SpinCore Technologies are present in your system, this function allows you to select which board to talk to.Once this function
        ///is called, all subsequent commands (such as pb_init(), pb_core_clock(), etc.) will be
        /// sent to the selected board.You may change which board is selected at any time.
        /// If you have only one board, it is not necessary to call this function.
        ///param board_num Specifies which board to select.Counting starts at 0.
        ///return A negative number is returned on failure. 0 is returned on success
        /// </summary>
        /// <param name="flags"></param>
        /// <param name="inst"></param>
        /// <param name="inst_data"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        [DllImport("spinapi.dll", EntryPoint = "pb_select_board", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern int pb_select_board(int id);

        /// <summary>
        /// 停止编程
        /// </summary>
        /// <returns></returns>
        [DllImport("spinapi.dll", EntryPoint = "pb_stop_programming", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern int pb_stop_programming();
    }
}
