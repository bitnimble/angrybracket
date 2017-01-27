using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AngryBracket
{
	public class NaturalSortComparer : IComparer<string>
	{
		//NumericLength assumes that the string is numerically valid
		private int NumericLength(string s)
		{
			int i = 0;
			for (; i < s.Length; i++)
			{
				if (s[i] != '0')
				{
					break;
				}
			}

			return s.Length - i;
		}

		public int Compare(string a, string b)
		{
			string[] aAlphaNumParts = Regex.Split(a, "([\\d]+)");
			string[] bAlphaNumParts = Regex.Split(b, "([\\d]+)");

			int i = 0;
			while (true)
			{

				bool aHasPart = i < aAlphaNumParts.Length;
				bool bHasPart = i < bAlphaNumParts.Length;

				if (!aHasPart && !bHasPart)
					return 0;

				if (!aHasPart)
					return -1;
				if (!bHasPart)
					return 1;

				string aPart = aAlphaNumParts[i];
				string bPart = bAlphaNumParts[i];
				int aLength = NumericLength(aPart);
				int bLength = NumericLength(bPart);

				i++;
				if (aLength == 0 && bLength == 0)
					continue;
				if (aLength == 0)
					return -1;
				if (bLength == 0)
					return 1;

				bool isNumber = false;
				if (aPart[0] >= '0' && aPart[0] <= '9')
					isNumber = true;

				int comparison;
				if (isNumber)
				{
					int lengthDiff = aLength - bLength;
					if (lengthDiff != 0)
						comparison = lengthDiff;
					else
						comparison = string.Compare(aPart, bPart, StringComparison.InvariantCulture);
				}
				else
					comparison = string.Compare(aPart, bPart, StringComparison.InvariantCulture);

				if (comparison != 0)
					return comparison;

			}
		}
	}
}
