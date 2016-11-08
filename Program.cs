using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;

namespace RemoteServer
{
	public class Program
	{
		public const string AppName = "Irda monitor";
		public static Program Instance { get; private set; }
		internal static Properties.Settings Settings { get { return Properties.Settings.Default; } }
		protected NotifyIcon _icon;

		[STAThread]
		static void Main(string[] args)
		{
			try
			{
				Instance = new Monitor();
				Application.ThreadException += (o, e) => Instance.Log(e.Exception);
				Application.Run();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
				Log($"Exception: {ex}");
				Application.Exit();
			}
		}

		public Program()
		{
			InitTrayIcon();
		}

		private void InitTrayIcon()
		{
			ContextMenuStrip menu = new ContextMenuStrip();
			menu.Items.AddRange(new ToolStripItem[] 
			{
				new ToolStripMenuItem("Show log", null, (q, w) => Process.Start(Settings.LogFile)),
				new ToolStripMenuItem("Clear log", null, (q, w) => File.Delete(Settings.LogFile)),
				new ToolStripMenuItem("Open app location", null, (q, w) => Process.Start(Path.GetDirectoryName(Application.ExecutablePath))),
				new ToolStripMenuItem("Exit", null, (q, w) => Application.Exit())
			});

			_icon = new NotifyIcon()
			{
				Icon = Properties.Resources.synchronize,
				Text = AppName,
				ContextMenuStrip = menu,
				Visible = true
			};
			_icon.MouseClick += (obj, args) =>
			{
				if (args.Button == MouseButtons.Left)
					Process.Start(Settings.LogFile);
			};

			Application.ApplicationExit += (obj, args) =>
			{
				_icon.Dispose();
				menu.Dispose();
			};
		}

		public static void Log(string message)
		{
			File.AppendAllText(Settings.LogFile, $"[{DateTime.Now}] {message}\r\n");
		}
		public void Log(Exception e)
		{
			Log(e, "Exception");
		}
		public void Log(Exception e, string message)
		{
			Log($"{message}: {e}");
			if (_icon != null)
				_icon.ShowBalloonTip(1000, "Error", e.Message, ToolTipIcon.Error);
		}
	}
}
