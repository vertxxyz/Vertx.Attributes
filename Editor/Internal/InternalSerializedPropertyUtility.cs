using System;
using System.Reflection;
using UnityEditor;

namespace Vertx.Attributes.Editor.Internal
{
	public static class InternalSerializedPropertyUtility
	{
		public static PropertyDrawer GetPropertyDrawer(SerializedProperty property)
		{
			PropertyHandler propertyHandler = ScriptAttributeUtility.GetHandler(property);
			return propertyHandler.propertyDrawer;
		}

		public static FieldInfo GetFieldInfoFromProperty(SerializedProperty property, out Type type) => ScriptAttributeUtility.GetFieldInfoFromProperty(property, out type);
	}
}