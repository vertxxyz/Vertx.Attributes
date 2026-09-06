using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Vertx.Attributes.Editor
{
#if UNITY_6000_7_OR_NEWER
	[Unity.Scripting.LifecycleManagement.NoAutoStaticsCleanup]
#endif
	[CustomPropertyDrawer(typeof(PlayOnlyFieldAttribute))]
	public sealed class PlayOnlyDrawer : DecoratorDrawer
	{
		private static readonly HashSet<DecoratePropertyElement> s_playOnlyDecorators = new();

		[InitializeOnLoadMethod]
		private static void Setup()
		{
			EditorApplication.playModeStateChanged += change =>
			{
				try
				{
					if (change != PlayModeStateChange.EnteredEditMode && change != PlayModeStateChange.EnteredPlayMode)
						return;
					
					s_playOnlyDecorators.RemoveWhere(e => !e.HasTarget);

					foreach (DecoratePropertyElement element in s_playOnlyDecorators)
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
				(property, element) => element.SetEnabled(Application.IsPlaying(property.serializedObject.targetObject))
			) { name = nameof(PlayOnlyDrawer) };
			s_playOnlyDecorators.Add(drawer);

			// Remove the drawer from our set if it's detached from the panel.
			drawer.RegisterCallback<DetachFromPanelEvent, (HashSet<DecoratePropertyElement> set, DecoratePropertyElement element)>(
				static (_, args) => args.set.Remove(args.element),
				(s_playOnlyDecorators, drawer)
			);

			return drawer;
		}
	}
}