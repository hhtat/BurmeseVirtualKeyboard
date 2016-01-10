using System;
using System.Runtime.InteropServices;

namespace BurmeseVirtualKeyboard
{
  internal class NativeMethods
  {
    [Flags]
    internal enum WindowStyle : int
    {
      None = 0,
      WS_EX_ACCEPTFILES = 0x00000010,
      WS_EX_APPWINDOW = 0x00040000,
      WS_EX_CLIENTEDGE = 0x00000200,
      WS_EX_COMPOSITED = 0x02000000,
      WS_EX_CONTEXTHELP = 0x00000400,
      WS_EX_CONTROLPARENT = 0x00010000,
      WS_EX_DLGMODALFRAME = 0x00000001,
      WS_EX_LAYERED = 0x00080000,
      WS_EX_LAYOUTRTL = 0x00400000,
      WS_EX_LEFT = 0x00000000,
      WS_EX_LEFTSCROLLBAR = 0x00004000,
      WS_EX_LTRREADING = 0x00000000,
      WS_EX_MDICHILD = 0x00000040,
      WS_EX_NOACTIVATE = 0x08000000,
      WS_EX_NOINHERITLAYOUT = 0x00100000,
      WS_EX_NOPARENTNOTIFY = 0x00000004,
      WS_EX_NOREDIRECTIONBITMAP = 0x00200000,
      WS_EX_OVERLAPPEDWINDOW = WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE,
      WS_EX_PALETTEWINDOW = WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST,
      WS_EX_RIGHT = 0x00001000,
      WS_EX_RIGHTSCROLLBAR = 0x00000000,
      WS_EX_RTLREADING = 0x00002000,
      WS_EX_STATICEDGE = 0x00020000,
      WS_EX_TOOLWINDOW = 0x00000080,
      WS_EX_TOPMOST = 0x00000008,
      WS_EX_TRANSPARENT = 0x00000020,
      WS_EX_WINDOWEDGE = 0x00000100,
    }

    internal enum WindowLong : int
    {
      GWL_WNDPROC = -4,
      GWL_HINSTANCE = -6,
      GWL_ID = -12,
      GWL_STYLE = -16,
      GWL_EXSTYLE = -20,
      GWL_USERDATA = -21,
    }

    internal enum InputType : uint
    {
      INPUT_MOUSE = 0,
      INPUT_KEYBOARD = 1,
      INPUT_HARDWARE = 2,
    }

    [Flags]
    internal enum KeyboardInputFlags : uint
    {
      None = 0,
      KEYEVENTF_EXTENDEDKEY = 0x00000001,
      KEYEVENTF_KEYUP = 0x00000002,
      KEYEVENTF_UNICODE = 0x00000004,
      KEYEVENTF_SCANCODE = 0x00000008,
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct Input
    {
      [FieldOffset(0)]
      public InputType type;
      [FieldOffset(4)]
      public MouseInput mi;
      [FieldOffset(4)]
      public KeyboardInput ki;
      [FieldOffset(4)]
      public HardwareInput hi;
    }

    internal struct MouseInput
    {
      public int dx;
      public int dy;
      public MouseInputData mouseData;
      public MouseInputFlags dwFlags;
      public uint time;
      public UIntPtr dwExtraInfo;
    }

    internal enum MouseInputData : uint
    {
      Zero = 0,
      XBUTTON1 = 0x00000001,
      XBUTTON2 = 0x00000002,
    }

    [Flags]
    internal enum MouseInputFlags : uint
    {
      None = 0,
      MOUSEEVENTF_ABSOLUTE = 0x00008000,
      MOUSEEVENTF_HWHEEL = 0x00001000,
      MOUSEEVENTF_MOVE = 0x00000001,
      MOUSEEVENTF_MOVE_NOCOALESCE = 0x00002000,
      MOUSEEVENTF_LEFTDOWN = 0x00000002,
      MOUSEEVENTF_LEFTUP = 0x00000004,
      MOUSEEVENTF_RIGHTDOWN = 0x00000008,
      MOUSEEVENTF_RIGHTUP = 0x00000010,
      MOUSEEVENTF_MIDDLEDOWN = 0x00000020,
      MOUSEEVENTF_MIDDLEUP = 0x00000040,
      MOUSEEVENTF_VIRTUALDESK = 0x00004000,
      MOUSEEVENTF_WHEEL = 0x00000800,
      MOUSEEVENTF_XDOWN = 0x00000080,
      MOUSEEVENTF_XUP = 0x00000100,
    }

    internal struct KeyboardInput
    {
      public ushort wVk;
      public ushort wScan;
      public KeyboardInputFlags dwFlags;
      public uint time;
      public UIntPtr dwExtraInfo;
    }

    internal struct HardwareInput
    {
      public uint uMsg;
      public short wParamL;
      public short wParamH;
    }

    [DllImport("user32.dll", SetLastError = true)]
    internal static extern WindowStyle GetWindowLong(IntPtr hWnd, WindowLong nIndex);

    [DllImport("user32.dll", SetLastError = true)]
    internal static extern int SetWindowLong(IntPtr hWnd, WindowLong nIndex, WindowStyle dwNewLong);

    [DllImport("user32.dll", SetLastError = true)]
    internal static extern uint SendInput(uint nInputs, Input[] pInputs, int cbSize);
  }
}
