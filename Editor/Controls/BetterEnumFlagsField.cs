#if UNITY_2021_1_OR_NEWER
using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Vertx.Attributes.Editor
{
	public sealed class BetterEnumFlagsField : VisualElement
	{
		private delegate FieldInfo GetTypeFromPropertyBase(SerializedProperty property, out Type type);

		private static GetTypeFromPropertyBase? s_getTypeFromProperty;

		private Type GetTypeFromProperty(SerializedProperty property)
		{
			if (s_getTypeFromProperty == null)
			{
				MethodInfo method = Type.GetType("UnityEditor.ScriptAttributeUtility,UnityEditor")!.GetMethod("GetFieldInfoFromProperty", BindingFlags.Static | BindingFlags.NonPublic)!;
				s_getTypeFromProperty = (GetTypeFromPropertyBase)Delegate.CreateDelegate(typeof(GetTypeFromPropertyBase), method);
			}

			s_getTypeFromProperty(property, out Type type);
			return type;
		}

		public BetterEnumFlagsField(SerializedProperty property, FieldInfo fieldInfo, bool hideObsoleteNames)
		{
			EnumFlagsValueAndNames? enumFlagsValueAndNames = EnumFlagsValueAndNames.Get(fieldInfo, hideObsoleteNames);
			if (enumFlagsValueAndNames == null)
			{
				return;
			}
			
			var dropdownButton = new DropdownButton(property.displayName, enumFlagsValueAndNames.GetName(property.intValue));
			dropdownButton.RegisterClickCallback<(SerializedProperty property, EnumFlagsValueAndNames valuesAndNames)>(
				(_, button, data) =>
				{
					Rect bounds = button.Q<VisualElement>(null, DropdownButton.DropdownUssClassName)?.worldBound
					              ?? button.worldBound;
					data.valuesAndNames.DropDown(bounds, data.property);
				},
				(property, enumFlagsValueAndNames)
			);
			dropdownButton.TrackPropertyValue(property, p => dropdownButton.Text = enumFlagsValueAndNames.GetName(p.intValue));
			Add(dropdownButton);
		}
	}
}
#endif