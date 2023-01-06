using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Vertx.Attributes.Editor
{
	[CustomPropertyDrawer(typeof(EditorOnlyAttribute))]
	public sealed class EditorOnlyDrawer : PropertyDrawer
	{
		public override float GetPropertyHeight(SerializedProperty property,
			GUIContent label) =>
			EditorGUI.GetPropertyHeight(property, label, true);

		public override void OnGUI(Rect position,
			SerializedProperty property,
			GUIContent label)
		{
			if (Application.IsPlaying(property.serializedObject.targetObject))
			{
				GUI.enabled = false;
				EditorGUI.PropertyField(position, property, label, true);
				GUI.enabled = true;
			}
			else
				EditorGUI.PropertyField(position, property, label, true);
		}
		
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var field = new PropertyField(property);
			field.SetEnabled(!Application.IsPlaying(property.serializedObject.targetObject));
			return field;
		}
	}
}