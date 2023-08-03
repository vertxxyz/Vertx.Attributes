using UnityEngine;

namespace Vertx.Attributes
{
	public sealed class ButtonAttribute : PropertyAttribute
	{
		public string MethodName { get; }

		public enum Location : byte
		{
			Bottom,
			Above,
			Below
		}
		
		public string DisplayNameOverride { get; set; } = null;
		public Location DisplayLocation { get; set; } = Location.Bottom;

		public ButtonAttribute(string methodName)
		{
			MethodName = methodName;
		}
	}
}