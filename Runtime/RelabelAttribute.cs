using UnityEngine;

namespace Vertx.Attributes
{
	public class RelabelAttribute : PropertyAttribute
	{
		public readonly string Name;
		
		public RelabelAttribute(string name) => Name = name;
	}
}