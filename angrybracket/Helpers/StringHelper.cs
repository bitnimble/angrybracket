using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngryBracket
{
	public static class StringHelper
	{
		public static string FormatString(this string format, params object[] args)
		{
			return string.Format(format, args);
		}

		public static string Truncate(this string s, int maxLength)
		{
			if (s.Length <= maxLength)
				return s;
			if (maxLength < 3)
				return maxLength == 0 ? "" : maxLength == 1 ? "." : "..";
			return s.Substring(0, maxLength - 3) + "...";
		}

		/// <summary>
		/// Counts the number of occurrences of the given character in the given string.
		/// </summary>
		/// <param name="s">The string to check</param>
		/// <param name="c">The char to look for</param>
		public static int Count(this string s, char c)
		{
			int count = 0;
			for (int i = 0; i < s.Length; i++)
				if (s[i] == c)
					count++;
			return count;
		}

		/// <summary>
		/// Reverses the current string.
		/// </summary>
		public static string Reverse(this string s)
		{
			char[] chars = new char[s.Length];
			for (int i = 0; i < chars.Length; i++)
				chars[i] = s[s.Length - i - 1];
			return new string(chars);
		}

		public static byte[] ParseAsHex(this string s)
		{
			return s.Pair().Select(pair => (byte)(GetHexVal(pair.Item1) << 4 | GetHexVal(pair.Item2))).ToArray();
		}

		static int GetHexVal(char c)
		{
			if (c >= '0' && c <= '9')
				return c - '0';
			if (c >= 'a' && c <= 'f')
				return c - 'a' + 10;
			if (c >= 'A' && c <= 'F')
				return c - 'A' + 10;
			return 0;
		}

		/// <summary>
		/// If input is in values, returns substitute.
		/// </summary>
		public static T Substitute<T>(T input, T substitute, params T[] values)
			where T : IEquatable<T>
		{
			foreach (var value in values)
				if (input.Equals(value))
					return substitute;
			return input;
		}

		public static string EscapeQuotesAndSlashes(this string s)
		{
			return s.Replace("\\", "\\\\").Replace("\"", "\\\"");
		}
		public static string UnescapeQuotesAndSlashes(this string s)
		{
			return s.Replace("\\\"", "\"").Replace("\\\\", "\\");
		}

		/// <summary>
		/// Escapes quotes &amp; slashes and surrounds the given string in quotes.
		/// </summary>
		public static string Quote(this string s) { return s == null ? null : ("\"" + EscapeQuotesAndSlashes(s) + "\""); }

		/// <summary>
		/// Unescapes quotes &amp; slashes and removes surrounding quotes from the given string. Does nothing if the string doesn't
		/// start &amp; end with quotes.
		/// </summary>
		public static string Unquote(this string s)
		{
			if (s.Length < 2)
				return s;
			if (s[0] != '"' || s[s.Length - 1] != '"')
				return s;

			return UnescapeQuotesAndSlashes(s.Substring(1, s.Length - 2));
		}

		/// <summary>
		/// Able to parse dates as:
		/// DD/MM/YY, DD/MM/YYYY, MM/YY, MM/YYYY, Mon/YY, Mon/YYYY, YYYY, YYYY/MM/DD, YYYY/Mon/DD, YYYYMMDD
		/// All previous with '-', ' ', '\\', '\t', ',' instead of '/'
		/// </summary>
		/// <param name="dateString"></param>
		/// <param name="twoDigitYearsNinetiesCutoff">Two digit years above this number are treated as being in the past. Set to -1 for all, 100 for none.</param>
		/// <returns></returns>
		public static DateTime ParseFreeFormDate(string dateString, int twoDigitYearsNinetiesCutoff = 50, bool american = false)
		{
			DateTime result;
			if (!TryParseFreeFormDate(dateString, out result, twoDigitYearsNinetiesCutoff, american))
				throw new FormatException("Could not parse '" + dateString + "'");
			return result;
		}

		/// <summary>
		/// Able to parse dates as:
		/// DD/MM/YY, DD/MM/YYYY, MM/YY, MM/YYYY, Mon/YY, Mon/YYYY, YYYY, YYYY/MM/DD, YYYY/Mon/DD
		/// All previous with '-', ' ', '\\', '\t', ',' instead of '/'
		/// </summary>
		/// <param name="dateString"></param>
		/// <param name="twoDigitYearsNinetiesCutoff">Two digit years above this number are treated as being in the past. Set to -1 for all, 100 for none.</param>
		/// <returns></returns>
		public static bool TryParseFreeFormDate(string dateString, out DateTime result, int twoDigitYearsNinetiesCutoff = 50, bool american = false)
		{
			result = DateTime.Today;
			var allowedSeps = new[] { '-', '/', '\\', ' ', '\t', ',' };

			int year = 0, month = 1, day = 1;
			var parts = dateString.Trim().Split(allowedSeps, StringSplitOptions.RemoveEmptyEntries);
			if (parts.Length == 1)
			{
				if (parts[0].Length == 8)
					parts = new[] { parts[0].Substring(0, 4), parts[0].Substring(4, 2), parts[0].Substring(6, 2) };
				else if (!TryParseYear(parts[0], twoDigitYearsNinetiesCutoff, out year))
					return false;
			}

			if (parts.Length == 2)
			{
				//Valid formats are all Month-Year, but there are different formats for both.
				if (!TryParseMonth(parts[0], out month))
					return false;

				if (!TryParseYear(parts[1], twoDigitYearsNinetiesCutoff, out year))
					return false;
			}
			else if (parts.Length == 3)
			{
				int dayIndex = american ? 1 : 0;
				int monthIndex = american ? 0 : 1;
				int yearIndex = 2;

				//Override for YYYY/MM/DD
				if (parts[0].Length == 4 && int.TryParse(parts[0], out year))
				{
					dayIndex = 2;
					monthIndex = 1;
					yearIndex = 0;
				}

				if (!int.TryParse(parts[dayIndex], out day) || !TryParseMonth(parts[monthIndex], out month) || !TryParseYear(parts[yearIndex], twoDigitYearsNinetiesCutoff, out year))
					return false;

			}
			else return false;

			result = new DateTime(year, month == 0 ? 1 : month, day == 0 ? 1 : day);
			return true;
		}

		private static bool TryParseYear(string str, int twoDigitYearCutoff, out int year)
		{
			year = 0;
			if (str.Length == 2)
			{
				if (!int.TryParse(str, out year))
					return false;
				//Adjust for two digit cutoff.
				if (year > twoDigitYearCutoff)
					year += 1900;
				else
					year += 2000;
				return true;
			}
			else if (str.Length == 4)
				return int.TryParse(str, out year);
			else return false;
		}

		private static bool TryParseMonth(string str, out int month)
		{
			month = 0;

			if (int.TryParse(str, out month))
				return true; //That worked well.

			str = str.ToLower();
			//Now try for an abbreviated month. We basically check to see if it starts with any of these.
			//Note that we don't care if the input is "mayasjdsadiops", "septober", "decuary", etc.
			var months = new[] { "jan", "feb", "mar", "apr", "may", "jun", "jul", "aug", "sep", "oct", "nov", "dec" };
			for (int i = 0; i < months.Length; i++)
			{
				if (str.StartsWith(months[i]))
				{
					month = i + 1;
					return true;
				}
			}

			return false;
		}

		public enum TimeUnits { Nanoseconds, Microseconds, Milliseconds, Seconds, Minutes, Hours, Days }
		public enum DistanceUnits { Nanometers, Micrometers, Millimeters, Centimeters, Meters, Kilometers }

		//Format: Unit, number of this unit in next unit, suffix, decimal places to print for this unit.
		private static readonly Tuple<TimeUnits, int, string, int>[] timeConversions = new[] {
			Tuple.Create(TimeUnits.Nanoseconds, 1000, "ns", 0),
			Tuple.Create(TimeUnits.Microseconds, 1000, "us", 2),
			Tuple.Create(TimeUnits.Milliseconds, 1000, "ms", 2),
			Tuple.Create(TimeUnits.Seconds, 60, "s", 2),
			Tuple.Create(TimeUnits.Minutes, 60, "min", 1),
			Tuple.Create(TimeUnits.Hours, 24, "hrs", 1),
			Tuple.Create(TimeUnits.Days, 0, "days", 2)
		};

		private static readonly Tuple<DistanceUnits, int, string, int>[] distanceConversions = new[] {
			Tuple.Create(DistanceUnits.Nanometers, 1000, "nm", 0),
			Tuple.Create(DistanceUnits.Micrometers, 10, "um", 0),
			Tuple.Create(DistanceUnits.Micrometers, 100, "um", 1),
			Tuple.Create(DistanceUnits.Millimeters, 10, "mm", 1),
			Tuple.Create(DistanceUnits.Centimeters, 10, "cm", 1),
			Tuple.Create(DistanceUnits.Meters, 10, "m", 2), //We break meters into 3 sections of different
			Tuple.Create(DistanceUnits.Meters, 10, "m", 1), //precision. 1-10m = 2dp, 10-100m = 1dp, 100-1000m = 0dp.
			Tuple.Create(DistanceUnits.Meters, 10, "m", 0),
			Tuple.Create(DistanceUnits.Kilometers, 10, "km", 2), //We do the same for km.
			Tuple.Create(DistanceUnits.Kilometers, 10, "km", 1),
			Tuple.Create(DistanceUnits.Kilometers, 0, "km", 0)
		};

		private static string ConvertUnit<T>(double value, T units, Tuple<T, int, string, int>[] conversions)
		{
			int index = -1;
			for (int i = 0; i < conversions.Length; i++)
				if (units.Equals(conversions[i].Item1))
				{
					index = i;
					break;
				}

			if (index == -1) throw new Exception("Invalid unit");

			//conversions[index] = units.
			//Move us as far left as we can go...
			while (index > 0 && value < 1.0f)
				value *= conversions[--index].Item2;

			//And as far right as we can go...
			while (index < conversions.Length && value >= conversions[index].Item2)
				value /= conversions[index++].Item2;

			return value.ToString("N" + conversions[index].Item4) + " " + conversions[index].Item3;
		}

		public static string ToTimeString(this float value, TimeUnits units = TimeUnits.Seconds)
		{
			return ConvertUnit(value, units, timeConversions);
		}
		public static string ToTimeString(this double value, TimeUnits units = TimeUnits.Seconds)
		{
			return ConvertUnit(value, units, timeConversions);
		}
		public static string ToDistanceString(this float value, DistanceUnits units = DistanceUnits.Meters)
		{
			return ConvertUnit(value, units, distanceConversions);
		}
		public static string ToDistanceString(this double value, DistanceUnits units = DistanceUnits.Meters)
		{
			return ConvertUnit(value, units, distanceConversions);
		}

		/// <summary>
		/// Determines whether the end of this string instance matches any of the specified strings.
		/// </summary>
		/// <param name="s"></param>
		/// <param name="values"></param>
		/// <returns></returns>
		public static bool EndsWith(this string s, IEnumerable<string> values)
		{
			foreach (string ending in values)
				if (s.EndsWith(ending))
					return true;
			return false;
		}

		/// <summary>
		/// Determines whether the beginning of this string instance matches any of the specified strings.
		/// </summary>
		/// <param name="s"></param>
		/// <param name="values"></param>
		/// <returns></returns>
		public static bool StartsWith(this string s, IEnumerable<string> values)
		{
			foreach (string prefix in values)
				if (s.StartsWith(prefix))
					return true;
			return false;
		}

		//The below functions are just 'ToString' functions for int/short/etc.
		//They have been tested to be faster than the framework versions.
		//They all use the Invariant Culture, with no formatting options available.
		//They are recommended only for use when profiling has revealed that the framework
		//methods are causing a slowdown.
		#region ToStringQuick

		static int toCharBuff(char[] chars, int chari, uint value)
		{
			if (value == 0)
				chars[--chari] = '0';
			else
			{
				do
					chars[--chari] = (char)('0' + (value % 10));
				while ((value /= 10) > 0);
			}
			return chari;
		}

		static int toCharBuff(char[] chars, int chari, ulong value)
		{
			if (value == 0)
				chars[--chari] = '0';
			else
			{
				do
					chars[--chari] = (char)('0' + (value % 10));
				while ((value /= 10) > 0);
			}
			return chari;
		}

		public static string ToStringQuick(this sbyte value)
		{
			//-128
			const int MaxLen = 4;
			char[] chars = new char[MaxLen];
			int chari = MaxLen;

			if (value < 0)
			{
				chari = toCharBuff(chars, chari, (uint)(-value));
				chars[--chari] = '-';
			}
			else
				chari = toCharBuff(chars, chari, (uint)value);

			return new string(chars, chari, MaxLen - chari);
		}

		public static string ToStringQuick(this short value)
		{
			//-32000
			const int MaxLen = 6;
			char[] chars = new char[MaxLen];
			int chari = MaxLen;

			if (value < 0)
			{
				chari = toCharBuff(chars, chari, (uint)(-value));
				chars[--chari] = '-';
			}
			else
				chari = toCharBuff(chars, chari, (uint)value);

			return new string(chars, chari, MaxLen - chari);
		}

		/// <summary>
		/// Faster than the framework version by > 30% in a release build.
		/// No formatting options. Invariant.
		/// </summary>
		public static string ToStringQuick(this int value)
		{
			//-2000000000
			const int MaxLen = 11;
			char[] chars = new char[MaxLen];
			int chari = MaxLen;

			if (value < 0)
			{
				chari = toCharBuff(chars, chari, (uint)(-value));
				chars[--chari] = '-';
			}
			else
				chari = toCharBuff(chars, chari, (uint)value);

			return new string(chars, chari, MaxLen - chari);
		}

		public static string ToStringQuick(this long value)
		{
			//-9223IcantRemember..
			const int MaxLen = 20;
			char[] chars = new char[MaxLen];
			int chari = MaxLen;

			if (value < 0)
			{
				chari = toCharBuff(chars, chari, (ulong)(-value));
				chars[--chari] = '-';
			}
			else
				chari = toCharBuff(chars, chari, (ulong)value);

			return new string(chars, chari, MaxLen - chari);
		}

		public static string ToStringQuick(this byte value)
		{
			//255
			const int MaxLen = 3;
			char[] chars = new char[MaxLen];
			int chari = MaxLen;

			if (value == 0)
				chars[--chari] = '0';
			else
			{
				do
					chars[--chari] = (char)('0' + (value % 10));
				while ((value /= 10) > 0);
			}

			return new string(chars, chari, MaxLen - chari);
		}

		public static string ToStringQuick(this ushort value)
		{
			//65535
			const int MaxLen = 5;
			char[] chars = new char[MaxLen];
			int chari = MaxLen;

			if (value == 0)
				chars[--chari] = '0';
			else
			{
				do
					chars[--chari] = (char)('0' + (value % 10));
				while ((value /= 10) > 0);
			}

			return new string(chars, chari, MaxLen - chari);
		}

		/// <summary>
		/// Faster than the framework version by > 30% in a release build.
		/// No formatting options. Invariant.
		/// </summary>
		public static string ToStringQuick(this uint value)
		{
			//4000000000
			char[] chars = new char[10];
			int chari = 10;

			if (value == 0)
				chars[--chari] = '0';
			else
			{
				while (value > 0)
				{
					chars[--chari] = (char)('0' + (value % 10));
					value /= 10;
				}
			}

			return new string(chars, chari, 10 - chari);
		}

		public static string ToStringQuick(this ulong value)
		{
			//18446744073709551616
			const int MaxLen = 20;
			char[] chars = new char[MaxLen];
			int chari = MaxLen;

			if (value == 0)
				chars[--chari] = '0';
			else
			{
				do
					chars[--chari] = (char)('0' + (value % 10));
				while ((value /= 10) > 0);
			}

			return new string(chars, chari, MaxLen - chari);
		}

		public static string ToStringQuick(this bool value)
		{
			return value ? "true" : "false";
		}

		public static bool TryParseSbyte(string s, out sbyte result)
		{
			result = 0;

			if (s.Length == 0) return false;

			int chari = -1;
			const int maxLen = 3;
			bool negative = false;
			if (s[0] == '-')
			{
				negative = true;
				chari++;
			}

			int max = maxLen + (negative ? 1 : 0);
			if (s.Length >= max) //Avoid extra branch in common case of < 100
			{
				if (s.Length == max) //Right on the border
				{
					//Don't worry about if this isn't a number -- that case will be picked up in the loop below.
					if (s[chari + 1] > '1') return false; //Too large -- we may be unable to parse as byte
				}
				else return false; //Too long
			}

			byte value = 0;
			max = Math.Min(max, s.Length) - 1;
			for (; chari < max;)
			{
				char c = s[++chari];
				byte n = (byte)(c - '0');
				if (n > 9) return false; //Bad char
				value = (byte)(value * (byte)10 + n);
			}

			result = negative ? (sbyte)-(sbyte)value : (sbyte)value;

			//Bounds check
			byte valueMax = (byte)(negative ? 128 : 127);
			return value <= valueMax; //Range check
		}

		public static bool TryParseShort(string s, out short result)
		{
			result = 0;

			if (s.Length == 0) return false;

			int chari = -1;
			const int maxLen = 5;
			bool negative = false;
			if (s[0] == '-')
			{
				negative = true;
				chari++;
			}

			int max = maxLen + (negative ? 1 : 0);
			if (s.Length >= max) //Avoid extra branch in common case of < 10,000
			{
				if (s.Length == max) //Right on the border
				{
					//Don't worry about if this isn't a number -- that case will be picked up in the loop below.
					if (s[chari + 1] > '3') return false; //Too large -- we may be unable to parse as ushort
				}
				else return false; //Too long
			}

			ushort value = 0;
			max = Math.Min(max, s.Length) - 1;
			for (; chari < max;)
			{
				char c = s[++chari];
				ushort n = (ushort)(c - '0');
				if (n > 9) return false; //Bad char
				value = (ushort)(value * (ushort)10 + n);
			}

			result = negative ? (short)-(short)value : (short)value;

			//Bounds check
			ushort valueMax = (ushort)(negative ? 32768 : 32767);
			return value <= valueMax; //Range check
		}

		public static bool TryParseInt(string s, out int result)
		{
			result = 0;

			if (s.Length == 0) return false;

			int chari = -1;
			const int maxLen = 11;
			bool negative = false;
			if (s[0] == '-')
			{
				negative = true;
				chari++;
			}

			int max = maxLen + (negative ? 1 : 0);
			if (s.Length >= max) //Avoid extra branch in common case of < 1 billion
			{
				if (s.Length == max) //Right on the border
				{
					//Don't worry about if this isn't a number -- that case will be picked up in the loop below.
					if (s[chari + 1] > '2') return false; //Too large -- we may be unable to parse as uint
				}
				else return false; //Too long
			}

			uint value = 0;
			max = Math.Min(max, s.Length) - 1;
			for (; chari < max;)
			{
				char c = s[++chari];
				uint n = (uint)c - '0';
				if (n > 9) return false; //Bad char
				value = value * 10U + n;
			}

			result = negative ? -(int)value : (int)value;

			//Bounds check
			uint valueMax = negative ? 2147483648U : 2147483647U;
			return value <= valueMax; //Range check
		}

		public static bool TryParseLong(string s, out long result)
		{
			result = 0;

			if (s.Length == 0) return false;

			int chari = -1;
			const int maxLen = 19;
			bool negative = false;
			if (s[0] == '-')
			{
				negative = true;
				chari++;
			}

			int max = maxLen + (negative ? 1 : 0);
			//Don't need to check s.Length == max for long, as ulong can hold a whole extra digit.
			if (s.Length > max) return false; //Too long

			ulong value = 0;
			max = Math.Min(max, s.Length) - 1;
			for (; chari < max;)
			{
				char c = s[++chari];
				ulong n = (ulong)c - '0';
				if (n > 9) return false; //Bad char
				value = value * 10UL + n;
			}

			result = negative ? -(long)value : (long)value;

			//Bounds check
			ulong valueMax = negative ? 9223372036854775808UL : 9223372036854775807UL;
			return value <= valueMax; //Range check
		}

		public static bool TryParseUShort(string s, out ushort result)
		{
			result = 0;

			if (s.Length == 0) return false;

			int chari = -1;
			const int maxLen = 5;

			if (s.Length >= maxLen) //Avoid extra branch in common case of < 1 billion
			{
				if (s.Length == maxLen) //Right on the border
				{
					//Don't worry about if this isn't a number -- that case will be picked up in the loop below.
					if (s[chari + 1] > '6') return false; //Too large
				}
				else return false; //Too long
			}

			ushort value = 0;
			int max = Math.Min(maxLen, s.Length) - 1;
			for (; chari < max;)
			{
				char c = s[++chari];
				ushort n = (ushort)(c - '0');
				if (n > 9) return false; //Bad char
				value = (ushort)(value * 10 + n);
			}

			result = value;

			//Bounds check
			return s.Length != maxLen || value > 9999; //Range check
		}

		public static bool TryParseUInt(string s, out uint result)
		{
			result = 0;

			if (s.Length == 0) return false;

			int chari = -1;
			const int maxLen = 11;

			if (s.Length >= maxLen) //Avoid extra branch in common case of < 1 billion
			{
				if (s.Length == maxLen) //Right on the border
				{
					//Don't worry about if this isn't a number -- that case will be picked up in the loop below.
					if (s[chari + 1] > '4') return false; //Too large
				}
				else return false; //Too long
			}

			uint value = 0;
			int max = Math.Min(maxLen, s.Length) - 1;
			for (; chari < max;)
			{
				char c = s[++chari];
				uint n = (uint)c - '0';
				if (n > 9) return false; //Bad char
				value = value * 10U + n;
			}

			result = value;

			//Bounds check
			return s.Length != maxLen || value > 999999999; //Range check
		}

		public static bool TryParseULong(string s, out ulong result)
		{
			result = 0;

			if (s.Length == 0) return false;

			int chari = -1;
			const int maxLen = 20;

			if (s.Length >= maxLen) //Avoid extra branch in common case of < 1 billion
			{
				if (s.Length == maxLen) //Right on the border
				{
					//Don't worry about if this isn't a number -- that case will be picked up in the loop below.
					if (s[chari + 1] > '1') return false; //Too large
				}
				else return false; //Too long
			}

			ulong value = 0;
			int max = Math.Min(maxLen, s.Length) - 1;
			for (; chari < max;)
			{
				char c = s[++chari];
				ulong n = (ulong)(c - '0');
				if (n > 9) return false; //Bad char
				value = value * 10UL + n;
			}

			result = value;

			//Bounds check
			return s.Length != maxLen || value > 9999999999999999999; //Range check
		}

		#endregion
	}
}
