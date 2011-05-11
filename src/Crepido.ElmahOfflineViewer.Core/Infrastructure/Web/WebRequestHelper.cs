﻿using System.IO;
using System.Net;

namespace Crepido.ElmahOfflineViewer.Core.Infrastructure.Web
{
	public class WebRequestHelper : IWebRequestHelper
	{
		public string Request(string url)
		{
			string result;

			using (var client = new WebClient())
			{
				var response = client.OpenRead(url);
				using (var reader = new StreamReader(response))
				{
					result = reader.ReadToEnd();
				}
			}

			return result;
		}
	}
}