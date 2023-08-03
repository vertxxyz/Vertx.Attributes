using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Vertx.Attributes.Editor
{
	internal static class VisualElementUtilities
	{
		public static bool TryFindInParent<TParent>(VisualElement element, out TParent result) where TParent : VisualElement
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
		
		// ReSharper disable once SuggestBaseTypeForParameter
		public static SerializedProperty GetSerializedProperty(PropertyField propertyField)
		{
			var property = (SerializedProperty)typeof(PropertyField).GetProperty("serializedProperty", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(propertyField);
			if (property == null)
				return null;
			if (property.propertyPath == "")
				property = property.serializedObject.FindProperty(propertyField.bindingPath);
			return property;
		}
	}
}