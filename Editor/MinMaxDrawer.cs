using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_MATHEMATICS
using Unity.Mathematics;
#endif

namespace Vertx.Attributes.Editor
{
	[CustomPropertyDrawer(typeof(MinMaxAttribute))]
	public sealed class MinMaxDrawer : PropertyDrawer
	{
#if UNITY_2020_1_OR_NEWER
		public const string UssClassName = "vertx-min-max-slider";
		public const string FieldUssClassName = UssClassName + "__field";
#endif

		private static string GetErrorMessage(SerializedProperty property) => $"{nameof(MinMaxAttribute)} only supports float, int, float2, int2, {nameof(Vector2)}, and {nameof(Vector2Int)}. Property type was {property.propertyType}";

		[Flags]
		private enum PropertyType
		{
			None,
			Int = 1,
			Float = 1 << 1,
			Vector = 1 << 2,
			Mathematics = 1 << 3
		}

		private static PropertyType DeterminePropertyTypes(SerializedProperty property)
		{
			PropertyType type = PropertyType.None;
			switch (property.propertyType)
			{
				case SerializedPropertyType.Integer:
					type |= PropertyType.Int;
					break;
				case SerializedPropertyType.Float:
					type |= PropertyType.Float;
					break;
				case SerializedPropertyType.Vector2:
					type |= PropertyType.Float;
					type |= PropertyType.Vector;
					break;
				case SerializedPropertyType.Vector2Int:
					type |= PropertyType.Int;
					type |= PropertyType.Vector;
					break;
#if UNITY_MATHEMATICS
				case SerializedPropertyType.Generic:
					switch (property.type)
					{
						case nameof(int2):
							type |= PropertyType.Vector;
							type |= PropertyType.Mathematics;
							type |= PropertyType.Int;
							break;
						case nameof(float2):
							type |= PropertyType.Vector;
							type |= PropertyType.Mathematics;
							type |= PropertyType.Float;
							break;
					}

					break;
#endif
				default:
					break;
			}

			return type;
		}

#if UNITY_2020_1_OR_NEWER
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			PropertyType type = DeterminePropertyTypes(property);
			if (type == PropertyType.None)
			{
				return new HelpBox(GetErrorMessage(property), HelpBoxMessageType.Error);
			}

			// Collect properties.
			bool isFloat = (type & PropertyType.Float) != 0;
			SerializedProperty minProp, maxProp;
			if ((type & (PropertyType.Mathematics | PropertyType.Vector)) != 0)
			{
				minProp = property.FindPropertyRelative("x");
				maxProp = property.FindPropertyRelative("y");
			}
			else
			{
				minProp = property;
				maxProp = property.Copy();
				maxProp.Next(false);
			}

			float min = isFloat ? minProp.floatValue : minProp.intValue;
			float max = isFloat ? maxProp.floatValue : maxProp.intValue;

			// Construct main slider
			MinMaxAttribute a = (MinMaxAttribute)attribute;
			MinMaxSlider minMaxSlider = new MinMaxSlider(a.Label == null ? property.displayName : a.Label.text, min, max, a.Min, a.Max);
			minMaxSlider.AddToClassList(UssClassName);
			minMaxSlider.RegisterCallback<AttachToPanelEvent, string>(StyleSheetUtils.AddStyleSheetOnPanelEvent, SheetPaths.AttributeStyles);

			// Add input fields
			if (isFloat)
			{
				FloatField minField, maxField;
				minMaxSlider.Insert(1, minField = new FloatField
				{
					bindingPath = minProp.propertyPath,
					isDelayed = true
				});
				minMaxSlider.Add(maxField = new FloatField
				{
					bindingPath = maxProp.propertyPath,
					isDelayed = true
				});
				minField.AddToClassList(FieldUssClassName);
				maxField.AddToClassList(FieldUssClassName);

				// Propagate serialized property changes back to the unbound MinMaxSlider.
				minField.RegisterCallback<ChangeEvent<float>, MinMaxSlider>(
					(evt, slider) => slider.minValue = evt.newValue,
					minMaxSlider
				);
				maxField.RegisterCallback<ChangeEvent<float>, MinMaxSlider>(
					(evt, slider) => slider.maxValue = evt.newValue,
					minMaxSlider
				);
			}
			else
			{
				IntegerField minField, maxField;
				minMaxSlider.Insert(1, minField = new IntegerField
				{
					bindingPath = minProp.propertyPath,
					isDelayed = true
				});
				minMaxSlider.Add(maxField = new IntegerField
				{
					bindingPath = maxProp.propertyPath,
					isDelayed = true
				});
				minField.AddToClassList(FieldUssClassName);
				maxField.AddToClassList(FieldUssClassName);

				// Propagate serialized property changes back to the unbound MinMaxSlider.
				minField.RegisterCallback<ChangeEvent<int>, MinMaxSlider>(
					(evt, slider) => slider.minValue = evt.newValue,
					minMaxSlider
				);
				maxField.RegisterCallback<ChangeEvent<int>, MinMaxSlider>(
					(evt, slider) => slider.maxValue = evt.newValue,
					minMaxSlider
				);
			}
			
			if (a.Aligned)
				minMaxSlider.AddToClassList(StyleSheetUtils.AlignedFieldUssClassName); // There's truly not enough room with this enabled, so it's optional.
			
			minMaxSlider.RegisterCallback<ChangeEvent<Vector2>, (PropertyType type, SerializedProperty property)>((evt, args) =>
			{
				MinMaxSlider slider = (MinMaxSlider)evt.target;
				bool localIsFloat = (args.type & PropertyType.Float) != 0;
				SerializedProperty localProperty = args.property;

				float xNew, yNew;
				if (localIsFloat)
				{
					xNew = evt.newValue.x;
					yNew = evt.newValue.y;
				}
				else
				{
					xNew = Mathf.Round(evt.newValue.x);
					yNew = Mathf.Round(evt.newValue.y);
					if (xNew != evt.newValue.x || yNew != evt.newValue.y)
					{
						slider.SetValueWithoutNotify(new Vector2(xNew, yNew));
					}
				}

				if ((args.type & PropertyType.Mathematics) != 0)
				{
					SerializedProperty x = localProperty.FindPropertyRelative("x");
					SerializedProperty y = localProperty.FindPropertyRelative("y");
					if (localIsFloat)
					{
						x.floatValue = xNew;
						y.floatValue = yNew;
					}
					else
					{
						x.intValue = (int)xNew;
						y.intValue = (int)yNew;
					}
				}
				else if ((args.type & PropertyType.Vector) != 0)
				{
					if (localIsFloat)
						localProperty.vector2Value = new Vector2(xNew, yNew);
					else
						localProperty.vector2IntValue = new Vector2Int((int)xNew, (int)yNew);
				}
				else
				{
					SerializedProperty current = localProperty;
					SerializedProperty next = localProperty.Copy();
					next.Next(false);

					if (localIsFloat)
					{
						current.floatValue = xNew;
						next.floatValue = yNew;
					}
					else
					{
						current.intValue = (int)xNew;
						next.intValue = (int)yNew;
					}
				}

				localProperty.serializedObject.ApplyModifiedProperties();
			}, (type, property));

			return minMaxSlider;
		}
#endif

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			MinMaxAttribute a = (MinMaxAttribute)attribute;
			DoSlider(a.Label ?? new GUIContent(property.displayName), position, property, a.Min, a.Max);
		}

		private static void DoSlider(GUIContent label, Rect position, SerializedProperty property, float minValue, float maxValue)
		{
			PropertyType type = DeterminePropertyTypes(property);
			if (type == PropertyType.None)
			{
				EditorGUI.HelpBox(position, GetErrorMessage(property), MessageType.Error);
				return;
			}

			bool isFloat = (type & PropertyType.Float) != 0;

			using (var prop = new EditorGUI.PropertyScope(position, label, property))
			{
				label = prop.content;
				if ((type & PropertyType.Mathematics) != 0)
				{
					SerializedProperty x = property.FindPropertyRelative("x");
					SerializedProperty y = property.FindPropertyRelative("y");
					float min = isFloat ? x.floatValue : x.intValue;
					float max = isFloat ? y.floatValue : y.intValue;
					using (var cCS = new EditorGUI.ChangeCheckScope())
					{
						DoSlider(label, position, ref min, ref max, minValue, maxValue, isFloat);
						if (cCS.changed)
						{
							if (isFloat)
							{
								x.floatValue = min;
								y.floatValue = max;
							}
							else
							{
								x.intValue = (int)min;
								y.intValue = (int)max;
							}
						}
					}

					return;
				}

				if ((type & PropertyType.Vector) != 0)
				{
					Vector2 minMax = isFloat ? property.vector2Value : property.vector2IntValue;
					float min = minMax.x;
					float max = minMax.y;
					DoSlider(label, position, ref min, ref max, minValue, maxValue, isFloat);
					if (isFloat)
						property.vector2Value = new Vector2(min, max);
					else
						property.vector2IntValue = new Vector2Int((int)min, (int)max);
				}
				else
				{
					SerializedProperty current = property.Copy();
					property.Next(false);
					SerializedProperty next = property;

					float min = isFloat ? current.floatValue : current.intValue;
					float max = isFloat ? next.floatValue : next.intValue;
					DoSlider(label, position, ref min, ref max, minValue, maxValue, isFloat);

					if (isFloat)
					{
						current.floatValue = min;
						next.floatValue = max;
					}
					else
					{
						current.intValue = (int)min;
						next.intValue = (int)max;
					}
				}
			}
		}

		private static void DoSlider(GUIContent label, Rect position, ref float min, ref float max, float minValue, float maxValue, bool isFloat)
		{
			const int padding = 2;

			position = EditorGUI.PrefixLabel(position, label);
			float floatWidth = Mathf.Max(40, position.width * 0.125f);
			float sliderWidth = position.width - floatWidth * 2 - padding * 2;
			position.width = floatWidth;
			using (new ZeroIndentScope())
			{
				if (isFloat)
					min = EditorGUI.FloatField(position, min);
				else
					min = EditorGUI.IntField(position, (int)min);

				position.x += floatWidth + padding;
				position.width = sliderWidth;
				EditorGUI.MinMaxSlider(position, ref min, ref max, minValue, maxValue);
				position.x += sliderWidth + padding;
				position.width = floatWidth;

				if (isFloat)
					max = EditorGUI.DelayedFloatField(position, max);
				else
					max = EditorGUI.DelayedIntField(position, (int)max);
			}

			min = Mathf.Min(min, max);
			max = Mathf.Max(min, max);
		}

		private class ZeroIndentScope : IDisposable
		{
			private readonly int previousIndent;

			public ZeroIndentScope()
			{
				previousIndent = EditorGUI.indentLevel;
				EditorGUI.indentLevel = 0;
			}

			public void Dispose() => EditorGUI.indentLevel = previousIndent;
		}
	}
}