using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Vertx.Attributes.Editor
{
	[CustomPropertyDrawer(typeof(InlineAttribute))]
	public class InlineDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (!property.hasChildren)
			{
				EditorGUI.PropertyField(position, property);
				return;
			}

			SerializedProperty end = property.GetEndProperty();
			bool enter = true;
			while (property.NextVisible(enter) && !SerializedProperty.EqualContents(end, property))
			{
				enter = false;
				position.height = EditorGUI.GetPropertyHeight(property);
				EditorGUI.PropertyField(position, property, true);
				position.y = position.yMax + EditorGUIUtility.standardVerticalSpacing;
			}
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			if (!property.hasChildren)
				return EditorGUI.GetPropertyHeight(property, label);

			property.isExpanded = true;
			return EditorGUI.GetPropertyHeight(property, label) - EditorGUIUtility.singleLineHeight;
		}

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			property.isExpanded = true;
			PropertyField field = new PropertyField(property);
			if (property.hasChildren)
				field.RegisterCallback<GeometryChangedEvent>(RemoveFoldout);
			return field;

			void RemoveFoldout(GeometryChangedEvent evt)
			{
				var foldout = field.Q<Foldout>();
				if (foldout == null)
					return;
				VisualElement contentContainer = foldout.contentContainer;
				if (contentContainer.childCount == 0)
					return;

				VisualElement root = new VisualElement();

				for (int i = contentContainer.childCount - 1; i >= 0; i--)
				{
					VisualElement element = contentContainer[i];
					contentContainer.RemoveAt(i);
					root.Add(element);
				}

				foldout.RemoveFromHierarchy();
				field.Add(root);
				field.UnregisterCallback<GeometryChangedEvent>(RemoveFoldout);
			}
		}
	}
}