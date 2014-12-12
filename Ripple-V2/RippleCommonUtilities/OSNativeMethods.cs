// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved. 

// get past MouseData not being initialized warning...it needs to be there for p/invoke
#pragma warning disable 0649

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace RippleCommonUtilities
{
    internal struct MouseInput
    {
        public int X;
        public int Y;
        public uint MouseData;
        public uint Flags;
        public uint Time;
        public IntPtr ExtraInfo;
    }

    internal struct Input
    {
        public int Type;
        public MouseInput MouseInput;
    }

    public static class OSNativeMethods
    {
        public const int InputMouse = 0;

        public const int MouseEventMove = 0x01;
        public const int MouseEventLeftDown = 0x02;
        public const int MouseEventLeftUp = 0x04;
        public const int MouseEventRightDown = 0x08;
        public const int MouseEventRightUp = 0x10;
        public const int MouseEventAbsolute = 0x8000;
        public const int WHEEL = 0x00000800;

        private static bool lastLeftDown;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SendInput(uint numInputs, Input[] inputs, int size);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern void mouse_event(uint dwFlags, uint dx, uint dy, int dwData, uint dwExtraInfo);

        public static void UpScrolling()
        {
            mouse_event((int)(WHEEL), 0, 0, 600, 0);
        }

        public static void DownScrolling()
        {
            int x = -600;
            mouse_event((int)(WHEEL), 0, 0, x, 0);
        }

        public static void SendMouseInput(int positionX, int positionY, int maxX, int maxY, bool leftDown)
        {
            if (positionX > int.MaxValue)
                throw new ArgumentOutOfRangeException("positionX");
            if (positionY > int.MaxValue)
                throw new ArgumentOutOfRangeException("positionY");

            Input[] i = new Input[2];

            // move the mouse to the position specified
            i[0] = new Input();
            i[0].Type = InputMouse;
            i[0].MouseInput.X = (positionX * 65535) / maxX;
            i[0].MouseInput.Y = (positionY * 65535) / maxY;
            i[0].MouseInput.Flags = MouseEventAbsolute | MouseEventMove;

            // determine if we need to send a mouse down or mouse up event
            if (!lastLeftDown && leftDown)
            {
                i[1] = new Input();
                i[1].Type = InputMouse;
                i[1].MouseInput.Flags = MouseEventLeftDown;
            }
            else if (lastLeftDown && !leftDown)
            {
                i[1] = new Input();
                i[1].Type = InputMouse;
                i[1].MouseInput.Flags = MouseEventLeftUp;
            }

            lastLeftDown = leftDown;

            // send it off
            uint result = SendInput(2, i, Marshal.SizeOf(i[0]));
            if (result == 0)
                throw new Win32Exception(Marshal.GetLastWin32Error());
        }
    }
}
