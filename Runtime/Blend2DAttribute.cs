using UnityEngine;

namespace Vertx.Attributes
{
	public sealed class Blend2DAttribute : PropertyAttribute
	{
		public readonly string XLabel;
		public readonly string YLabel;
		public readonly Vector2 Min;
		public readonly Vector2 Max;
		
		public Blend2DAttribute() : this("X", "Y") { }
		
		public Blend2DAttribute(float minX, float minY, float maxX, float maxY) : this("X", "Y", minX, minY, maxX, maxY) { }

		public Blend2DAttribute(string xLabel, string yLabel, float minX = -1, float minY = -1, float maxX = 1, float maxY = 1)
		{
			XLabel = xLabel;
			YLabel = yLabel;
			Min = new Vector2(minX, minY);
			Max = new Vector2(maxX, maxY);
		}
	}
}