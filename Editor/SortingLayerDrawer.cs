using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Vertx.Attributes.Editor
{
	[CustomPropertyDrawer(typeof(SortingLayerAttribute))]
	public sealed class SortingLayerDrawer : PropertyDrawer
	{
		private static class Styles
		{
			private static GUIStyle? s_soldPopupStyle;

			public static readonly GUIContent SortingLayerStyle = EditorGUIUtility.TrTextContent("Sorting Layer", "Name of the Renderer's sorting layer");

			public static GUIStyle BoldPopupStyle =>
				s_soldPopupStyle ??= new GUIStyle(EditorStyles.popup)
				{
					fontStyle = FontStyle.Bold
				};
		}

		private static readonly Action<Rect, GUIContent, SerializedProperty, GUIStyle, GUIStyle> s_sortingLayerField
			= (Action<Rect, GUIContent, SerializedProperty, GUIStyle, GUIStyle>)
			Delegate.CreateDelegate(
				typeof(Action<Rect, GUIContent, SerializedProperty, GUIStyle, GUIStyle>),
				typeof(EditorGUI).GetMethod(
					"SortingLayerField",
					BindingFlags.NonPublic | BindingFlags.Static,
					null,
					new[] { typeof(Rect), typeof(GUIContent), typeof(SerializedProperty), typeof(GUIStyle), typeof(GUIStyle) },
					null
				)!
			);

		private static readonly Func<SerializedProperty, bool> s_hasPrefabOverride
			= (Func<SerializedProperty, bool>)
			Delegate.CreateDelegate(
				typeof(Func<SerializedProperty, bool>),
				Type.GetType("UnityEditor.SortingLayerEditorUtility,UnityEditor")!.GetMethod(
					"HasPrefabOverride",
					BindingFlags.NonPublic | BindingFlags.Static
				)!
			);

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (property.propertyType == SerializedPropertyType.Integer)
			{
				bool hasPrefabOverride = s_hasPrefabOverride(property);
				s_sortingLayerField(position, Styles.SortingLayerStyle, property, hasPrefabOverride ? Styles.BoldPopupStyle : EditorStyles.popup, hasPrefabOverride ? EditorStyles.boldLabel : EditorStyles.label);
			}
			else
			{
				EditorGUI.LabelField(position, label.text, $"{nameof(SortingLayerAttribute)} shouldn't be applied to {property.propertyType}, it's only valid on ints.");
			}
		}

#if UNITY_2021_1_OR_NEWER
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			List<string> names = SortingLayer.layers.Select(x => x.name).ToList();

			// Switch to a SortingLayerField if Unity ever implements one.
			var layerField = new DropdownField(names, property.intValue) { label = property.displayName };
			layerField.TrackPropertyValue(property, serializedProperty =>
			{
				if (serializedProperty.intValue < 0 || serializedProperty.intValue >= names.Count)
					layerField.SetValueWithoutNotify($"Invalid layer! ({serializedProperty.intValue})");
				else
					layerField.value = names[serializedProperty.intValue];
			});
			if (property.intValue < 0 || property.intValue >= names.Count)
				layerField.SetValueWithoutNotify($"Invalid layer! ({property.intValue})");
			layerField.RegisterValueChangedCallback(evt =>
			{
				property.intValue = names.IndexOf(evt.newValue);
				property.serializedObject.ApplyModifiedProperties();
			});
			layerField.AddToClassList(StyleSheetUtils.AlignedFieldUssClassName);
			return layerField;
		}
#endif
	}
}