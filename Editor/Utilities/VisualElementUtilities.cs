using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Vertx.Attributes.Editor
{
#if UNITY_6000_7_OR_NEWER
	[Unity.Scripting.LifecycleManagement.NoAutoStaticsCleanup]
#endif
	internal static class VisualElementUtilities
	{
		public static bool TryFindInParent<TParent>(
			VisualElement element,
			[NotNullWhen(true)] out TParent? result
		) where TParent : VisualElement
		{
			VisualElement parent = element.parent;
			// ReSharper disable once UseNegatedPatternInIsExpression
			while (!(parent is TParent))
			{
				if (parent == null)
				{
					result = null;
					return false;
				}

				parent = parent.parent;
			}

			result = (TParent)parent;
			return true;
		}

		private static readonly Func<PropertyField, SerializedProperty> s_getSerializedPropertyFunc =
			(Func<PropertyField, SerializedProperty>)Delegate.CreateDelegate(
				typeof(Func<PropertyField, SerializedProperty>),
				typeof(PropertyField).GetProperty("serializedProperty", BindingFlags.NonPublic | BindingFlags.Instance)!.GetMethod
			);

		// ReSharper disable once SuggestBaseTypeForParameter
		public static SerializedProperty? GetSerializedProperty(PropertyField propertyField)
		{
			SerializedProperty property = s_getSerializedPropertyFunc(propertyField);
			if (property == null)
				return null;
			if (property.propertyPath == "")
				property = property.serializedObject.FindProperty(propertyField.bindingPath);
			return property;
		}
	}
}