using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RemoteServer
{
	public static class RequestHandler
	{
		public static Dictionary<string, Action> Handlers = new Dictionary<string, Action>
		{
			{"key_test", Test}
		};

		public static void Test()
		{
			MessageBox.Show("Test-key");
		}
	}
}
