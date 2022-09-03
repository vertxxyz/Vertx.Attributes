using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Vertx.Attributes.Editor
{
	internal class PropertyFieldWithoutLabel : PropertyField
	{
		public PropertyFieldWithoutLabel(SerializedProperty serializedProperty) : base(serializedProperty) => RegisterCallback<GeometryChangedEvent, VisualElement>((_, root) =>
		{
			if (root.childCount == 0)
				return;
			foreach (var child in root.Children())
			{
				if (!child.ClassListContains(BaseField<int>.ussClassName + "__inspector-field"))
					continue;
				for (int i = child.childCount - 1; i >= 0; i--)
				{
					if (child[i] is Label label)
						label.RemoveFromHierarchy();
				}
			}
		}, this);
	}
}