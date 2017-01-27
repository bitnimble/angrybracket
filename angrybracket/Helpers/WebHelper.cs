using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AngryBracket.Helpers
{
	class WebHelper
	{
		private static byte[] createPostData(Dictionary<string, string> postData)
		{
			StringBuilder result = new StringBuilder("{");
			foreach (var keyValuePair in postData)
			{
				result.Append("\"" + keyValuePair.Key + "\":");
				result.Append("\"" + keyValuePair.Value + "\",");
			}
			result[result.Length - 1] = '}';

			return Encoding.ASCII.GetBytes(result.ToString());
		}

		public static string Post(string address, Dictionary<string, string> postData, string contentType)
		{
			HttpWebRequest hwr = WebRequest.CreateHttp(address);
			hwr.ContentType = contentType;
			hwr.Method = "POST";

			byte[] postDataBytes = createPostData(postData);
			hwr.ContentLength = postDataBytes.Length;

			Stream reqStream = hwr.GetRequestStream();
			reqStream.Write(postDataBytes, 0, postDataBytes.Length);
			reqStream.Flush();
			reqStream.Close();

			Stream respStream = hwr.GetResponse().GetResponseStream();
			string result = new StreamReader(respStream).ReadToEnd();

			return result;
		}

		public static string Get(string address)
		{
			HttpWebRequest hwr = WebRequest.CreateHttp(address);
			hwr.Method = "GET";

			Stream respStream = hwr.GetResponse().GetResponseStream();
			string result = new StreamReader(respStream).ReadToEnd();

			return result;
		}
	}
}
