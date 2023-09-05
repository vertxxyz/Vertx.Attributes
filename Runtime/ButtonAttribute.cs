using System;
using UnityEngine;

namespace Vertx.Attributes
{
	public sealed class ButtonAttribute : PropertyAttribute
	{
		public string MethodName { get; }
		public Type StaticMethodType { get; }

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
		
		public ButtonAttribute(string methodName, Type staticMethodType)
		{
			MethodName = methodName;
			StaticMethodType = staticMethodType;
		}
	}
}