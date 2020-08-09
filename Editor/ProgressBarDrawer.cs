using UnityEditor;
using UnityEngine;

namespace Vertx.Attributes.Editor
{
	[CustomPropertyDrawer(typeof(ProgressAttribute))]
	public class ProgressBarDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			position.height = EditorGUIUtility.singleLineHeight;

			EditorGUI.LabelField(position, label);

			position.y = position.yMax + EditorGUIUtility.standardVerticalSpacing;

			ProgressAttribute progressAttribute = (ProgressAttribute) attribute;
			float normalised = property.floatValue / progressAttribute.MaxValue;
			EditorGUI.ProgressBar(position, normalised, $"{normalised * 100:F2}");
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing;
	}
}