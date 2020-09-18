using UnityEngine;

namespace Vertx.Attributes
{
	public class ProgressAttribute : PropertyAttribute
	{
		public readonly float MaxValue;
		public ProgressAttribute(float maxValue = 1) => MaxValue = maxValue;
	}
}