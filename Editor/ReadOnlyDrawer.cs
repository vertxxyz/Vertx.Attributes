using UnityEditor;
using UnityEngine.UIElements;

namespace Vertx.Attributes.Editor
{
	[CustomPropertyDrawer(typeof(ReadOnlyFieldAttribute))]
	public sealed class ReadOnlyDrawer : DecoratorDrawer
	{
		public override VisualElement CreatePropertyGUI() => new DecoratePropertyElement((_, element) =>
		{
			element.SetEnabled(false);
		}) { name = nameof(ReadOnlyDrawer) };
	}
}