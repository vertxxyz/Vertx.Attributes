using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Vertx.Attributes.Editor
{
	[CustomPropertyDrawer(typeof(RelabelAttribute))]
	public sealed class RelabelDrawer : PropertyDrawer
	{
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
			=> EditorGUI.GetPropertyHeight(property, label, true);

		public override void OnGUI(Rect position,
			SerializedProperty property,
			GUIContent label)
		{
			label.text = ((RelabelAttribute)attribute).Name;
			EditorGUI.PropertyField(position, property, label, true);
		}

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
			=> new PropertyField(property, ((RelabelAttribute)attribute).Name);
	}
}