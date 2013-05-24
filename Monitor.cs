using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
		HttpListener _server;

		public Monitor()
		{
			StartServer();
			Log(String.Format("Server started on port {0}", Settings.Port));
			var t = new Thread(Listen);
			t.IsBackground = true;
			t.Start();
		}

		public void StartServer()
		{
			_server = new HttpListener();
			string prefix = String.Format("http://*:{0}/", Settings.Port);
			_server.Prefixes.Add(prefix);

			try
			{
				_server.Start();
			}
			catch (HttpListenerException ex)
			{
				if (ex.ErrorCode == 5)
				{
					var username = Environment.GetEnvironmentVariable("USERNAME");
					var userdomain = Environment.GetEnvironmentVariable("USERDOMAIN");
					var message = String.Format("netsh http add urlacl url={0} user={1}\\{2} listen=yes", prefix, userdomain, username);
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
					Log("Listener error", ex);
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
				Log("Handler exception", ex);
				return "Error: " + ex.Message;
			}
		}
	}
}
