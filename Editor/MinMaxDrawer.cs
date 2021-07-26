using System;
using UnityEditor;
using UnityEngine;
#if UNITY_MATHEMATICS
using Unity.Mathematics;
#endif

namespace Vertx.Attributes.Editor
{
	[CustomPropertyDrawer(typeof(MinMaxAttribute))]
	public class MinMaxDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			MinMaxAttribute a = (MinMaxAttribute)attribute;
			DoSlider(a.Label ?? new GUIContent(property.displayName), position, property, a.Min, a.Max);
		}

		private static void DoSlider(GUIContent label, Rect position, SerializedProperty property, float minValue, float maxValue)
		{
			using (var prop = new EditorGUI.PropertyScope(position, label, property))
			{
				label = prop.content;
				bool isFloat = false;
				bool isVector = false;
#if UNITY_MATHEMATICS
				bool isMathematics = false;
#endif
				switch (property.propertyType)
				{
					case SerializedPropertyType.Integer:
						break;
					case SerializedPropertyType.Float:
						isFloat = true;
						break;
					case SerializedPropertyType.Vector2:
						isFloat = true;
						isVector = true;
						break;
					case SerializedPropertyType.Vector2Int:
						isVector = true;
						break;
#if UNITY_MATHEMATICS
					case SerializedPropertyType.Generic:
						switch (property.type)
						{
							case nameof(int2):
								isMathematics = true;
								break;
							case nameof(float2):
								isFloat = true;
								isMathematics = true;
								break;
							default:
								Debug.LogWarning(
									$"{nameof(MinMaxAttribute)} only supports float, int, {nameof(Vector2)}, and {nameof(Vector2Int)}. Property type was {property.propertyType}");
								return;
						}

						break;
#endif
					default:
						Debug.LogWarning(
							$"{nameof(MinMaxAttribute)} only supports float, int, {nameof(Vector2)}, and {nameof(Vector2Int)}. Property type was {property.propertyType}");
						return;
				}

#if UNITY_MATHEMATICS
				if (isMathematics)
				{
					SerializedProperty x = property.FindPropertyRelative("x");
					SerializedProperty y = property.FindPropertyRelative("y");
					Vector2 minMax = isFloat ? new Vector2(x.floatValue, y.floatValue) : new Vector2(x.intValue, y.intValue);
					float min = minMax.x;
					float max = minMax.y;
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
#endif
				if (isVector)
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