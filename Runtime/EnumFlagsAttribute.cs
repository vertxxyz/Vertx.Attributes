using UnityEngine;

namespace Vertx.Attributes
{
	/// <summary>
	/// Displays a enum bit field with multiple values instead of displaying the default 'Mixed'.
	/// </summary>
	public class EnumFlagsAttribute : PropertyAttribute
	{
		/// <summary>
		/// Tints the 0/None enum as red if it's assigned in the inspector.
		/// </summary>
		public bool RedZero;
		
		/// <summary>
		/// Hides enum values marked with <see cref="System.ObsoleteAttribute"/>.
		/// </summary>
		public bool HideObsoleteNames = true;
	}
}