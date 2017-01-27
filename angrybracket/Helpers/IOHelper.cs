using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngryBracket
{
	public static class IOHelper
	{
		/// <summary>
		/// Reads from the given TextReader up until the given char is found.
		/// A string is returned containing all consumed chars, except for the trailing c.
		/// </summary>
		public static string ReadUntil(this TextReader sr, char c)
		{
			StringBuilder str = new StringBuilder(128);

			int read = sr.Read();
			while (read != -1 && read != c)
			{
				str.Append((char)read);
				read = sr.Read();
			}
			return str.ToString();
		}

		/// <summary>
		/// Reads from the given TextReader up until the given string is found.
		/// A string is returned containing all consumed chars, including the trailing instance of s.
		/// </summary>
		public static string ReadUntil(this TextReader sr, string s)
		{
			StringBuilder sb = new StringBuilder(128);

			int read = sr.Read();
			while (read != -1)
			{
				sb.Append((char)read);

				if (read == s[s.Length - 1])
				{
					//Iterate backwards looking for a match
					bool match = true;
					for (int i = 0; i < s.Length; i++)
					{
						if (sb[sb.Length - i - 1] != s[s.Length - i - 1])
						{
							match = false;
							break;
						}
					}
					if (match) break;
				}

				read = sr.Read();
			}
			return sb.ToString();
		}

		public static void WriteIndentedLine(this TextWriter tw, int indentLevel, string format, params object[] args)
		{
			for (int i = 0; i < indentLevel; i++)
				tw.Write("\t");
			tw.WriteLine(format, args);
		}

		public static void WriteIndentedLine(this TextWriter tw, int indentLevel, string line)
		{
			for (int i = 0; i < indentLevel; i++)
				tw.Write("\t");
			tw.WriteLine(line);
		}

		public static void WriteIndented(this TextWriter tw, int indentLevel, string format, params object[] args)
		{
			for (int i = 0; i < indentLevel; i++)
				tw.Write("\t");
			tw.Write(format, args);
		}

		public static void WriteIndent(this TextWriter tw, int indentLevel)
		{
			for (int i = 0; i < indentLevel; i++)
				tw.Write("\t");
		}

		public static void WriteQuoted(this TextWriter tw, string text, char quoteCharacter = '"')
		{
			tw.Write(quoteCharacter);
			for (int i = 0; i < text.Length; i++)
			{
				if (text[i] == '"' || text[i] == '\\' || text[i] == '\n' || text[i] == '\r')
					tw.Write("\\");
				tw.Write(text[i]);
			}
			tw.Write(quoteCharacter);
		}

		public static void EnsureRemainingBytes(this BinaryReader br, long length)
		{
			var streamLength = br.BaseStream.Length;
			var streamPos = br.BaseStream.Position;

			if (streamLength - streamPos < length)
				throw new Exception("Expected at least " + length + " more bytes in stream");
		}
	}
}
