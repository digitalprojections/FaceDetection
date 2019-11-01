using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FaceDetection
{
    public class MouseListener
    {
        #region Constant, Structures, and Delegate Definitions
        public delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);
        public LowLevelMouseProc mouseProc;

        private enum MouseMessages
        {
            WM_LBUTTONDOWN = 0x0201,
            WM_LBUTTONUP = 0x0202,
            WM_MOUSEMOVE = 0x0200,
            WM_MOUSEWHEEL = 0x020A,
            WM_RBUTTONDOWN = 0x0204,
            WM_RBUTTONUP = 0x0205
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MSLLHOOKSTRUCT
        {
            public POINT pt;
            public uint mouseData;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }
        #endregion

        #region Fields
        private IntPtr hhook = IntPtr.Zero;

        private const int WH_MOUSE_LL = 14;
        #endregion

        #region Events
        public event MouseEventHandler MouseLeftDown;
        public event MouseEventHandler MouseLeftUp;
        public event MouseEventHandler MouseRightDown;
        public event MouseEventHandler MouseRightUp;
        public event MouseEventHandler MouseMove;
        #endregion

        #region Constructors and Destructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MouseListener"/> class and installs the keyboard hook.
        /// </summary>
        public MouseListener()
        {
            Hook();
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="MouseListener"/> is reclaimed by garbage collection and uninstalls the keyboard hook.
        /// </summary>
        ~MouseListener()
        {
            Unhook();
        }
        #endregion

        #region Public Methods
        public void Hook()
        {
            mouseProc = HookCallback;
            IntPtr hInstance = LoadLibrary("User32");
            hhook = SetWindowsHookEx(WH_MOUSE_LL, mouseProc, hInstance, 0);
        }

        public void Unhook()
        {
            UnhookWindowsHookEx(hhook);
        }
        #endregion

        #region Methods
        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
                MouseEventArgs mea = null;
                switch((MouseMessages)wParam)
                {
                    case MouseMessages.WM_LBUTTONDOWN:
                        if(MouseLeftDown != null)
                        {
                            mea = new MouseEventArgs(MouseButtons.Left, 1, hookStruct.pt.x, hookStruct.pt.y, 0);
                            MouseLeftDown(this, mea);
                        }
                        break;
                    case MouseMessages.WM_LBUTTONUP:
                        if (MouseLeftUp != null)
                        {
                            mea = new MouseEventArgs(MouseButtons.Left, 1, hookStruct.pt.x, hookStruct.pt.y, 0);
                            MouseLeftUp(this, mea);
                        }
                        break;
                    case MouseMessages.WM_RBUTTONDOWN:
                        if (MouseRightDown != null)
                        {
                            mea = new MouseEventArgs(MouseButtons.Right, 1, hookStruct.pt.x, hookStruct.pt.y, 0);
                            MouseRightDown(this, mea);
                        }
                        break;
                    case MouseMessages.WM_RBUTTONUP:
                        if (MouseRightUp != null)
                        {
                            mea = new MouseEventArgs(MouseButtons.Right, 1, hookStruct.pt.x, hookStruct.pt.y, 0);
                            MouseRightUp(this, mea);
                        }
                        break;
                    case MouseMessages.WM_MOUSEMOVE:
                        if (MouseMove != null)
                        {
                            mea = new MouseEventArgs(MouseButtons.None, 0, hookStruct.pt.x, hookStruct.pt.y, 0);
                            MouseMove(this, mea);
                        }
                        break;
                }
            }

            

            return CallNextHookEx(hhook, nCode, wParam, lParam);
        }
        #endregion

        


        #region DLL imports
        /// <summary>
        /// Sets the windows hook, do the desired event, one of hInstance or threadId must be non-null
        /// </summary>
        /// <param name="idHook">The id of the event you want to hook</param>
        /// <param name="lpfn">The callback.</param>
        /// <param name="hInstance">The handle you want to attach the event to, can be null</param>
        /// <param name="threadId">The thread you want to attach the event to, can be null</param>
        /// <returns>a handle to the desired hook</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hInstance, uint threadId);

        /// <summary>
        /// Unhooks the windows hook.
        /// </summary>
        /// <param name="hInstance">The hook handle that was returned from SetWindowsHookEx</param>
        /// <returns>True if successful, false otherwise</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hInstance);

        /// <summary>
        /// Calls the next hook.
        /// </summary>
        /// <param name="idHook">The hook id</param>
        /// <param name="nCode">The hook code</param>
        /// <param name="wParam">The wparam.</param>
        /// <param name="lParam">The lparam.</param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// Loads the library.
        /// </summary>
        /// <param name="lpFileName">Name of the library</param>
        /// <returns>A handle to the library</returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr LoadLibrary(string lpFileName);
        #endregion
    }
}