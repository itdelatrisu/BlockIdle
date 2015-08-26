using System;
using System.Runtime.InteropServices;

namespace itdelatrisu.BlockIdle {
	/// <summary>
	/// Mouse simulator.
	/// http://stackoverflow.com/a/5098968
	/// </summary>
	public static class MouseSimulator {
		[DllImport("user32.dll", SetLastError = true)]
		private static extern uint SendInput(uint nInputs, ref INPUT pInputs, int cbSize);

		[StructLayout(LayoutKind.Sequential)]
		private struct INPUT {
			public SendInputEventType type;
			public MouseKeybdhardwareInputUnion mkhi;
		}

		[StructLayout(LayoutKind.Explicit)]
		private struct MouseKeybdhardwareInputUnion {
			[FieldOffset(0)]
			public MouseInputData mi;

			[FieldOffset(0)]
			public KEYBDINPUT ki;

			[FieldOffset(0)]
			public HARDWAREINPUT hi;
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct KEYBDINPUT {
			public ushort wVk;
			public ushort wScan;
			public uint dwFlags;
			public uint time;
			public IntPtr dwExtraInfo;
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct HARDWAREINPUT {
			public int uMsg;
			public short wParamL;
			public short wParamH;
		}

		private struct MouseInputData {
			public int dx;
			public int dy;
			public uint mouseData;
			public MouseEventFlags dwFlags;
			public uint time;
			public IntPtr dwExtraInfo;
		}

		[Flags]
		private enum MouseEventFlags : uint {
			MOUSEEVENTF_MOVE = 0x0001,
			MOUSEEVENTF_LEFTDOWN = 0x0002,
			MOUSEEVENTF_LEFTUP = 0x0004,
			MOUSEEVENTF_RIGHTDOWN = 0x0008,
			MOUSEEVENTF_RIGHTUP = 0x0010,
			MOUSEEVENTF_MIDDLEDOWN = 0x0020,
			MOUSEEVENTF_MIDDLEUP = 0x0040,
			MOUSEEVENTF_XDOWN = 0x0080,
			MOUSEEVENTF_XUP = 0x0100,
			MOUSEEVENTF_WHEEL = 0x0800,
			MOUSEEVENTF_VIRTUALDESK = 0x4000,
			MOUSEEVENTF_ABSOLUTE = 0x8000
		}

		private enum SendInputEventType : int {
			InputMouse,
			InputKeyboard,
			InputHardware
		}

		/// <summary>
		/// Moves the mouse.
		/// </summary>
		/// <param name="dx">The relative shift in the x position.</param>
		/// <param name="dy">The relative shift in the y position.</param>
		public static void MoveMouse(int dx, int dy) {
			INPUT input = new INPUT();
			input.type = SendInputEventType.InputMouse;
			input.mkhi.mi.dx = dx;
			input.mkhi.mi.dx = dy;
			input.mkhi.mi.mouseData = 0;
			input.mkhi.mi.dwFlags = MouseEventFlags.MOUSEEVENTF_MOVE;
			input.mkhi.mi.time = 0;
			input.mkhi.mi.dwExtraInfo = (IntPtr)0;

			SendInput(1, ref input, Marshal.SizeOf(new INPUT()));
		}
	}
}
