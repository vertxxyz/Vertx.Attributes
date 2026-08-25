using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Vertx.Attributes.Editor
{
	[CustomPropertyDrawer(typeof(EditorOnlyFieldAttribute))]
	public sealed class EditorOnlyDrawer : DecoratorDrawer
	{
		private static readonly HashSet<DecoratePropertyElement> s_editorOnlyDecorators = new();

		[InitializeOnLoadMethod]
		private static void Setup()
		{
			EditorApplication.playModeStateChanged += change =>
			{
				try
				{
					if (change != PlayModeStateChange.EnteredEditMode && change != PlayModeStateChange.EnteredPlayMode)
						return;
					
					s_editorOnlyDecorators.RemoveWhere(e => !e.HasTarget);

					foreach (DecoratePropertyElement element in s_editorOnlyDecorators)
						element.ModifyAll();
				}
				catch (Exception e)
				{
					Debug.LogException(e);
				}
			};
		}

		public override VisualElement CreatePropertyGUI()
		{
			var drawer = new DecoratePropertyElement(
				// Only set the element to enabled if we're not playing.
				(property, element) => element.SetEnabled(!Application.IsPlaying(property.serializedObject.targetObject))
			) { name = nameof(EditorOnlyDrawer) };
			s_editorOnlyDecorators.Add(drawer);

			// Remove the drawer from our set if it's detached from the panel.
			drawer.RegisterCallback<DetachFromPanelEvent, (HashSet<DecoratePropertyElement> set, DecoratePropertyElement element)>(
				static (_, args) => args.set.Remove(args.element),
				(s_editorOnlyDecorators, drawer)
			);

			return drawer;
		}
	}
}