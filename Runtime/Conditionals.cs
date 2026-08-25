using UnityEngine;

namespace Vertx.Attributes
{
	public struct Conditional
	{
		public string FieldName;
		public Comparator Comparator;
	}

	public enum Comparator
	{
		And,
		Or
	}
	
	public class ShowIfAttribute : PropertyAttribute
	{
		public readonly Conditional[] Arguments;

		public ShowIfAttribute(params Conditional[] arguments) => Arguments = arguments;
	}
	
	public class HideIfAttribute : PropertyAttribute
	{
		public readonly Conditional[] Arguments;

		public HideIfAttribute(params Conditional[] arguments) => Arguments = arguments;
	}
}