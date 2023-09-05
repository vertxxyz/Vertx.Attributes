using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Vertx.Attributes.Editor
{
	[CustomPropertyDrawer(typeof(CustomHeaderAttribute))]
	public sealed class CustomHeaderDecorator : DecoratorDrawer
	{
#if UNITY_2022_1_OR_NEWER
		public const string headerLabelClassName = "unity-header-drawer__label";
		
		public override VisualElement CreatePropertyGUI()
		{
			var customHeader = (CustomHeaderAttribute)attribute;
			var label = new Label(customHeader.LabelText);
			label.AddToClassList(headerLabelClassName);

			IStyle style = label.style;
			if (!float.IsNaN(customHeader.FontSize))
				style.fontSize = customHeader.FontSize;
			if (!float.IsNaN(customHeader.MarginTop))
				style.marginTop = customHeader.MarginTop;
			if (!float.IsNaN(customHeader.MarginRight))
				style.marginRight = customHeader.MarginRight;
			if (!float.IsNaN(customHeader.MarginBottom))
				style.marginBottom = customHeader.MarginBottom;
			if (!float.IsNaN(customHeader.MarginLeft))
				style.marginLeft = customHeader.MarginLeft;

			return label;
		}
#endif

		public override void OnGUI(Rect position) => EditorGUI.HelpBox(position, $"{nameof(CustomHeaderAttribute)} is unsupported when using IMGUI.", MessageType.Warning);
	}
}
