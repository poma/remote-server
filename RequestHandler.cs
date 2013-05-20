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
		public static Dictionary<string, Action> Handlers = new Dictionary<string, Action>
		{
			{"key_test", Test},
			{"volume_up", VolumeUp},
			{"volume_down", VolumeDown},
			{"mute", Mute},
			{"play", PlayPause}
		};

		public static void Test()
		{
			MessageBox.Show("Test-key");
		}

		public static void VolumeUp()
		{
			InputSimulator.SimulateKeyPress(VirtualKeyCode.VOLUME_UP);
			//var device = new MMDeviceEnumerator().GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
			//device.AudioEndpointVolume.VolumeStepUp();
		}

		public static void VolumeDown()
		{
			InputSimulator.SimulateKeyPress(VirtualKeyCode.VOLUME_DOWN);
			//var device = new MMDeviceEnumerator().GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
			//device.AudioEndpointVolume.VolumeStepDown();
		}

		public static void Mute()
		{
			InputSimulator.SimulateKeyPress(VirtualKeyCode.VOLUME_MUTE);
			//var device = new MMDeviceEnumerator().GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
			//device.AudioEndpointVolume.Mute = !device.AudioEndpointVolume.Mute;
		}

		public static void PlayPause()
		{
			InputSimulator.SimulateKeyPress(VirtualKeyCode.MEDIA_PLAY_PAUSE);
		}
	}
}
