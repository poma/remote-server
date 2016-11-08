using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace RemoteServer
{
	public class Monitor: Program
	{
		private HttpListener _server;

		public Monitor()
		{
			StartServer();
			new Thread(Listen) { IsBackground = true, Name = "Listener" }.Start();
		}

		public void StartServer()
		{
			_server = new HttpListener();
			string prefix = $"http://*:{Settings.Port}/";
			_server.Prefixes.Add(prefix);

			try
			{
				_server.Start();
				Log($"Server started on port {Settings.Port}");
			}
			catch (HttpListenerException ex)
			{
				if (ex.ErrorCode == 5)
				{
					var username = Environment.GetEnvironmentVariable("USERNAME");
					var userdomain = Environment.GetEnvironmentVariable("USERDOMAIN");
					var message = $"netsh http add urlacl url={prefix} user={userdomain}\\{username} listen=yes";
					Clipboard.SetText(message);
					MessageBox.Show("Run command (copied to clipboard): \r\n" + message);
					Application.Exit();
				}
				else
				{
					throw;
				}
			}
		}

		public void Listen()
		{
			while (true)
			{
				try
				{
					HttpListenerContext context = _server.GetContext();

					var result = HandleRequest(context.Request);
					byte[] buffer = Encoding.UTF8.GetBytes(result);
					HttpListenerResponse response = context.Response;
					response.ContentLength64 = buffer.Length;
					response.OutputStream.Write(buffer, 0, buffer.Length);
					context.Response.Close();
				}
				catch (Exception ex)
				{
					Log(ex, "Listener error");
				}
			}
		}


		public string HandleRequest(HttpListenerRequest request)
		{
			try
			{
				if (request.Url.AbsolutePath == "/remote")
				{
					string key = request.QueryString["key"];
					string count = request.QueryString["count"];
					return RequestHandler.HandleKeypress(key, count);
				}
				else
				{
					Log("Unknown request: " + request.Url.AbsolutePath);
					return "missed";
				}
			}
			catch (Exception ex)
			{
				Log(ex, "Handler exception");
				return "Error: " + ex.Message;
			}
		}
	}
}
