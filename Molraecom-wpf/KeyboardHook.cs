using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Molraecom_wpf
{
    public class KeyboardHook
    {
        // events
        public delegate void HookEventHandler(object sender, HookEventArgs e);

        private readonly HookProc _hookFunction;

        private readonly HookType _hookType = HookType.WH_KEYBOARD_LL;
        private IntPtr _hookHandle = IntPtr.Zero;

        public KeyboardHook()
        {
            _hookFunction = HookCallback;
            Install();
        }

        public event HookEventHandler KeyDown;
        public event HookEventHandler KeyUp;

        ~KeyboardHook()
        {
            Uninstall();
        }

        // hook function called by system
        private int HookCallback(int code, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam)
        {
            if (code < 0)
                return CallNextHookEx(_hookHandle, code, wParam, ref lParam);

            // KeyUp event
            if ((lParam.flags & 0x80) != 0 && KeyUp != null)
                KeyUp(this, new HookEventArgs(lParam.vkCode));

            // KeyDown event
            if ((lParam.flags & 0x80) == 0 && KeyDown != null)
                KeyDown(this, new HookEventArgs(lParam.vkCode));

            return CallNextHookEx(_hookHandle, code, wParam, ref lParam);
        }

        private void Install()
        {
            // make sure not already installed
            if (_hookHandle != IntPtr.Zero)
                return;

            // need instance handle to module to create a system-wide hook
            var list = Assembly.GetExecutingAssembly().GetModules();
            Debug.Assert(list != null && list.Length > 0);

            // install system-wide hook
            _hookHandle = SetWindowsHookEx(_hookType,
                _hookFunction, Marshal.GetHINSTANCE(list[0]), 0);
        }

        private void Uninstall()
        {
            if (_hookHandle != IntPtr.Zero)
            {
                // uninstall system-wide hook
                UnhookWindowsHookEx(_hookHandle);
                _hookHandle = IntPtr.Zero;
            }
        }

        // hook method called by system
        private delegate int HookProc(int code, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam);

        #region pinvoke details

        private enum HookType
        {
            WH_JOURNALRECORD = 0,
            WH_JOURNALPLAYBACK = 1,
            WH_KEYBOARD = 2,
            WH_GETMESSAGE = 3,
            WH_CALLWNDPROC = 4,
            WH_CBT = 5,
            WH_SYSMSGFILTER = 6,
            WH_MOUSE = 7,
            WH_HARDWARE = 8,
            WH_DEBUG = 9,
            WH_SHELL = 10,
            WH_FOREGROUNDIDLE = 11,
            WH_CALLWNDPROCRET = 12,
            WH_KEYBOARD_LL = 13,
            WH_MOUSE_LL = 14
        }

        public struct KBDLLHOOKSTRUCT
        {
            public uint vkCode;
            public uint scanCode;
            public uint flags;
            public uint time;
            public IntPtr extraInfo;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(
            HookType code, HookProc func, IntPtr instance, int threadID);

        [DllImport("user32.dll")]
        private static extern int UnhookWindowsHookEx(IntPtr hook);

        [DllImport("user32.dll")]
        private static extern int CallNextHookEx(
            IntPtr hook, int code, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam);

        #endregion
    }

// The callback method converts the low-level keyboard data into something more .NET friendly with the HookEventArgs class.

    public class HookEventArgs : EventArgs
    {
        public static bool Alt;

        public static bool Control;

        // using Windows.Forms.Keys instead of Input.Key since the Forms.Keys maps
        // to the Win32 KBDLLHOOKSTRUCT virtual key member, where Input.Key does not
        public static Keys Key;
        public bool Shift;

        public HookEventArgs(uint keyCode)
        {
            // detect what modifier keys are pressed, using 
            // Windows.Forms.Control.ModifierKeys instead of Keyboard.Modifiers
            // since Keyboard.Modifiers does not correctly get the state of the 
            // modifier keys when the application does not have focus
            Key = (Keys) keyCode;
            Alt = (System.Windows.Forms.Control.ModifierKeys & Keys.Alt) != 0;
            Control = (System.Windows.Forms.Control.ModifierKeys & Keys.Control) != 0;
            Shift = (System.Windows.Forms.Control.ModifierKeys & Keys.Shift) != 0;
        }
    }
}