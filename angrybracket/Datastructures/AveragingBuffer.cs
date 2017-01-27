using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngryBracket
{
	public class AveragingBuffer
	{
		CircularBuffer<double> values;
		double runningTotal;
		int bufferSize;

		public double CurrentAverage
		{
			get { return runningTotal / values.Count; }
		}

		public AveragingBuffer(int size)
		{
			if (size <= 0)
				throw new Exception("AveragingBuffer must be initialised with a positive size");
			bufferSize = size;
			values = new CircularBuffer<double>(bufferSize);
			values.Push(0);
		}

		public double Push(double value)
		{
			if (double.IsNaN(value) || double.IsInfinity(value))
				return runningTotal / values.Count;
			if (values.Count == values.Capacity)
				runningTotal -= values[0];
			runningTotal += value;
			values.Push(value);

			return runningTotal / values.Count;
		}
	}
}
