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
			var flagsAttribute = (EnumFlagsAttribute) attribute;
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

		private static readonly Dictionary<Type, string[]> namesLookup = new Dictionary<Type, string[]>();
		private static readonly Dictionary<Type, int[]> valuesLookup = new Dictionary<Type, int[]>();
		private static readonly Dictionary<Type, Dictionary<int, string>> valuesDictLookup = new Dictionary<Type, Dictionary<int, string>>();
		private static readonly int layerMaskFieldHash = "vertx_EnumFlagsField".GetHashCode();

		private static void EnumFlagsFieldDistinct(Rect position, GUIContent label, SerializedProperty maskProperty, Type enumType)
		{
			int mask = maskProperty.intValue;

			if (!namesLookup.TryGetValue(enumType, out string[] names))
				namesLookup.Add(enumType, names = Enum.GetNames(enumType).Skip(1).Select(ObjectNames.NicifyVariableName).ToArray());

			if (!valuesLookup.TryGetValue(enumType, out int[] values))
				valuesLookup.Add(enumType, values = new int[names.Length]);
			else if (values.Length != names.Length)
				Array.Resize(ref values, names.Length);

			if (!valuesDictLookup.TryGetValue(enumType, out Dictionary<int, string> valuesDict))
				valuesDictLookup.Add(enumType, valuesDict = new Dictionary<int, string>());

			bool MaskContainsIndex(int index)
			{
				int compBit = 1 << index;
				return (mask & compBit) != 0;
			}

			//Set the values used by our mask.
			int count = 0;
			for (int i = 0; i < values.Length; i++)
			{
				if (!MaskContainsIndex(i))
					continue;
				values[i] = 1 << i;
				count++;
			}

			//Collect the string we display in our field.
			if (!valuesDict.TryGetValue(mask, out string str))
			{
				if (count == 0)
				{
					str = "Nothing";
				}
				else if (count == values.Length)
				{
					str = "Everything";
				}
				else
				{
					StringBuilder sB = new StringBuilder();
					for (int i = 0; i < values.Length; i++)
					{
						if (!MaskContainsIndex(i))
							continue;
						if (sB.Length > 0)
							sB.Append(", ");
						sB.Append(names[i]);
					}

					str = sB.ToString();
				}

				valuesDict.Add(mask, str);
			}

			position = EditorGUI.PrefixLabel(position, label);
			if (GUI.Button(position, str, EditorStyles.layerMaskField))
			{
				GenericMenu menu = new GenericMenu();
				menu.AddItem(new GUIContent("Nothing"), count == 0, () =>
				{
					maskProperty.intValue = 0;
					maskProperty.serializedObject.ApplyModifiedProperties();
				});
				menu.AddItem(new GUIContent("Everything"), count == values.Length, () =>
				{
					maskProperty.intValue = ~0;
					maskProperty.serializedObject.ApplyModifiedProperties();
				});
				menu.AddSeparator(string.Empty);
				for (int i = 0; i < values.Length; i++)
				{
					int localI = i;
					//int localIndex = (int)(Mathf.Log(localI) / Mathf.Log(2)) + 1;
					menu.AddItem(new GUIContent(names[i]), MaskContainsIndex(i), () =>
					{
						//If the mask is "everything" then we want to make sure that this flip removes everything up to the max index.
						if (mask == ~0)
						{
							mask = 0;
							//Just toggle *on* the values that are really in the mask range.
							foreach (int value in values)
							{
								//int index = (int)(Mathf.Log(value) / Mathf.Log(2)) + 1;
								mask |= value;
							}
						}
						//If we're one away from having "everything"
						else if (count == values.Length - 1)
						{
							//And the mask is going to flip the bit in question
							if ((values[localI] & mask) == 0)
							{
								//Set the mask to "everything"
								maskProperty.intValue = ~0;
								maskProperty.serializedObject.ApplyModifiedProperties();
								return;
							}
						}

						maskProperty.intValue = mask ^ 1 << localI;
						maskProperty.serializedObject.ApplyModifiedProperties();
					});
				}

				menu.DropDown(position);
			}
		}
	}
}