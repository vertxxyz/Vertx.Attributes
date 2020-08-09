using System;
using UnityEditor;
using UnityEngine;

namespace Vertx.Attributes.Editor
{
	[CustomPropertyDrawer(typeof(CurveDisplayAttribute))]
	public class CurveDisplayDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var curveDisplay = (CurveDisplayAttribute) attribute;
			switch (curveDisplay.Display)
			{
				case CurveDisplayAttribute.CurveDisplay.RectAndColor:
					EditorGUI.CurveField(
						position,
						property,
						curveDisplay.Color,
						curveDisplay.Rect,
						label
					);
					break;
				case CurveDisplayAttribute.CurveDisplay.ColorOnly:
					property.animationCurveValue = EditorGUI.CurveField(
						position,
						label,
						property.animationCurveValue,
						curveDisplay.Color,
						new Rect()
					);
					break;
				default:
					throw new NotImplementedException($"{curveDisplay.Display} is improperly initialised.");
			}
		}
	}
}