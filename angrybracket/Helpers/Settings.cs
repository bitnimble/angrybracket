using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AngryBracket
{
	public class Settings
	{
		Dictionary<Type, string> typePrefixes = new Dictionary<Type, string>
		{
			{ typeof(bool), "b" },
			{ typeof(string), "s" },
			{ typeof(int), "i" },
			{ typeof(float), "f" }
		};

		Dictionary<Type, Dictionary<string, object>> settings = new Dictionary<Type, Dictionary<string, object>>();

		Dictionary<Type, Func<string, object>> coercionFuncs = new Dictionary<Type, Func<string, object>>
		{
			{ typeof(bool), s => bool.Parse(s) },
			{ typeof(string), s => s },
			{ typeof(int), s => int.Parse(s) },
			{ typeof(float), s => float.Parse(s) }
		};

		object Coerce(Type t, string input)
		{
			if (!coercionFuncs.ContainsKey(t))
				throw new Exception("Attempted to coerce type " + t.Name + " but did not have the corresponding coercion function available");

			try
			{
				return coercionFuncs[t](input);
			}
			catch (Exception e)
			{
				//Rethrow
				throw new Exception("Error occurred when coercing type " + t.Name, e);
			}
		}

		Type MatchType(string input, out string remainder)
		{
			int index = input.IndexOf('.');
			if (index < 0)
				throw new Exception("Invalid setting line; missing type delimiter: \n" + input);

			string prefix = input.Substring(0, input.IndexOf('.'));
			if (!typePrefixes.Values.Contains(prefix))
				throw new Exception("Could not match type prefix for line: \n" + input);

			remainder = input.Substring(input.IndexOf('.') + 1);
			return typePrefixes.First(kvp => kvp.Value == prefix).Key;
		}

		public void Save(string file)
		{
			StringBuilder builder = new StringBuilder();

			foreach (var types in settings)
			{
				var t = types.Key;
				var subDict = types.Value;

				foreach (var setting in subDict)
				{
					builder.Append(typePrefixes[t]);
					builder.Append('.');
					builder.Append(setting.Key);
					builder.Append('=');
					builder.AppendLine(setting.Value.ToString());
				}
			}

			File.WriteAllText(file, builder.ToString());
		}

		public void Load(string file)
		{
			settings.Clear();
			//TODO: use a line by line reader
			var lines = File.ReadAllLines(file).Select(l => l.Trim());
			foreach (var line in lines)
			{
				string setting;
				Type t = MatchType(line, out setting);

				string[] parts = setting.Split('=');
				if (parts.Length < 2)
					throw new Exception("Could not find '=' when parsing line: \n" + setting);

				string val = string.Join("", new ArraySegment<string>(parts, 1, parts.Length - 1));

				string key = parts[0];
				object value = Coerce(t, val);

				Set(t, key, value);
			}
		}

		public void Set(Type t, string key, object value)
		{
			if (!typePrefixes.ContainsKey(t))
				throw new Exception("Type " + t.Name + " does not have a registered type prefix.");
			if (key.Contains('.'))
				throw new Exception("Settings key '" + key + "' cannot contain the '.' character");

			if (!settings.ContainsKey(t))
				settings[t] = new Dictionary<string, object>();

			var subDict = settings[t];
			subDict[key] = value;
		}

		public void Set<T>(string key, T value)
		{
			Set(typeof(T), key, value);
		}

		public T Get<T>(string key)
		{
			if (!settings.ContainsKey(typeof(T)))
				throw new Exception("Key '" + key + "' not found in settings");
			var subDict = settings[typeof(T)];
			if (!subDict.ContainsKey(key))
				throw new Exception("Key '" + key + "' not found in settings");
			return (T)subDict[key];
		}
	}
}
