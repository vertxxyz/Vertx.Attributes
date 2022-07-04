using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Vertx.Attributes.Editor
{
	[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
	public class ReadOnlyDrawer : PropertyDrawer
	{
		public override float GetPropertyHeight(SerializedProperty property,
			GUIContent label) =>
			EditorGUI.GetPropertyHeight(property, label, true);

		public override void OnGUI(Rect position,
			SerializedProperty property,
			GUIContent label)
		{
			GUI.enabled = false;
			EditorGUI.PropertyField(position, property, label, true);
			GUI.enabled = true;
		}
		
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var field = new PropertyField(property);
			field.SetEnabled(false);
			return field;
		}
	}
}