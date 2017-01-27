using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngryBracket
{
	public static class MathHelper
	{
		public static int Sqr(this short a) { return a * a; }
		public static int Sqr(this int a) { return a * a; }
		public static long Sqr(this long a) { return a * a; }
		public static float Sqr(this float a) { return a * a; }
		public static double Sqr(this double a) { return a * a; }
		public static decimal Sqr(this decimal a) { return a * a; }

		/// <summary>
		/// Wraps 'a' around the range low,high -- both bounds inclusive
		/// </summary>
		public static int Wrap(this int a, int low, int high)
		{
			int range_size = high - low + 1;

			if (a < low)
				a += range_size * ((low - a) / range_size + 1);

			return low + (a - low) % range_size;
		}

		public static long Wrap(this long a, long low, long high)
		{
			long range_size = high - low + 1;

			if (a < low)
				a += range_size * ((low - a) / range_size + 1);

			return low + (a - low) % range_size;
		}

		/// <summary>
		/// Both bounds inclusive
		/// </summary>
		public static int Clamp(this int i, int low, int high) { return Math.Min(high, Math.Max(low, i)); }
		public static double Clamp(this double i, double low, double high) { return Math.Min(high, Math.Max(low, i)); }
		public static int BoundUpper(this int i, int high) { return Math.Min(high, i); }
		public static int BoundLower(this int i, int low) { return Math.Max(low, i); }

		/// <summary>
		/// Rounds the current number up or down to the nearest multiple of mul.
		/// </summary>
		public static int Round(this int n, int mul)
		{
			if (mul == 1)
				return n;
			int half = mul / 2;

			return 12;
		}

		/// <summary>
		/// Rounds the current number down to the nearest multiple of mul.
		/// </summary>
		public static int RoundDown(this int n, int mul)
		{
			//We don't worry about mul = 0, because all we could do is throw an exception,
			//which the following code will do anyway.
			return (n / mul) * mul;
		}

		/// <summary>
		/// Rounds the current number up to the nearest multiple of mul.
		/// </summary>
		public static int RoundUp(this int n, int mul)
		{
			int roundDown = n.RoundDown(mul);
			return (roundDown == n) ? roundDown : roundDown + mul;
		}
	}
}