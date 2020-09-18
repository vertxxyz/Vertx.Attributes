using UnityEngine;

namespace Vertx.Attributes
{
	public class MinMaxAttribute : PropertyAttribute
	{
		public readonly float Min;
		public readonly float Max;
		public readonly GUIContent Label;

		public MinMaxAttribute(string label, float min, float max)
		{
			Label = new GUIContent(label);
			Min = min;
			Max = max;
		}
		
		public MinMaxAttribute(float min, float max)
		{
			Min = min;
			Max = max;
		}
	}
}