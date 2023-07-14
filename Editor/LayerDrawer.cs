using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Vertx.Attributes.Editor
{
	[CustomPropertyDrawer(typeof(LayerAttribute))]
	public sealed class LayerDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (property.propertyType == SerializedPropertyType.Integer)
			{
				EditorGUI.BeginChangeCheck();
				int layer = EditorGUI.LayerField(position, label, property.intValue);
				if (EditorGUI.EndChangeCheck())
					property.intValue = layer;
			}
			else
			{
				EditorGUI.LabelField(position, label.text, $"{nameof(LayerAttribute)} shouldn't be applied to {property.propertyType}, it's only valid on ints.");
			}
		}

#if UNITY_2021_1_OR_NEWER
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			LayerField layerField = new LayerField(property.displayName) { bindingPath = property.propertyPath };
			layerField.AddToClassList(StyleSheetUtils.AlignedFieldUssClassName);
			return layerField;
		}
#endif
	}
}