using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngryBracket
{
	public static class DrawingHelper
	{
		public static Point GetCenter(this Rectangle r)
		{
			return new Point(r.X + r.Width / 2, r.Y + r.Height / 2);
		}
		public static PointF GetCenter(this RectangleF r)
		{
			return new PointF(r.X + r.Width / 2.0f, r.Y + r.Height / 2.0f);
		}

		public static PointF ToPointF(this Point point)
		{
			return new PointF((float)point.X, (float)point.Y);
		}

		public static Point ToPoint(this PointF pointf)
		{
			return new Point((int)pointf.X, (int)pointf.Y);
		}

		public static SizeF ToSizeF(this Size size)
		{
			return new SizeF(size.Width, size.Height);
		}

		/// <summary>
		/// Different semantics to Point.Offset -- struct extensions cannot modify this.
		/// </summary>
		public static PointF Translate(this PointF p, PointF other)
		{
			p.X = p.X + other.X;
			p.Y = p.Y + other.Y;
			return p;
		}

		/// <summary>
		/// Different semantics to Point.Offset -- struct extensions cannot modify this.
		/// </summary>
		public static Point Translate(this Point p, Point other)
		{
			p.X = p.X + other.X;
			p.Y = p.Y + other.Y;
			return p;
		}

		/// <summary>
		/// Different semantics to Point.Offset -- struct extensions cannot modify this.
		/// </summary>
		public static Point Translate(this Point p, int dx, int dy)
		{
			p.X = p.X + dx;
			p.Y = p.Y + dy;
			return p;
		}

		public static RectangleF ToRectangleF(this Rectangle rect)
		{
			return new RectangleF(rect.X, rect.Y, rect.Width, rect.Height);
		}

		public static Rectangle ToRectangle(this RectangleF rect)
		{
			return new Rectangle((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
		}
	}
}
