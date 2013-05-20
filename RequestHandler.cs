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
		public static Dictionary<string, Action<NameValueCollection>> Handlers = new Dictionary<string, Action<NameValueCollection>>
		{
			{"/test", Test}
		};

		public static void Test(NameValueCollection args)
		{
			MessageBox.Show("Test. qwe=" + args["qwe"]);
		}
	}
}
