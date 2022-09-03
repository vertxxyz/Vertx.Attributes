using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Vertx.Attributes.Editor
{
	[CustomPropertyDrawer(typeof(EnumFlagsAttribute))]
	public class EnumFlagsDrawer : PropertyDrawer
	{
		private delegate FieldInfo GetTypeFromPropertyBase(SerializedProperty property, out Type type);

		private static GetTypeFromPropertyBase s_GetTypeFromProperty;

		private static Type GetTypeFromProperty(SerializedProperty property)
		{
			if (s_GetTypeFromProperty == null)
			{
				MethodInfo method = Type.GetType("UnityEditor.ScriptAttributeUtility,UnityEditor").GetMethod("GetFieldInfoFromProperty", BindingFlags.Static | BindingFlags.NonPublic);
				s_GetTypeFromProperty = (GetTypeFromPropertyBase)Delegate.CreateDelegate(typeof(GetTypeFromPropertyBase), method);
			}

			s_GetTypeFromProperty(property, out Type type);
			return type;
		}

		public override VisualElement CreatePropertyGUI(SerializedProperty property) => new BetterEnumFlagsField(property, fieldInfo, ((EnumFlagsAttribute)attribute).HideObsoleteNames);

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var flagsAttribute = (EnumFlagsAttribute)attribute;
			EditorGUI.BeginProperty(position, label, property);
			if (flagsAttribute.RedZero && property.intValue == 0)
				GUI.color = new Color(1f, 0.46f, 0.51f);
			EnumFlagsFieldDistinct(position, label, property, fieldInfo, flagsAttribute.HideObsoleteNames);
			GUI.color = Color.white;
			EditorGUI.EndProperty();
		}

		private static void EnumFlagsFieldDistinct(Rect position, GUIContent label, SerializedProperty maskProperty, FieldInfo fieldInfo, bool hideObsoleteNames)
		{
			var valueAndNames = EnumFlagsValueAndNames.Get(fieldInfo, hideObsoleteNames);

			if (valueAndNames == null)
			{
				DrawFailedLabel();
				return;
			}

			position = EditorGUI.PrefixLabel(position, label);
			if (GUI.Button(position, valueAndNames.GetName(maskProperty.intValue), EditorStyles.layerMaskField))
				valueAndNames.DropDown(position, maskProperty);

			void DrawFailedLabel() => EditorGUI.HelpBox(position, $"{label.text} failed to be drawn by Enum Flags. Perhaps it is not a serializable enum type?", MessageType.Error);
		}
	}
}