using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;
// ReSharper disable ArrangeObjectCreationWhenTypeEvident
// ReSharper disable ConvertIfStatementToNullCoalescingAssignment

namespace Vertx.Attributes.Editor
{
#if UNITY_6000_7_OR_NEWER
	[Unity.Scripting.LifecycleManagement.NoAutoStaticsCleanup]
#endif
	internal sealed class EnumFlagsValueAndNames
	{
		private static readonly Dictionary<Type, EnumFlagsValueAndNames?> s_lookup = new Dictionary<Type, EnumFlagsValueAndNames?>();
		private readonly string _noneName = "Nothing";
		private readonly Dictionary<int, string> _valueToNames = new Dictionary<int, string>();
		private readonly Dictionary<int, string> _complexNameLookup = new Dictionary<int, string>();
		private readonly int _everythingValue;

		private enum SupportedTypes : byte
		{
			Byte,
			Short,
			Int
		}

		public static EnumFlagsValueAndNames? Get(FieldInfo fieldInfo, bool hideObsoleteNames)
		{
			Type enumType = fieldInfo.FieldType;
			if (!enumType.IsEnum)
			{
				if (enumType.IsGenericType)
					enumType = enumType.GetGenericArguments().Single();
				if (enumType.IsArray)
					enumType = enumType.GetElementType()!;
			}

			if (s_lookup.TryGetValue(enumType, out EnumFlagsValueAndNames? valueAndNames))
				return valueAndNames;

			try
			{
				s_lookup.Add(enumType, valueAndNames = new EnumFlagsValueAndNames(enumType, hideObsoleteNames));
			}
			catch (Exception e)
			{
				Debug.LogException(e);
				s_lookup.Add(enumType, null);
				valueAndNames = null;
			}

			return valueAndNames;
		}

		private EnumFlagsValueAndNames(Type enumType, bool hideObsoleteNames)
		{
			Dictionary<string, FieldInfo> fieldLookup = enumType
				.GetFields(BindingFlags.Public | BindingFlags.Static)
				.ToDictionary(k => k.Name, v => v);

			string[] names = Enum.GetNames(enumType);
			Array values = Enum.GetValues(enumType);
			Type underlyingType = Enum.GetUnderlyingType(enumType);
			SupportedTypes type;
			if (underlyingType == typeof(int))
				type = SupportedTypes.Int;
			else if (underlyingType == typeof(short))
				type = SupportedTypes.Short;
			else if (underlyingType == typeof(byte))
				type = SupportedTypes.Byte;
			else
				type = SupportedTypes.Int; // Default and let any exceptions occur
			_everythingValue = 0;
			for (var i = 0; i < values.Length; i++)
			{
				int value = type switch
				{
					SupportedTypes.Byte => (byte)values.GetValue(i),
					SupportedTypes.Short => (short)values.GetValue(i),
					SupportedTypes.Int => (int)values.GetValue(i),
					_ => throw new ArgumentOutOfRangeException()
				};

				string valueName = names[i];
				string? nicifiedName = null;
				if (hideObsoleteNames && fieldLookup.TryGetValue(valueName, out var fieldInfo))
				{
					var obsoleteAttribute = fieldInfo.GetCustomAttribute<ObsoleteAttribute>();
					if (obsoleteAttribute != null)
						continue;

					var inspectorNameAttribute = fieldInfo.GetCustomAttribute<InspectorNameAttribute>();
					if (inspectorNameAttribute != null)
						nicifiedName = inspectorNameAttribute.displayName;
				}

				if (nicifiedName == null)
					nicifiedName = ObjectNames.NicifyVariableName(valueName);

				_everythingValue |= value;

				if (value == 0)
				{
					_noneName = nicifiedName;
					continue;
				}

				if (_valueToNames.TryGetValue(value, out string name))
				{
					_valueToNames[value] = $"{name} | {nicifiedName}";
					continue;
				}

				_valueToNames.Add(value, nicifiedName);
			}

			_valueToNames.Add(0, _noneName);
		}

		public string GetName(int value)
		{
			if (value == _everythingValue) return "Everything";
			if (_valueToNames.TryGetValue(value, out var result))
				return result;

			if (_complexNameLookup.TryGetValue(value, out result))
				return result;

			StringBuilder stringBuilder = new StringBuilder();
			bool hitMax = false;
			foreach (KeyValuePair<int, string> pair in _valueToNames)
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
				var secondaryBuilder = new StringBuilder("Not ");
				foreach (KeyValuePair<int, string> pair in _valueToNames)
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
				if (!hitMax) // Remove the last ", "
					stringBuilder.Remove(stringBuilder.Length - 2, 2);
				_complexNameLookup.Add(value, stringBuilder.ToString());
			}
			else
			{
				_complexNameLookup.Add(value, "⚠️ Invalid ⚠️");
			}

			return _complexNameLookup[value];
		}

		public void DropDown(Rect rect, SerializedProperty property)
		{
			GenericMenu menu = new GenericMenu();

			int originalValue = property.intValue;

			menu.AddItem(new GUIContent(_noneName), originalValue == 0, () =>
			{
				property.intValue = 0;
				property.serializedObject.ApplyModifiedProperties();
			});
			menu.AddItem(new GUIContent("Everything"), originalValue == _everythingValue, () =>
			{
				property.intValue = _everythingValue;
				property.serializedObject.ApplyModifiedProperties();
			});

			menu.AddSeparator("");

			foreach (KeyValuePair<int, string> pair in _valueToNames)
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

			foreach (KeyValuePair<int, string> pair in _valueToNames)
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
					if (pair.Key == 0 || (pair.Key & property.intValue) == pair.Key)
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
}