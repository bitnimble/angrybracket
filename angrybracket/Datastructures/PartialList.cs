using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngryBracket
{
	public static class __PartialListExt
	{
		public static PartialList<T> ToPartialList<T>(this IReadOnlyList<T> list, int startIndex, int count)
		{
			return new PartialList<T>(list, startIndex, count);
		}
	}

	public class PartialList<T> : IReadOnlyList<T>, IList<T>
	{
		IReadOnlyList<T> Source;
		int _Count;
		int StartIndex;

		public PartialList(IReadOnlyList<T> source, int startIndex, int count)
		{
			Source = source;
			_Count = count;
			StartIndex = startIndex;
		}

		public T this[int index]
		{
			get
			{
				if (index >= Count)
					throw new IndexOutOfRangeException();

				return Source[StartIndex + index];
			}
		}

		public int Count { get { return _Count; } }

		public bool IsReadOnly { get { return true; } }

		T IList<T>.this[int index]
		{
			get { return this[index]; }
			set { throw new InvalidOperationException(); }
		}

		public IEnumerator<T> GetEnumerator()
		{
			for (int i = 0; i < Count; i++)
				yield return this[i];
		}

		IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

		public int IndexOf(T item)
		{
			for (int i = 0; i < Count; i++)
				if (this[i].Equals(item))
					return i;
			return -1;
		}

		public void Insert(int index, T item) { throw new InvalidOperationException(); }
		public void RemoveAt(int index) { throw new InvalidOperationException(); }
		public void Add(T item) { throw new InvalidOperationException(); }
		public void Clear() { throw new InvalidOperationException(); }
		public bool Contains(T item) { return IndexOf(item) != -1; }
		public void CopyTo(T[] array, int arrayIndex) { throw new InvalidOperationException(); } //Could be implemented, but isn't.
		public bool Remove(T item) { throw new InvalidOperationException(); }
	}
}
