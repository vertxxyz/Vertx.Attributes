using UnityEditor;
using UnityEngine;

namespace Vertx.Attributes.Editor
{
	[CustomPropertyDrawer(typeof(EditorOnlyAttribute))]
	public class EditorOnlyDrawer : PropertyDrawer
	{
		public override float GetPropertyHeight(SerializedProperty property,
			GUIContent label) =>
			EditorGUI.GetPropertyHeight(property, label, true);

		public override void OnGUI(Rect position,
			SerializedProperty property,
			GUIContent label)
		{
			if (Application.isPlaying)
			{
				GUI.enabled = false;
				EditorGUI.PropertyField(position, property, label, true);
				GUI.enabled = true;
			}
			else
				EditorGUI.PropertyField(position, property, label, true);
		}
	}
}