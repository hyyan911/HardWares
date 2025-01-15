using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HardWares.端口基类
{
    /// <summary>
    /// 串口队列
    /// </summary>
    internal class PortDispatcher
    {
        /// <summary>
        /// 信息队列
        /// </summary>
        internal Queue<object> Queue { get; set; } = new Queue<object>();

        internal Queue<object> Prior_Queue { get; set; } = new Queue<object>();

        internal PortObject Parent { get; set; } = null;


        private int abort_flag = 0;
        /// <summary>
        /// 终止标志
        /// </summary>
        internal bool Abort_Flag
        {
            get { return Thread.VolatileRead(ref abort_flag) == 0 ? false : true; }

            set
            {
                Thread.VolatileWrite(ref abort_flag, value ? 1 : 0);
            }
        }

        internal Thread Sender = null;

        internal Thread Receiver = null;

        internal Encoding Coder = null;

        public PortDispatcher(PortObject parent, Encoding Coder)
        {
            Parent = parent;
            this.Coder = Coder;

            Sender = new Thread(() =>
            {
                while (Abort_Flag == false)
                {
                    if (parent.IsOpen())
                    {
                        //写
                        if (Prior_Queue.Count != 0)
                        {
                            lock (Prior_Queue)
                            {
                                object obj = Prior_Queue.Dequeue();
                                if (obj is string)
                                {
                                    parent.PortWrite(Coder.GetBytes(obj as string));
                                }
                                if (obj is List<byte>)
                                {
                                    parent.PortWrite((obj as List<byte>).ToArray());
                                }
                            }
                            continue;
                        }
                        if (Queue.Count != 0)
                        {
                            try
                            {
                                lock (Queue)
                                {
                                    object obj = Queue.Dequeue();
                                    if (obj is string)
                                    {
                                        parent.PortWrite(Coder.GetBytes(obj as string));
                                    }
                                    if (obj is List<byte>)
                                    {
                                        parent.PortWrite((obj as List<byte>).ToArray());
                                    }
                                }
                            }
                            catch (Exception) { }
                        }
                    }
                    Thread.Sleep(20);
                }
            });
            Sender.Start();

            Receiver = new Thread(() =>
            {
                while (Abort_Flag == false)
                {
                    if (parent.IsOpen())
                    {
                        try
                        {
                            byte[] seg = parent.PortRead();
                            lock (Parent.ReceiveBuffer)
                            {
                                Parent.ReceiveBuffer.AddRange(seg.ToList());
                                Parent.ReceiveAct();
                            }
                        }
                        catch (Exception e) { }
                    }
                    Thread.Sleep(20);
                }
            });
            Receiver.Start();
        }

        /// <summary>
        /// 添加信息到队列
        /// </summary>
        internal void AddMessage(string message)
        {
            lock (Queue)
            {
                Queue.Enqueue(message);
            }
        }

        /// <summary>
        /// 添加信息到队列
        /// </summary>
        internal void AddMessage(List<byte> message)
        {
            lock (Queue)
            {
                Queue.Enqueue(message);
            }
        }

        /// <summary>
        /// 添加最高优先级信息
        /// </summary>
        internal void AddProierMessage(string message)
        {
            lock (Prior_Queue)
            {
                Prior_Queue.Enqueue(message);
            }
        }

        /// <summary>
        /// 添加最高优先级信息
        /// </summary>
        internal void AddProierMessage(List<byte> message)
        {
            lock (Prior_Queue)
            {
                Prior_Queue.Enqueue(message);
            }
        }

        /// <summary>
        /// 清理队列
        /// </summary>
        internal void ClearQueue()
        {
            Queue.Clear();
            Prior_Queue.Clear();
        }

        /// <summary>
        /// 关闭并释放线程
        /// </summary>
        internal void Close()
        {
            if (Receiver == null || Sender == null) return;
            Abort_Flag = true;
            while (Receiver.ThreadState == ThreadState.Running || Sender.ThreadState == ThreadState.Running)
            {
                Thread.Sleep(5);
            }
            Sender = null;
            Receiver = null;
        }
    }
}
