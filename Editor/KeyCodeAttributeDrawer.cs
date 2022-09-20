using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace Vertx.Attributes.Editor
{
	[CustomPropertyDrawer(typeof(KeyCodeAttribute))]
	public sealed class KeyCodeAttributeDrawer : EnumDropdownDrawer
	{
		private GUIStyle _objectFieldStyle;
		private GUIStyle ObjectFieldStyle => _objectFieldStyle ?? (_objectFieldStyle = "IN ObjectField");

		private const float widthInput = 18;

#if UNITY_2020_1_OR_NEWER
		public const string UssClassName = "vertx-keycode-dropdown";
		public const string PickerUssClassName = UssClassName + "__picker";
		public const string ActivePickerUssClassName = PickerUssClassName + "--active";

		private class AcceptInput : VisualElement
		{
			public AcceptInput(SerializedProperty property)
			{
				AddToClassList(ObjectField.selectorUssClassName);
				AddToClassList(PickerUssClassName);
				RegisterCallback<ClickEvent, SerializedProperty>((evt, args) =>
				{
					var target = (VisualElement)evt.target;
					target.focusable = true;
					target.Focus();
					target.AddToClassList(ActivePickerUssClassName);
					target.RegisterCallback<KeyDownEvent, SerializedProperty>(CaptureKeyboard, args);
					evt.StopPropagation();
				}, property);
				RegisterCallback<FocusOutEvent>(evt =>
				{
					var target = (VisualElement)evt.target;
					target.focusable = false;
					target.RemoveFromClassList(ActivePickerUssClassName);
					target.UnregisterCallback<KeyDownEvent, SerializedProperty>(CaptureKeyboard);
					evt.StopPropagation();
				});
			}

			private static void CaptureKeyboard(KeyDownEvent evt, SerializedProperty p)
			{
				var target = (VisualElement)evt.target;
				if (evt.keyCode != KeyCode.Escape)
				{
					p.intValue = (int)Event.current.keyCode;
					p.serializedObject.ApplyModifiedProperties();
				}

				target.Blur();
			}
		}


		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			VisualElement root = base.CreatePropertyGUI(property);
			root.AddToClassList(UssClassName);
			root.Add(new AcceptInput(property));
			return root;
		}
#endif

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			position.width -= widthInput;

			Rect pickerRect = new Rect(position.xMax, position.y, widthInput, position.height);
			int id = GUIUtility.GetControlID((int)pickerRect.x, FocusType.Keyboard, pickerRect);
			GUI.color = GUIUtility.keyboardControl == id ? Color.green : Color.white;

			base.OnGUI(position, property, label);
			position.x += position.width;
			position.width = widthInput;

			if (Event.current.type == EventType.MouseDown && position.Contains(Event.current.mousePosition))
			{
				GUIUtility.keyboardControl = id;
				Event.current.Use();
			}

			position.y -= 2;
			position.x += 1;
			GUI.Label(position, GUIContent.none, ObjectFieldStyle);
			if (GUIUtility.keyboardControl == id && Event.current.type == EventType.KeyUp)
			{
				if (Event.current.keyCode != KeyCode.Escape)
					property.intValue = (int)Event.current.keyCode;
				Event.current.Use();
				GUIUtility.keyboardControl = -1;
			}
			else if (GUIUtility.keyboardControl == id && Event.current.isKey)
				Event.current.Use();

			GUI.color = Color.white;
		}
	}
}