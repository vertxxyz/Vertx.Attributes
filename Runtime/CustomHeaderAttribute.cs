using UnityEngine;

namespace Vertx.Attributes
{
	/// <summary>
	/// Similar to <see cref="HeaderAttribute"/>, but properties like font size and margin can be altered.
	/// </summary>
	public sealed class CustomHeaderAttribute : PropertyAttribute
	{
		public string LabelText { get; }
		public float MarginTop { get; set; } = float.NaN;
		public float MarginRight { get; set; } = float.NaN;
		public float MarginBottom { get; set; } = float.NaN;
		public float MarginLeft { get; set; } = float.NaN;
		public float FontSize { get; set; } = float.NaN;
		
		public CustomHeaderAttribute(string labelText) => LabelText = labelText;
	}
}