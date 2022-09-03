using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Vertx.Attributes.Editor
{
	public class BetterEnumFlagsField : VisualElement
	{
		private EnumFlagsValueAndNames _enumFlagsValueAndNames;

		private delegate FieldInfo GetTypeFromPropertyBase(SerializedProperty property, out Type type);

		private static GetTypeFromPropertyBase s_GetTypeFromProperty;

		private Type GetTypeFromProperty(SerializedProperty property)
		{
			if (s_GetTypeFromProperty == null)
			{
				MethodInfo method = Type.GetType("UnityEditor.ScriptAttributeUtility,UnityEditor").GetMethod("GetFieldInfoFromProperty", BindingFlags.Static | BindingFlags.NonPublic);
				s_GetTypeFromProperty = (GetTypeFromPropertyBase)Delegate.CreateDelegate(typeof(GetTypeFromPropertyBase), method);
			}

			s_GetTypeFromProperty(property, out Type type);
			return type;
		}

		public BetterEnumFlagsField(SerializedProperty property, FieldInfo fieldInfo, bool hideObsoleteNames) : base()
		{
			_enumFlagsValueAndNames = EnumFlagsValueAndNames.Get(fieldInfo, hideObsoleteNames);
			var dropdownButton = new DropdownButton(property.displayName, _enumFlagsValueAndNames.GetName(property.intValue));
			dropdownButton.RegisterClickCallback<(SerializedProperty property, EnumFlagsValueAndNames valuesAndNames)>(
				(_, button, data) =>
				{
					Rect bounds = button.Q<VisualElement>(null, DropdownButton.DropdownUssClassName)?.worldBound
					              ?? button.worldBound;
					data.valuesAndNames.DropDown(bounds, data.property);
				},
				(property, _enumFlagsValueAndNames)
			);
			dropdownButton.TrackPropertyValue(property, p => dropdownButton.Text = _enumFlagsValueAndNames.GetName(p.intValue));
			Add(dropdownButton);
		}
	}
}