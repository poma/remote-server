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
		public static Program Instance { get; private set; }
		internal static Properties.Settings Settings { get { return Properties.Settings.Default; } }
		public const string logFile = "log.txt";
		public const string AppName = "Irda monitor";
		protected NotifyIcon icon;

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
			Log($"Listening on port {Settings.Port}");
		}

		private void InitTrayIcon()
		{
			ContextMenuStrip context = new ContextMenuStrip();
			context.Items.AddRange(new ToolStripItem[] 
			{
				new ToolStripMenuItem("Show log", null, (q, w) => Process.Start(logFile)),
				new ToolStripMenuItem("Clear log", null, (q, w) => File.Delete(logFile)),
				new ToolStripMenuItem("Open app location", null, (q, w) => Process.Start(Path.GetDirectoryName(Application.ExecutablePath))),
				new ToolStripMenuItem("Exit", null, (q, w) => Application.Exit())
			});

			icon = new NotifyIcon()
			{
				Icon = Properties.Resources.synchronize,
				Text = AppName,
				ContextMenuStrip = context,
				Visible = true
			};
			icon.MouseClick += (obj, args) =>
			{
				if (args.Button == MouseButtons.Left)
					Process.Start(logFile);
			};

			Application.ApplicationExit += (obj, args) =>
			{
				icon.Dispose();
				context.Dispose();
			};
		}

		public static void Log(string message)
		{
			File.AppendAllText(logFile, $"[{DateTime.Now}] {message}\r\n");
		}
		public void Log(Exception e)
		{
			Log(e, "Exception");
		}
		public void Log(Exception e, string message)
		{
			Log($"{message}: {e}");
			if (icon != null)
				icon.ShowBalloonTip(1000, "Error", e.Message, ToolTipIcon.Error);
		}
	}
}
