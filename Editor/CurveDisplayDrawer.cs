using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Vertx.Attributes.Editor
{
	[CustomPropertyDrawer(typeof(CurveDisplayAttribute))]
	public sealed class CurveDisplayDrawer : PropertyDrawer
	{
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var curveDisplay = (CurveDisplayAttribute)attribute;
			CurveField curveField;
			// ReSharper disable once ConvertSwitchStatementToSwitchExpression
			switch (curveDisplay.Display)
			{
				case CurveDisplayAttribute.CurveDisplay.RectAndColor:
					curveField = new CurveField(property.displayName)
					{
						bindingPath = property.propertyPath,
						ranges = curveDisplay.Rect
					};
					break;
				case CurveDisplayAttribute.CurveDisplay.ColorOnly:
					curveField = new CurveField(property.displayName)
					{
						bindingPath = property.propertyPath
					};
					break;
				default:
					throw new NotImplementedException($"{curveDisplay.Display} is improperly initialised.");
			}

			curveField.AddToClassList(StyleSheetUtils.AlignedFieldUssClassName);
			if (curveDisplay.Height > 0)
				curveField.style.height = curveDisplay.Height;

			AssignColor(curveField, curveDisplay);
			curveField.RegisterCallback<CustomStyleResolvedEvent, CurveDisplayAttribute>((evt, args) => AssignColor((CurveField)evt.target, args), curveDisplay);

			return curveField;
		}

		private static void AssignColor(CurveField curveField, CurveDisplayAttribute curveDisplay) =>
			// ReSharper disable once PossibleNullReferenceException
			typeof(CurveField).GetField("m_CurveColor", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(curveField, curveDisplay.Color);
	}
}