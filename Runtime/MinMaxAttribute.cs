using UnityEngine;

namespace Vertx.Attributes
{
	public class MinMaxAttribute : PropertyAttribute
	{
		public readonly float Min;
		public readonly float Max;
		public readonly GUIContent Label;
		
		/// <summary>
		/// UIToolkit-specific setting that aligns fields in the inspector.
		/// When enabled the space taken up by the slider will be smaller, but the input will align with other inspector fields.
		/// </summary>
		public bool Aligned { get; set; }

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