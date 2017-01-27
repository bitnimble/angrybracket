using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngryBracket
{
	public class CircularBuffer<T> : IEnumerable<T>
	{
		T[] items;
		int s, e;
		int count;

		//index -> items.size
		//0 -> index - 1


		public CircularBuffer(int capacity)
		{
			if (capacity <= 1)
				throw new ArgumentOutOfRangeException("capacity", "Capacity must be greater than one");
			items = new T[capacity];
			s = e = 0;
		}

		public void Push(T item)
		{
			e = (e + 1) % items.Length;
			items[e] = item;
			if (s == e) s = (s + 1) % items.Length;
			count = Math.Min(items.Length, count + 1);
		}

		/// <summary>
		/// Resets the collection but does not allow items to be garbage collected.
		/// </summary>
		public void Clear()
		{
			s = e = count = 0;
		}

		public int Count { get { return count; } }

		public int Capacity { get { return items.Length; } }

		public T this[int i]
		{
			get
			{
				if (i >= count || i < 0)
					throw new IndexOutOfRangeException("Index must be between 0 and " + count);

				return items[(s + i) % items.Length];
			}
		}

		public IEnumerator<T> GetEnumerator()
		{
			for (int i = 0; i < count; i++)
				yield return items[(s + i) % items.Length];
			yield break;
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
