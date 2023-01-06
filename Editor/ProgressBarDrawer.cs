using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Vertx.Attributes.Editor
{
	[CustomPropertyDrawer(typeof(ProgressAttribute))]
	public sealed class ProgressBarDrawer : PropertyDrawer
	{
#if UNITY_2021_1_OR_NEWER
		public const string UssClassName = "vertx-progress-bar";
		public const string RootUssClassName = UssClassName + "__root";
		
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var progressAttribute = (ProgressAttribute)attribute;
			var root = new VisualElement { pickingMode = PickingMode.Ignore };
			root.AddToClassList(BaseField<int>.ussClassName);
			root.AddToClassList(UssClassName);
			
			var label = new Label(property.displayName);
			label.AddToClassList(BaseField<int>.labelUssClassName);
			root.Add(label);
			
			var bar = new ProgressBar
			{
				bindingPath = property.propertyPath,
				lowValue = progressAttribute.MinValue,
				highValue = progressAttribute.MaxValue
			};
			bar.AddToClassList(RootUssClassName);
			bar.RegisterCallback<AttachToPanelEvent, string>(StyleSheetUtils.AddStyleSheetOnPanelEvent, SheetPaths.AttributeStyles);
			string text = GetText(property.floatValue, progressAttribute, out _);
			Label innerLabel;
			bar.Add(innerLabel = new Label
			{
				style =
				{
					position = Position.Absolute,
					top = 0, right = 0, bottom = 0, left = 0,
					unityTextAlign = TextAnchor.MiddleCenter
				},
				text = text
			});
			bar.RegisterCallback<ChangeEvent<float>, (Label label, ProgressAttribute attribute)>(
				(evt, args) => args.label.text = GetText(evt.newValue, args.attribute, out _),
				(innerLabel, progressAttribute)
			);
			root.Add(bar);
			return root;
		}
#endif

		private static string GetText(float value, ProgressAttribute attribute, out float normalised)
		{
			normalised = Mathf.InverseLerp(attribute.MinValue, attribute.MaxValue, value);
			return $"{normalised * 100:F2}%";
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			position.height = EditorGUIUtility.singleLineHeight;

			EditorGUI.LabelField(position, label);

			position.y = position.yMax + EditorGUIUtility.standardVerticalSpacing;

			ProgressAttribute progressAttribute = (ProgressAttribute)attribute;
			string text = GetText(property.floatValue, progressAttribute, out float normalised);
			EditorGUI.ProgressBar(position, normalised, text);
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing;
	}
}