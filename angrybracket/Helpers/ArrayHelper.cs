using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngryBracket
{
	public static class ArrayHelper
	{
		public static T[] Subarray<T>(this T[] array, int startIndex)
		{
			T[] newArray = new T[array.Length - startIndex];

			for (int i = startIndex; i < array.Length; i++)
				newArray[i - startIndex] = array[i];

			return newArray;
		}

		public static T[] Subarray<T>(this T[] array, int startIndex, int length)
		{
			T[] newArray = new T[length];

			for (int i = 0; i < length; i++)
				newArray[i] = array[i + startIndex];

			return newArray;
		}

		public static T Offset<T>(this T[] array, int index)
		{
			if (array.Length == 0)
				throw new IndexOutOfRangeException("Cannot offset into array of size 0");

			//TODO: ((index % array.Length)+array.Length)%array.Length
			while (index < 0)
				index += array.Length;
			if (index >= array.Length)
				index %= array.Length;

			return array[index];
		}

		public static int IndexOf<T>(this T[] array, T item)
		{
			for (int i = 0; i < array.Length; i++)
				if (array[i].Equals(item))
					return i;
			return -1;
		}

		public static T[] Shift<T>(this T[] array, int count)
		{
			T[] newArray = new T[array.Length];

			Array.Copy(array, count, newArray, 0, array.Length - count);
			Array.Copy(array, 0, newArray, array.Length - 1, count);

			return newArray;
		}

		static char[] Base64Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/".ToCharArray();
		const char Base64Padding = '=';

		static void encodeLast(byte[] array, int baseArrayIndex, char[] output, int baseOutputIndex, int remainCount)
		{
			byte first = array[baseArrayIndex + 0];
			byte second = remainCount >= 2 ? array[baseArrayIndex + 1] : (byte)0;
			byte third = remainCount >= 3 ? array[baseArrayIndex + 2] : (byte)0;

			output[baseOutputIndex + 0] = Base64Chars[first >> 2];
			output[baseOutputIndex + 1] = Base64Chars[((first & 0x03) << 4) | (second >> 4)];
			output[baseOutputIndex + 2] = remainCount >= 2 ? Base64Chars[((second & 0x0F) << 2) | (third >> 6)] : Base64Padding;
			output[baseOutputIndex + 3] = remainCount >= 3 ? Base64Chars[third & 0x3F] : Base64Padding;
		}

		//Faster than inbuilt
		public static string ToBase64(this byte[] array)
		{
			int encodedLength = array.Length.RoundUp(3) / 3 * 4;
			int remainingBytes = array.Length % 3;

			char[] output = new char[encodedLength];

			for (int i = 0; i < array.Length / 3; i++)
			{
				int baseArrayIndex = i * 3;
				byte first = array[baseArrayIndex + 0];
				byte second = array[baseArrayIndex + 1];
				byte third = array[baseArrayIndex + 2];

				int baseOutputIndex = i * 4;
				output[baseOutputIndex + 0] = Base64Chars[first >> 2];
				output[baseOutputIndex + 1] = Base64Chars[((first & 0x03) << 4) | (second >> 4)];
				output[baseOutputIndex + 2] = Base64Chars[((second & 0x0F) << 2) | (third >> 6)];
				output[baseOutputIndex + 3] = Base64Chars[third & 0x3F];
			}

			if (remainingBytes == 2)
			{
				int baseArrayIndex = array.Length - remainingBytes;
				byte first = array[baseArrayIndex + 0];
				byte second = array[baseArrayIndex + 1];

				int baseOutputIndex = output.Length - 4;
				output[baseOutputIndex + 0] = Base64Chars[first >> 2];
				output[baseOutputIndex + 1] = Base64Chars[((first & 0x03) << 4) | (second >> 4)];
				output[baseOutputIndex + 2] = Base64Chars[(second & 0x0F) << 2];
				output[baseOutputIndex + 3] = Base64Padding;
			}
			else if (remainingBytes == 1)
			{
				int baseArrayIndex = array.Length - remainingBytes;
				byte first = array[baseArrayIndex + 0];

				int baseOutputIndex = output.Length - 4;
				output[baseOutputIndex + 0] = Base64Chars[first >> 2];
				output[baseOutputIndex + 1] = Base64Chars[(first & 0x03) << 4];
				output[baseOutputIndex + 2] = Base64Padding;
				output[baseOutputIndex + 3] = Base64Padding;
			}

			return new string(output);
		}
	}
}
