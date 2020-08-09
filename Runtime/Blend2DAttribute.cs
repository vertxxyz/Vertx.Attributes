using System;
using UnityEngine;

namespace Vertx.Attributes
{
	[AttributeUsage(AttributeTargets.Field)]
	public class Blend2DAttribute : PropertyAttribute
	{
		public readonly string XLabel;
		public readonly string YLabel;
		public readonly Vector2 Min;
		public readonly Vector2 Max;

		public Blend2DAttribute(string xLabel, string yLabel)
		{
			XLabel = xLabel;
			YLabel = yLabel;
			Min = Vector2.one * -1;
			Max = Vector2.one;
		}

		public Blend2DAttribute(string xLabel, string yLabel, float minX, float minY, float maxX, float maxY)
		{
			XLabel = xLabel;
			YLabel = yLabel;
			Min = new Vector2(minX, minY);
			Max = new Vector2(maxX, maxY);
		}
	}
}