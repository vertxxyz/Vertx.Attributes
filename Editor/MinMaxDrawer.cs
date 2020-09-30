using UnityEditor;
using UnityEngine;

namespace Vertx.Attributes.Editor
{
	[CustomPropertyDrawer(typeof(MinMaxAttribute))]
	public class MinMaxDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			MinMaxAttribute a = (MinMaxAttribute) attribute;
			DoSlider(a.Label ?? new GUIContent(property.displayName), position, property, a.Min, a.Max);
		}

		private static void DoSlider(GUIContent label, Rect position, SerializedProperty property, float minValue, float maxValue)
		{
			using (new EditorGUI.PropertyScope(position, GUIContent.none, property))
			{
				bool isFloat = false;
				bool isVector = false;
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
					default:
						Debug.LogWarning($"{nameof(MinMaxAttribute)} only supports float, int, {nameof(Vector2)}, and {nameof(Vector2Int)}.");
						return;
				}

				if (isVector)
				{
					Vector2 minMax = isFloat ? property.vector2Value : property.vector2IntValue;
					float min = minMax.x;
					float max = minMax.y;
					DoSlider(label, position, ref min, ref max, minValue, maxValue, isFloat);
					if (isFloat)
						property.vector2Value = new Vector2(min, max);
					else
						property.vector2IntValue = new Vector2Int((int) min, (int) max);
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
						current.intValue = (int) min;
						next.intValue = (int) max;
					}
				}
			}
		}

		private static void DoSlider(GUIContent label, Rect position, ref float min, ref float max, float minValue, float maxValue, bool isFloat)
		{
			float width = EditorGUIUtility.labelWidth;
			position.width -= width;
			Rect labelLeft = new Rect(position.x, position.y, width, position.height);
			EditorGUI.LabelField(labelLeft, label);

			position = new Rect(labelLeft.xMax, position.y, position.width, position.height);
			float floatWidth = position.width * 0.125f;
			float sliderWidth = position.width - floatWidth * 2;
			position.width = floatWidth;
			if (isFloat)
				min = EditorGUI.FloatField(position, min);
			else
				min = EditorGUI.IntField(position, (int) min);
			position.x += floatWidth;
			position.width = sliderWidth;
			EditorGUI.MinMaxSlider(position, ref min, ref max, minValue, maxValue);
			position.x += sliderWidth;
			position.width = floatWidth;
			if (isFloat)
				max = EditorGUI.DelayedFloatField(position, max);
			else
				max = EditorGUI.DelayedIntField(position, (int) max);

			min = Mathf.Min(min, max);
			max = Mathf.Max(min, max);
		}
	}
}