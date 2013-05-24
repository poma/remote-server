using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CoreAudioApi;
using WindowsInput;

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

		public static Dictionary<string, Action> SingleHandlers = new Dictionary<string, Action>
		{
			{"KEY_MENU", Test}
		};

		public static Dictionary<string, Action<int>> MultipleHandlers = new Dictionary<string, Action<int>>
		{
			{"KEY_MENU2", Test2}
		};

		public static string HandleKeypress(string key, string s_count)
		{
			if (key == null) return "empty-key";
			Log("Keypress: " + key);
			int count = String.IsNullOrEmpty(s_count) ? 0 : int.Parse(s_count);
			if (SingleMediaKeys.ContainsKey(key))
			{
				if (count == 1)
					InputSimulator.SimulateKeyPress(SingleMediaKeys[key]);
			}
			else if (MultipleMediaKeys.ContainsKey(key))
			{
				InputSimulator.SimulateKeyPress(MultipleMediaKeys[key]);
			}
			else if (SingleHandlers.ContainsKey(key))
			{
				if (count == 1)
					SingleHandlers[key]();
			}
			else if (MultipleHandlers.ContainsKey(key))
			{
				MultipleHandlers[key](count);
			}
			else
			{
				Log("Unknown key: " + key);
				return "missed-key";
			}
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

		

		private static void Log(string message)
		{
			Program.Log(message);
		}
	}
}
