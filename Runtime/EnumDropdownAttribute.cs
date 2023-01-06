using UnityEngine;

namespace Vertx.Attributes
{
	/// <summary>
	/// Shows an AdvancedDropdown instead of a GenericMenu.<br/>
	/// This allows for proper scrolling on Windows, and is pretty much invaluable for large enums.
	/// </summary>
	public class EnumDropdownAttribute : PropertyAttribute
	{
		/// <summary>
		/// Tints the 0/None enum as red if it's assigned in the inspector.
		/// </summary>
		public bool RedZero { get; set; }
	}
}