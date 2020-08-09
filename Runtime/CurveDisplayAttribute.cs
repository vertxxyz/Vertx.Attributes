using UnityEngine;

namespace Vertx.Attributes
{
	/// <summary>
	/// Apply to an Animation Curve to restrict its Range and/or change its Color.
	/// </summary>
	public class CurveDisplayAttribute : PropertyAttribute
	{
		public enum CurveDisplay
		{
			RectAndColor,
			ColorOnly
		}

		public readonly CurveDisplay Display;
		public readonly Rect Rect;
		public readonly Color Color;

		public CurveDisplayAttribute(int minX, int minY, int maxX, int maxY)
		{
			Color = new Color(0.4f, 1, 0);
			Rect = new Rect(minX, minY, maxX, maxY);
			Display = CurveDisplay.RectAndColor;
		}

		public CurveDisplayAttribute(
			int minX,
			int minY,
			int maxX,
			int maxY,
			float r,
			float g,
			float b
		)
		{
			Rect = new Rect(minX, minY, maxX, maxY);
			Color = new Color(r, g, b);
			Display = CurveDisplay.RectAndColor;
		}

		public CurveDisplayAttribute(float r, float g, float b)
		{
			Color = new Color(r, g, b);
			Display = CurveDisplay.ColorOnly;
		}
	}
}