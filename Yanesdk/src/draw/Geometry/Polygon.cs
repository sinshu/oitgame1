using System;
using System.Collections.Generic;
using System.Text;

namespace Yanesdk.Draw
{
	/// <summary>
	/// ���p�`
	/// </summary>
	public class Polygon : List<Point>
	{
		#region ctor
		public Polygon() { }
		public Polygon(Point[] p) { AddRange(p); }
		#endregion
	}
}
