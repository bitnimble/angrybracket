using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AngryBracket
{
	public static class ThreadHelper
	{
		public static void Multithread<T>(this IEnumerable<T> ienum, int threadCount, Action<T> action)
		{
			Multithread(ienum, threadCount, (i, t) => action(t));
		}

		public static void Multithread<T>(this IEnumerable<T> ienum, int threadCount, Action<int, T> action)
		{
			Thread[] pool = new Thread[threadCount];
			IEnumerator<T> enumerator = ienum.GetEnumerator();
			bool done = false;

			for (int i = 0; i < threadCount; i++)
			{
				pool[i] = new Thread((j) =>
				{
					while (true)
					{
						T current;

						lock (ienum)
						{
							if (done || !enumerator.MoveNext())
							{
								done = true;
								break;
							}
							current = enumerator.Current;
						}

						action((int)j, current);
					}
				});
			}
			for (int i = 0; i < threadCount; i++)
			{
				pool[i].Start(i);
			}

			for (int i = 0; i < threadCount; i++)
			{
				pool[i].Join();
			}
		}
	}
}
