using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
#if UNITY_2021_1_OR_NEWER
using UnityEngine.Pool;
#endif
using UnityEngine.UIElements;

namespace Vertx.Attributes.Editor
{
	[CustomPropertyDrawer(typeof(InlineAttribute))]
	public class InlineDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (!property.hasChildren)
			{
				EditorGUI.PropertyField(position, property);
				return;
			}

			SerializedProperty end = property.GetEndProperty();
			bool enter = true;
			while (property.NextVisible(enter) && !SerializedProperty.EqualContents(end, property))
			{
				enter = false;
				position.height = EditorGUI.GetPropertyHeight(property);
				EditorGUI.PropertyField(position, property, true);
				position.y = position.yMax + EditorGUIUtility.standardVerticalSpacing;
			}
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			if (!property.hasChildren)
				return EditorGUI.GetPropertyHeight(property, label);

			property.isExpanded = true;
			return EditorGUI.GetPropertyHeight(property, label) - EditorGUIUtility.singleLineHeight;
		}

#if !UNITY_2021_1_OR_NEWER
		// ReSharper disable once ArrangeObjectCreationWhenTypeEvident
		private static readonly List<VisualElement> elements = new List<VisualElement>();
#endif
		
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			property.isExpanded = true;
			PropertyField field = new PropertyField(property);
			if (property.hasChildren)
				field.RegisterCallback<GeometryChangedEvent>(RemoveFoldout);
			return field;

			void RemoveFoldout(GeometryChangedEvent evt)
			{
				var foldout = field.Q<Foldout>();
				if (foldout == null)
					return;
				VisualElement contentContainer = foldout.contentContainer;
				if (contentContainer.childCount == 0)
					return;

				VisualElement root = new VisualElement();
				
#if UNITY_2021_1_OR_NEWER
				using (ListPool<VisualElement>.Get(out List<VisualElement> elements))
#endif
				{
					for (int i = 0; i < contentContainer.childCount; i++)
					{
						VisualElement element = contentContainer[i];
						elements.Add(element);
					}
					
					contentContainer.Clear();
					
					foreach (VisualElement element in elements)
						root.Add(element);
#if !UNITY_2021_1_OR_NEWER
					elements.Clear();
#endif
				}

				foldout.RemoveFromHierarchy();
				field.Add(root);
				field.UnregisterCallback<GeometryChangedEvent>(RemoveFoldout);
			}
		}
	}
}