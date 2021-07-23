using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Vertx.Attributes.Editor
{
	[CustomPropertyDrawer(typeof(EnumFlagsAttribute))]
	public class EnumFlagsDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var flagsAttribute = (EnumFlagsAttribute)attribute;
			Type enumType = fieldInfo.FieldType;
			if (!enumType.IsEnum)
			{
				if (enumType.IsGenericType)
					enumType = enumType.GetGenericArguments().Single();
				if (enumType.IsArray)
					enumType = enumType.GetElementType();
			}

			EditorGUI.BeginProperty(position, label, property);
			if (flagsAttribute.RedZero && property.intValue == 0)
				GUI.color = new Color(1f, 0.46f, 0.51f);
			EnumFlagsFieldDistinct(position, label, property, enumType);
			GUI.color = Color.white;
			EditorGUI.EndProperty();
		}

		private static readonly Dictionary<Type, ValueAndNames> lookup = new Dictionary<Type, ValueAndNames>();

		private class ValueAndNames
		{
			private readonly string noneName = "Nothing";
			private readonly Dictionary<int, string> valueToNames = new Dictionary<int, string>();
			private readonly Dictionary<int, string> complexNameLookup = new Dictionary<int, string>();
			private readonly int everythingValue;

			public ValueAndNames(Type enumType)
			{
				string[] names = Enum.GetNames(enumType);
				Array values = Enum.GetValues(enumType);
				everythingValue = 0;
				for (int i = 0; i < values.Length; i++)
				{
					int value = (int)values.GetValue(i);
					string nicifiedName = ObjectNames.NicifyVariableName(names[i]);
					everythingValue |= value;

					if (value == 0)
					{
						noneName = nicifiedName;
						continue;
					}

					if (valueToNames.TryGetValue(value, out string name))
					{
						valueToNames[value] = $"{name} | {nicifiedName}";
						continue;
					}

					valueToNames.Add(value, nicifiedName);
				}

				valueToNames.Add(0, noneName);
			}

			public string GetName(int value)
			{
				if (value == everythingValue) return "Everything";
				if (valueToNames.TryGetValue(value, out var result))
					return result;

				if (complexNameLookup.TryGetValue(value, out result))
					return result;

				StringBuilder stringBuilder = new StringBuilder();
				bool hitMax = false;
				foreach (KeyValuePair<int, string> pair in valueToNames)
				{
					if (pair.Key == 0) continue;
					if ((pair.Key & value) == 0) continue;
					if (!IsPowerOfTwo(pair.Key)) continue;
					stringBuilder.Append(pair.Value);
					if (stringBuilder.Length > 80)
					{
						stringBuilder.Append("...");
						hitMax = true;
						break;
					}
					stringBuilder.Append(", ");
				}

				if (hitMax)
				{
					hitMax = false;
					StringBuilder secondaryBuilder = new StringBuilder("Not ");
					foreach (KeyValuePair<int, string> pair in valueToNames)
					{
						if (pair.Key == 0) continue;
						if ((pair.Key & value) != 0) continue;
						if (!IsPowerOfTwo(pair.Key)) continue;
						secondaryBuilder.Append(pair.Value);
						if (secondaryBuilder.Length > 80)
						{
							hitMax = true;
							break;
						}
						secondaryBuilder.Append(", ");
					}

					if (!hitMax)
						stringBuilder = secondaryBuilder;
				}

				if (stringBuilder.Length != 0)
				{
					if(!hitMax) // Remove the last ", "
						stringBuilder.Remove(stringBuilder.Length - 2, 2);
					complexNameLookup.Add(value, stringBuilder.ToString());
				}
				else
				{
					complexNameLookup.Add(value, "⚠️ Invalid ⚠️");
				}

				return complexNameLookup[value];
			}

			public void DropDown(Rect rect, SerializedProperty property)
			{
				GenericMenu menu = new GenericMenu();

				int originalValue = property.intValue;

				menu.AddItem(new GUIContent(noneName), originalValue == 0, () =>
				{
					property.intValue = 0;
					property.serializedObject.ApplyModifiedProperties();
				});
				menu.AddItem(new GUIContent("Everything"), originalValue == everythingValue, () =>
				{
					property.intValue = everythingValue;
					property.serializedObject.ApplyModifiedProperties();
				});

				menu.AddSeparator("");

				foreach (KeyValuePair<int, string> pair in valueToNames)
				{
					if (pair.Key == 0) continue;
					if (!IsPowerOfTwo(pair.Key)) continue;
					menu.AddItem(new GUIContent(pair.Value), (pair.Key & originalValue) != 0, () =>
					{
						property.intValue ^= pair.Key;
						property.serializedObject.ApplyModifiedProperties();
					});
				}

				bool hasSeparator = false;

				foreach (KeyValuePair<int, string> pair in valueToNames)
				{
					if (pair.Key == 0) continue;
					if (IsPowerOfTwo(pair.Key)) continue;
					if (!hasSeparator)
					{
						menu.AddSeparator("");
						hasSeparator = true;
					}

					menu.AddItem(new GUIContent(pair.Value), (pair.Key & originalValue) == pair.Key, () =>
					{
						if(pair.Key == 0 || (pair.Key & property.intValue) == pair.Key)
							property.intValue ^= pair.Key;
						else
							property.intValue |= pair.Key;
						property.serializedObject.ApplyModifiedProperties();
					});
				}

				menu.DropDown(rect);
			}

			private static bool IsPowerOfTwo(int x) => x != 0 && (x & (x - 1)) == 0;
		}

		private static void EnumFlagsFieldDistinct(Rect position, GUIContent label, SerializedProperty maskProperty, Type enumType)
		{
			if (!lookup.TryGetValue(enumType, out ValueAndNames valueAndNames))
				lookup.Add(enumType, valueAndNames = new ValueAndNames(enumType));

			position = EditorGUI.PrefixLabel(position, label);
			if (GUI.Button(position, valueAndNames.GetName(maskProperty.intValue), EditorStyles.layerMaskField))
				valueAndNames.DropDown(position, maskProperty);
		}
	}
}