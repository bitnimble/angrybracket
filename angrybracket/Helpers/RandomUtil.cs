using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AngryBracket
{
	/// <summary>
	/// Makes Random static (thread-safe).
	/// Adds some useful helpers.
	/// </summary>
	public static class RandomUtil
	{
		static ThreadLocal<Random> random = new ThreadLocal<Random>(() => new Random());

		public static int Next() { return random.Value.Next(); }
		/// <summary>
		/// Max is exclusive upper bound
		/// </summary>
		/// <param name="max">The exclusive upper bound</param>
		public static int Next(int max) { return random.Value.Next(max); }
		/// <summary>
		/// Max is exclusive upper bound
		/// </summary>
		/// <param name="min">Inclusive lower bound</param>
		/// <param name="max">The exclusive upper bound</param>
		public static int Next(int min, int max) { return random.Value.Next(min, max); }

		public static long NextLong() { return (((uint)random.Value.Next() << 32) | (uint)random.Value.Next()); }

		public static double NextDouble() { return random.Value.NextDouble(); }
		public static double NextDouble(double min, double max) { return random.Value.NextDouble() * (max - min) + min; }

		public static float NextFloat() { return (float)NextDouble(); }
		public static float NextFloat(double min, double max) { return (float)NextDouble(min, max); }

		public static bool NextBool() { return Next(0, 2) == 0; }

		/// <summary>
		/// A normalised sample where 1 represents 1 std deviation, 2 represents 2 std deviations, etc.
		/// </summary>
		/// <param name="r"></param>
		/// <returns></returns>
		public static double NormalSample(this Random r)
		{
			var sinWave = Math.Sin(2.0 * Math.PI * r.NextDouble());
			var sqrt = Math.Sqrt(-2.0 * Math.Log(r.NextDouble()));
			return sinWave * sqrt;
		}

		public static double NormalSample()
		{
			return random.Value.NormalSample();
		}

		public const string MixedAlphaNumeric = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
		public const string LowerAlphaNumeric = "abcdefghijklmnopqrstuvwxyz0123456789";
		public const string LowerAlpha = "abcdefghijklmnopqrstuvwxyz";

		public static string RandomString(int length)
		{
			return RandomString(length, MixedAlphaNumeric);
		}

		public static string RandomString(int length, string sourceChars)
		{
			int sourceLen = sourceChars.Length;
			char[] chars = new char[length];

			for (int i = 0; i < length; i++)
				chars[i] = sourceChars[Next(sourceLen)];

			return new string(chars, 0, length);
		}
	}

	public class SecureRandomUtil
	{
		RNGCryptoServiceProvider random;
		byte[] buffer = new byte[512];
		int bpos = 0;

		public SecureRandomUtil()
		{
			random = new RNGCryptoServiceProvider();
		}

		byte[] GetBytes(int count)
		{
			var buffer = new byte[count];
			random.GetBytes(buffer);
			return buffer;
		}

		T InvokeWithBytes<T>(int count, Func<byte[], int, T> func)
		{
			if (bpos + count > buffer.Length)
			{
				//Count cannot be fulfilled by using buffer.
				if (count > buffer.Length)
				{
					var tempBuffer = GetBytes(count);
					return func(tempBuffer, 0);
				}

				//Refill
				random.GetBytes(buffer);
				bpos = 0;
			}
			var result = func(buffer, bpos);
			bpos += count;
			return result;
		}

		public int Next() { return InvokeWithBytes(4, BitConverter.ToInt32); }
		public int Next(int max) { return Math.Abs(InvokeWithBytes(4, BitConverter.ToInt32)) % max; }
		public int Next(int min, int max) { return Math.Abs(InvokeWithBytes(4, BitConverter.ToInt32)) % (max - min) + min; }

		public long NextLong() { return InvokeWithBytes(4, BitConverter.ToInt64); }

		/// <summary>
		/// FIXME: Should be between 0 and 1
		/// </summary>
		/// <returns></returns>
		public double NextDouble() { return InvokeWithBytes(8, BitConverter.ToDouble); }
		public double NextDouble(double min, double max) { return NextDouble() * (max - min) + min; }


		public float NextFloat() { return (float)NextDouble(); }
		public float NextFloat(double min, double max) { return (float)NextDouble(min, max); }

		public bool NextBool() { return Next(0, 2) == 0; }

		public const string MixedAlphaNumeric = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
		public const string LowerAlphaNumeric = "abcdefghijklmnopqrstuvwxyz0123456789";
		public const string LowerAlpha = "abcdefghijklmnopqrstuvwxyz";

		public string RandomString(int length)
		{
			return RandomString(length, MixedAlphaNumeric);
		}

		public string RandomString(int length, string sourceChars)
		{
			int sourceLen = sourceChars.Length;
			char[] chars = new char[length];

			for (int i = 0; i < length; i++)
				chars[i] = sourceChars[Next(sourceLen)];

			return new string(chars, 0, length);
		}

		public byte[] NextBytes(int count)
		{
			return GetBytes(count);
		}
	}
}
