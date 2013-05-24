using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WindowsInput;
using System.Diagnostics;
using RemoteServer.Properties;

namespace RemoteServer
{
	public static class RequestHandler
	{
		public static Dictionary<string, VirtualKeyCode> SingleMediaKeys = new Dictionary<string, VirtualKeyCode>
		{
			{"KEY_PLAY", VirtualKeyCode.PLAY},
			{"KEY_PAUSE", VirtualKeyCode.PAUSE},
			{"KEY_STOP", VirtualKeyCode.MEDIA_STOP},
			{"KEY_REWIND", VirtualKeyCode.MEDIA_PREV_TRACK},
			{"KEY_FORWARD", VirtualKeyCode.MEDIA_NEXT_TRACK},
			{"KEY_MUTE", VirtualKeyCode.VOLUME_MUTE}
		};

		public static Dictionary<string, VirtualKeyCode> MultipleMediaKeys = new Dictionary<string, VirtualKeyCode>
		{
			{"KEY_VOLUMEUP", VirtualKeyCode.VOLUME_UP},
			{"KEY_VOLUMEDOWN", VirtualKeyCode.VOLUME_DOWN}
		};

		public static Dictionary<string, string> WinampLists = new Dictionary<string, string>
		{
			{"KEY_1", "enigma"},
			{"KEY_2", "poets"}
		};

		public static Dictionary<string, Action> SingleHandlers = new Dictionary<string, Action>
		{
			{"KEY_MENU3", Test}
		};

		public static Dictionary<string, Action<int>> MultipleHandlers = new Dictionary<string, Action<int>>
		{
			{"KEY_MENU2", Test2}
		};

		public static string HandleKeypress(string key, string s_count)
		{
			if (key == null) return "empty-key";
			Log("Keypress: " + key);
			int count = String.IsNullOrEmpty(s_count) ? 1 : int.Parse(s_count);

			if (count == 1)
			{
				SingleMediaKeys.Apply(key, InputSimulator.SimulateKeyPress);
				WinampLists.Apply(key, val => Process.Start(Settings.WinampPath, String.Format(@"{0}\{1}.m3u", Settings.WinampPlaylistPath, val)));
				SingleHandlers.Apply(key, act => act());
			}
			
			MultipleMediaKeys.Apply(key, InputSimulator.SimulateKeyPress);
			MultipleHandlers.Apply(key, act => act(count));

			return "ok";
		}

		public static void Test()
		{			
			MessageBox.Show("Test-key");
		}

		public static void Test2(int count)
		{
			MessageBox.Show("Test-key 2 - " + count);
		}

		
		private static Settings Settings { get { return Properties.Settings.Default; } }
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
	}
}
