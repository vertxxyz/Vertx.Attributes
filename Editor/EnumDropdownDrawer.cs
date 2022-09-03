using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Vertx.Attributes.Editor
{
	[CustomPropertyDrawer(typeof(EnumDropdownAttribute))]
	public class EnumDropdownDrawer : PropertyDrawer
	{
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var dropdownButton = new DropdownButton(
				property.displayName,
				property.enumDisplayNames[property.enumValueIndex]
			);
			dropdownButton.TrackPropertyValue(
				property,
				serializedProperty => dropdownButton.Text = serializedProperty.enumDisplayNames[serializedProperty.enumValueIndex]
			);
			dropdownButton.RegisterClickCallback((_, button, data) =>
			{
				Rect position = button.worldBound;
				var dropdown = new NaiveAdvancedDropdown<SerializedProperty>(
					new Vector2(position.width, 300),
					data.displayName,
					data.enumDisplayNames,
					(i, p) =>
					{
						p.enumValueIndex = i;
						p.serializedObject.ApplyModifiedProperties();
					},
					data);
				dropdown.Show(position);
			}, property);
			return dropdownButton;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			using (var scope = new EditorGUI.PropertyScope(position, label, property))
			{
				var buttonRect = EditorGUI.PrefixLabel(position, scope.content);
				if (((EnumDropdownAttribute)attribute).RedZero && property.enumValueIndex == 0)
					GUI.color = new Color(1f, 0.46f, 0.51f);
				if (!GUI.Button(buttonRect, property.enumDisplayNames[property.enumValueIndex], EditorStyles.popup))
				{
					GUI.color = Color.white;
					return;
				}

				GUI.color = Color.white;
			}

			var dropdown = new NaiveAdvancedDropdown<SerializedProperty>(
				new Vector2(position.width, 300),
				property.displayName,
				property.enumDisplayNames,
				(i, p) =>
				{
					p.enumValueIndex = i;
					p.serializedObject.ApplyModifiedProperties();
				},
				property
			);
			dropdown.Show(position);
		}
	}
}