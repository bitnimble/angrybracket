using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace AngryBracket.Helpers
{
	public class JsonHelper
	{
		public static T Parse<T>(string input)
		{
			DataContractJsonSerializer s = new DataContractJsonSerializer(typeof(T));
			using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(input)))
			{
				return (T)s.ReadObject(stream);
			}
		}

		public static string Stringify<T>(T input)
		{
			DataContractJsonSerializer s = new DataContractJsonSerializer(typeof(T));
			using (var stream = new MemoryStream())
			{
				s.WriteObject(stream, input);
				stream.Position = 0;
				StreamReader reader = new StreamReader(stream);
				return reader.ReadToEnd();
			}
		}
	}
}
