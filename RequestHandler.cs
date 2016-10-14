using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsInput;
using WindowsInput.Native;
using System.Diagnostics;
using RemoteServer.Properties;
using System.Globalization;

namespace RemoteServer
{
	public static class RequestHandler
	{
		/// <summary>
		/// Push and hold registers as a single key press
		/// </summary>
		public static Dictionary<string, VirtualKeyCode> SingleMediaKeys = new Dictionary<string, VirtualKeyCode>
		{
			{"KEY_PLAY", VirtualKeyCode.MEDIA_PLAY_PAUSE},
			{"KEY_PAUSE", VirtualKeyCode.MEDIA_PLAY_PAUSE},
			{"KEY_STOP", VirtualKeyCode.MEDIA_STOP},
			{"KEY_REWIND", VirtualKeyCode.MEDIA_PREV_TRACK},
			{"KEY_FORWARD", VirtualKeyCode.MEDIA_NEXT_TRACK},
			{"KEY_MUTE", VirtualKeyCode.VOLUME_MUTE},
			{"KEY_SLEEP", VirtualKeyCode.LAUNCH_APP2},
			{"KEY_WAKEUP", VirtualKeyCode.SPACE},
			{"KEY_OK", VirtualKeyCode.LBUTTON},
			{"KEY_MENU", VirtualKeyCode.RBUTTON}
		};

		/// <summary>
		/// Push and hold registers as a multiple key presses
		/// </summary>
		public static Dictionary<string, VirtualKeyCode> MultipleMediaKeys = new Dictionary<string, VirtualKeyCode>
		{
			{"KEY_VOLUMEUP", VirtualKeyCode.VOLUME_UP},
			{"KEY_VOLUMEDOWN", VirtualKeyCode.VOLUME_DOWN}
		};

		/// <summary>
		/// Custom key handlers
		/// </summary>
		public static Dictionary<string, Action> SingleHandlers = new Dictionary<string, Action>
		{
			{"KEY_OK", () => InputSimulator.Mouse.LeftButtonClick()},
			{"KEY_MENU", () => InputSimulator.Mouse.RightButtonClick()}
		};

		/// <summary>
		/// Custom key handlers
		/// </summary>
		public static Dictionary<string, Action<int>> MultipleHandlers = new Dictionary<string, Action<int>>
		{
			{"KEY_LEFT", MouseLeft},
			{"KEY_RIGHT", MouseRight},
			{"KEY_UP", MouseUp},
			{"KEY_DOWN", MouseDown}
		};

		public static Dictionary<string, string> WinampLists = new Dictionary<string, string>
		{
			{"KEY_1", "enigma"},
			{"KEY_2", "morcheeba"},
			{"KEY_0", "all"}
		};


		private static InputSimulator InputSimulator = new InputSimulator();

		public static string HandleKeypress(string key, string s_count)
		{
			if (key == null) return "empty-key";
			Log("Keypress: " + key);
			int count = String.IsNullOrEmpty(s_count) ? 1 : int.Parse(s_count, NumberStyles.HexNumber);
			
			if (count == 0)
			{
				SingleMediaKeys.Apply(key, InputSimulator.Keyboard.KeyPress);
				SingleHandlers.Apply(key, act => act());
				WinampLists.Apply(key, val => Process.Start(Settings.WinampPath, $@"{Settings.WinampPlaylistPath}\{val}.m3u"));
			}

			MultipleMediaKeys.Apply(key, InputSimulator.Keyboard.KeyPress);
			MultipleHandlers.Apply(key, act => act(count));

			return "ok";
		}

		public static void MoveMouse(int x, int y)
		{
			InputSimulator.Mouse.MoveMouseBy(x, y);
		}
		public static int Acceleration(int step)
		{
			return step * step;
		}
		public static void MouseLeft(int count)
		{
			MoveMouse(-1 * Acceleration(count), 0);
		}
		public static void MouseRight(int count)
		{
			MoveMouse(1 * Acceleration(count), 0);
		}
		public static void MouseUp(int count)
		{
			MoveMouse(0, -1 * Acceleration(count));
		}
		public static void MouseDown(int count)
		{
			MoveMouse(0, 1 * Acceleration(count));
		}

		
		private static Settings Settings { get { return Program.Settings; } }
		private static void Log(string message)
		{
			Program.Log(message);
		}
	}

	public static class Extensions
	{
		public static void Apply<T>(this Dictionary<string, T> dic, string key, Action<T> action)
		{
			if (dic.ContainsKey(key))
				action(dic[key]);
		}

		public static void Apply<T, U>(this Dictionary<string, T> dic, string key, Func<T, U> action)
		{
			if (dic.ContainsKey(key))
				action(dic[key]);
		}
	}
}
