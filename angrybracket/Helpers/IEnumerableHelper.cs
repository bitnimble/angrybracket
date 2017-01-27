using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngryBracket
{
	public static class IEnumerableHelper
	{
		/// <summary>
		/// Returns pairs of elements.
		/// The resulting enumeration is half the length.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="enumerable"></param>
		/// <returns></returns>
		public static IEnumerable<Tuple<T, T>> Pair<T>(this IEnumerable<T> enumerable)
		{
			var enumerator = enumerable.GetEnumerator();
			while (enumerator.MoveNext())
			{
				var t1 = enumerator.Current;
				if (!enumerator.MoveNext())
					yield break;
				var t2 = enumerator.Current;
				yield return Tuple.Create(t1, t2);
			}
			yield break;
		}

		/// <summary>
		/// Returns each item in the original collection, paired with the previous item.
		/// The resulting enumerable is N-1 items long.
		/// If there is only 1 item, nothing is returned.
		/// </summary>
		public static IEnumerable<Tuple<T, T>> PairWithPrevious<T>(this IEnumerable<T> enumerable)
		{
			var enumerator = enumerable.GetEnumerator();
			if (!enumerator.MoveNext())
				yield break;
			T prev = enumerator.Current;
			while (enumerator.MoveNext())
			{
				var current = enumerator.Current;
				yield return Tuple.Create(prev, current);
				prev = current;
			}
			yield break;
		}

		/// <summary>
		/// Undoes PairWithPrevious, except when the original enumeration only had 1 element.
		/// </summary>
		public static IEnumerable<T> UnpairWithPrevious<T>(this IEnumerable<Tuple<T, T>> enumerable)
		{
			var enumerator = enumerable.GetEnumerator();
			if (!enumerator.MoveNext())
				yield break;

			Tuple<T, T> previous = enumerator.Current;
			yield return previous.Item1;

			while (enumerator.MoveNext())
			{
				previous = enumerator.Current;
				yield return previous.Item1;
			}
			yield return previous.Item2;
			yield break;
		}

		/// <summary>
		/// Returns triplets of elements, along with the index of the center element.
		/// The first and last elements are not returned, as they have no neighbours. To get these elements,
		/// use the overload taking a T defaultValue.
		/// </summary>
		/// <returns>Tuple&lt;previousT, currentT, nextT, currentIndex&gt;</returns>
		public static IEnumerable<Tuple<T, T, T, int>> GetWithNeighbours<T>(this IEnumerable<T> enumerable)
		{
			var enumerator = enumerable.GetEnumerator();
			if (!enumerator.MoveNext())
				yield break;
			T previous = enumerator.Current;

			if (!enumerator.MoveNext())
				yield break;
			T current = enumerator.Current;
			int i = 1;
			while (enumerator.MoveNext())
			{
				T next = enumerator.Current;

				yield return Tuple.Create(previous, current, next, i);

				previous = current;
				current = next;
				i++;
			}

			yield break;
		}

		/// <summary>
		/// Returns triplets of elements, along with the index of the center element.
		/// The first and last elements are returned with their left &amp; right values respectively given by the default value.
		/// </summary>
		/// <param name="defaultValue">Takes (index, current) returns other neighbour.
		/// If the sequence is of length 1, it will be invoked twice with the arguments (0, elem[0]).</param>
		/// <returns>Tuple&lt;previousT, currentT, nextT, currentIndex&gt;</returns>
		public static IEnumerable<Tuple<T, T, T, int>> GetWithNeighbours<T>(this IEnumerable<T> enumerable, Func<int, T, T> defaultValue)
		{
			var enumerator = enumerable.GetEnumerator();
			if (!enumerator.MoveNext())
				yield break; //Empty.
			T previous = enumerator.Current;

			if (!enumerator.MoveNext())
			{
				yield return Tuple.Create(defaultValue(0, previous), previous, defaultValue(0, previous), 0);
				yield break;
			}

			T current = enumerator.Current;
			yield return Tuple.Create(defaultValue(0, previous), previous, current, 0);

			int i = 1;
			while (enumerator.MoveNext())
			{
				T next = enumerator.Current;

				yield return Tuple.Create(previous, current, next, i);

				previous = current;
				current = next;
				i++;
			}

			yield return Tuple.Create(previous, current, defaultValue(i, current), i);
			yield break;
		}

		/// <summary>
		/// Returns triplets of elements, along with the index of the center element.
		/// The first and last elements are returned with their left &amp; right values respectively given by the default value.
		/// </summary>
		public static IEnumerable<Tuple<T, T, T, int>> GetWithNeighbours<T>(this IEnumerable<T> enumerable, T firstLeft, T lastRight)
		{
			var enumerator = enumerable.GetEnumerator();
			if (!enumerator.MoveNext())
				yield break; //Empty.
			T previous = enumerator.Current;

			if (!enumerator.MoveNext())
			{
				yield return Tuple.Create(firstLeft, previous, lastRight, 0);
				yield break;
			}

			T current = enumerator.Current;
			yield return Tuple.Create(firstLeft, previous, current, 0);

			int i = 1;
			while (enumerator.MoveNext())
			{
				T next = enumerator.Current;

				yield return Tuple.Create(previous, current, next, i);

				previous = current;
				current = next;
				i++;
			}

			yield return Tuple.Create(previous, current, lastRight, i);
			yield break;
		}

		/// <summary>
		/// Both bounds inclusive
		/// </summary>
		public static IEnumerable<int> CreateSequence(int min, int max)
		{
			for (int i = min; i <= max; i++)
				yield return i;
		}

		/// <summary>
		/// Both bounds inclusive
		/// </summary>
		public static IEnumerable<long> CreateSequence(long min, long max)
		{
			for (long i = min; i <= max; i++)
				yield return i;
		}

		/// <summary>
		/// Splits this enumerable into multiple enumerables. The first element of each returned sequence is the first
		/// element to return 'true' for splitCondition.
		/// Calling this instantly enumerates the entire source enumerable.
		/// </summary>
		public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> enumerable, Predicate<T> splitCondition)
		{
			List<List<T>> result = new List<List<T>>();
			List<T> current = new List<T>();
			foreach (var item in enumerable)
			{
				if (splitCondition(item))
				{
					if (current.Count > 0)
						result.Add(current);
					current = new List<T>();
				}
				current.Add(item);
			}
			if (current.Count > 0)
				result.Add(current);

			foreach (var item in result)
				yield return item;
			yield break;
		}

		public static IEnumerable<Tuple<T, U>> PairWith<T, U>(this IEnumerable<T> a, IEnumerable<U> b)
		{
			var ita = a.GetEnumerator();
			var itb = b.GetEnumerator();

			while (ita.MoveNext() && itb.MoveNext())
				yield return Tuple.Create(ita.Current, itb.Current);
		}

		/// <summary>
		/// Alias for String.Join(sep, IEnumerable)
		/// </summary>
		/// <param name="enumerable">The enumerable to join</param>
		/// <param name="sep">Will be placed between elements as they are joined. eg ", "</param>
		public static string JoinToString<T>(this IEnumerable<T> enumerable, string sep = "")
		{
			return string.Join(sep, enumerable);
		}

		/// <summary>
		/// Returns an enumerable containing the set difference between the two sets using the default comparer.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="enumerable"></param>
		/// <param name="other"></param>
		/// <returns></returns>
		public static IEnumerable<T> Difference<T>(this IEnumerable<T> enumerable, IEnumerable<T> other)
		{
			return enumerable.Except(other).Union(other.Except(enumerable));
		}

		/// <summary>
		/// Returns an enumerable containing the set difference between the two sets using the specified equality comparer.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="enumerable"></param>
		/// <param name="other"></param>
		/// <param name="comparer"></param>
		/// <returns></returns>
		public static IEnumerable<T> Difference<T>(this IEnumerable<T> enumerable, IEnumerable<T> other, IEqualityComparer<T> comparer)
		{
			return enumerable.Except(other, comparer).Union(other.Except(enumerable, comparer));
		}
	}
}
