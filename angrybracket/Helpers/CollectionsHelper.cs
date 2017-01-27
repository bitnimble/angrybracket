using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngryBracket
{
	public static class CollectionsHelper
	{
		/// <summary>
		/// So you can use new Queue { item1, item2 };
		/// </summary>
		public static void Add<T>(this Queue<T> queue, T item) { queue.Enqueue(item); }

		/// <summary>
		/// So you can use new Stack { item1, item2 };
		/// Last item is on top.
		/// </summary>
		public static void Add<T>(this Stack<T> stack, T item) { stack.Push(item); }

		/// <summary>
		/// Last item ends up on top.
		/// </summary>
		public static void PushRange<T>(this Stack<T> stack, IEnumerable<T> enumerable)
		{
			foreach (var item in enumerable)
				stack.Push(item);
		}

		public static void EnqueueRange<T>(this Queue<T> queue, IEnumerable<T> enumerable)
		{
			foreach (var item in enumerable)
				queue.Enqueue(item);
		}

		public static void AddRange<T>(this Stack<T> stack, IEnumerable<T> enumerable)
		{
			foreach (var item in enumerable)
				stack.Push(item);
		}

		public static void RemoveRange<T>(this ICollection<T> collection, IEnumerable<T> enumerable)
		{
			foreach (var item in enumerable)
				collection.Remove(item);
		}

		public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> enumerable)
		{
			foreach (var item in enumerable)
				collection.Add(item);
		}

		public static void AddRange<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, IEnumerable<Tuple<TKey, TValue>> enumerable)
		{
			foreach (var item in enumerable)
				dictionary.Add(item.Item1, item.Item2);
		}

		public static Stack<T> Clone<T>(this Stack<T> stack)
		{
			return new Stack<T>(stack.ToArray());
		}

		public static List<T> Clone<T>(this List<T> list) { return new List<T>(list); }

		/// <summary>
		/// Replaces the first occurence of toReplace in the list with replacement.
		/// </summary>
		public static void Replace<T>(this List<T> list, T toReplace, T replacement)
		{
			int index = list.IndexOf(toReplace);
			if (index != -1)
				list[index] = replacement;
		}

		/// <summary>
		/// Replaces the first occurence of toReplace in the list with the items specified in replacements.
		/// </summary>
		public static void Replace<T>(this List<T> list, T toReplace, IEnumerable<T> replacements)
		{
			int index = list.IndexOf(toReplace);
			if (index != -1)
				list.RemoveAt(index);

			list.InsertRange(index, replacements);
		}

		/// <summary>
		/// Replaces the first occurence of toReplace in the list with replacement.
		/// </summary>
		public static void ReplaceAll<T>(this List<T> list, T toReplace, T replacement)
		{
			var comparer = EqualityComparer<T>.Default;
			for (int i = 0; i < list.Count; i++)
				if (comparer.Equals(list[i], toReplace))
					list[i] = replacement;
		}

		public static bool IsEmpty<T>(this IReadOnlyCollection<T> collection) { return collection.Count == 0; }
	}
}
