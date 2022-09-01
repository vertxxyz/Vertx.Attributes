using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;

namespace Vertx.Attributes.Editor
{
	public class BetterEnumFlagsField : EnumFlagsField
	{
		private EnumFlagsDrawer.ValueAndNames _valueAndNames;

		private delegate FieldInfo GetTypeFromPropertyBase(SerializedProperty property, out Type type);

		private static GetTypeFromPropertyBase s_GetTypeFromProperty;

		private Type GetTypeFromProperty(SerializedProperty property)
		{
			if (s_GetTypeFromProperty == null)
			{
				MethodInfo method = Type.GetType("UnityEditor.ScriptAttributeUtility,UnityEditor").GetMethod("GetFieldInfoFromProperty", BindingFlags.Static | BindingFlags.NonPublic);
				s_GetTypeFromProperty = (GetTypeFromPropertyBase)Delegate.CreateDelegate(typeof(GetTypeFromPropertyBase), method);
			}

			s_GetTypeFromProperty(property, out Type type);
			return type;
		}
		
		public BetterEnumFlagsField(SerializedProperty property, bool hideObsoleteNames) : base(property.displayName)
		{
			bindingPath = property.propertyPath;
			_valueAndNames = new EnumFlagsDrawer.ValueAndNames(GetTypeFromProperty(property), hideObsoleteNames);
			// TODO actually add all these values.
			/*choices = new List<string>(_valueAndNames.displayNames);
			choicesMasks = new List<int>(m_EnumData.flagValues);*/
		}
	}
}