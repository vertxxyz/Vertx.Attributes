using UnityEngine;

namespace Vertx.Attributes
{
	/// <summary>
	/// Styles a field as a read-only progress bar.
	/// </summary>
	public class ProgressAttribute : PropertyAttribute
	{
		public readonly float MinValue;
		public readonly float MaxValue;
		
		/// <summary>
		/// Styles a field as a read-only progress bar.
		/// </summary>
		/// <param name="maxValue">The maximum value used by the field. This will be mapped to 100%.</param>
		public ProgressAttribute(float maxValue = 1) => MaxValue = maxValue;

		/// <summary>
		/// Styles a field as a read-only progress bar.
		/// </summary>
		/// <param name="minValue">The minimum value used by the field. This will be mapped to 0%.</param>
		/// <param name="maxValue">The maximum value used by the field. This will be mapped to 100%.</param>
		public ProgressAttribute(float minValue, float maxValue)
		{
			MinValue = minValue;
			MaxValue = maxValue;
		}
	}
}