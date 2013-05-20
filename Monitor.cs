using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Threading;

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
			_server.Prefixes.Add("http://localhost:8080/");
			_server.Start();
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
				Action<NameValueCollection> handler = null;
				if (RequestHandler.Handlers.TryGetValue(request.Url.AbsolutePath, out handler))
				{
					Log("Request: " + request.Url.ToString());
					handler(request.QueryString);
					return "ok";
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
