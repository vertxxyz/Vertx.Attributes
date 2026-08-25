using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Vertx.Attributes.Editor
{
	[CustomPropertyDrawer(typeof(InlineAttribute))]
	public sealed class InlineDrawer : DecoratorDrawer
	{
		public override VisualElement CreatePropertyGUI() => new DecoratePropertyElement((property, element) =>
		{
			if (property.propertyType == SerializedPropertyType.ObjectReference)
			{
				if (element.parent[element.parent.childCount - 1] is InspectorElement) return;
				if (property.objectReferenceValue == null) return;
				element.parent.Add(new InspectorElement(property.objectReferenceValue));
				return;
			}
			
			property.isExpanded = true;
			if (property.hasChildren)
				element.RegisterCallback<GeometryChangedEvent, VisualElement>(RemoveFoldout, element);
		}) { name = nameof(InlineDrawer) };

		private void RemoveFoldout(GeometryChangedEvent change, VisualElement field) => RemoveFoldout(field);

		private void RemoveFoldout(VisualElement field)
		{
			Foldout foldout = field as Foldout ?? field.Q<Foldout>();
			if (foldout == null)
			{
				return;
			}

			if (foldout.hierarchy.childCount > 0 && foldout.hierarchy[0] is Toggle toggle)
			{
				toggle.style.display = DisplayStyle.None;
			}
			
			VisualElement contentContainer = foldout.contentContainer;
			contentContainer.style.marginLeft = 0;
			field.UnregisterCallback<GeometryChangedEvent, VisualElement>(RemoveFoldout);
		}
	}
}