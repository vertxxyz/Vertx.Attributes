using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Vertx.Attributes.Editor
{
#if UNITY_6000_7_OR_NEWER
	[Unity.Scripting.LifecycleManagement.NoAutoStaticsCleanup]
#endif
	internal static class StyleSheetUtils
	{
		public static readonly string AlignedFieldUssClassName =
#if UNITY_2022_2_OR_NEWER
			BaseField<int>.alignedFieldUssClassName;
#else
			BaseField<int>.ussClassName + "__aligned";
#endif

		private static readonly Dictionary<string, StyleSheet> s_sheetLookup = new Dictionary<string, StyleSheet>();

		/// <summary>
		/// Adds a stylesheet to the root of the inspector if it's not already present.
		/// </summary>
		public static void AddStyleSheetOnPanelEvent(AttachToPanelEvent evt, string stylePath)
		{
			VisualElement root = evt.destinationPanel.visualTree;
			root = root.Children().SingleOrDefault(c => c.name.StartsWith("rootVisualContainer", StringComparison.Ordinal)) ?? root;
			if (!s_sheetLookup.TryGetValue(stylePath, out StyleSheet sheet))
			{
				s_sheetLookup.Add(stylePath, sheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(stylePath));
				if (sheet == null)
				{
					Debug.LogWarning($"\"{stylePath}\" was not found.");
					return;
				}
			}
			else if (sheet == null)
			{
				sheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(stylePath);
				if (sheet == null)
				{
					return;
				}

				s_sheetLookup[stylePath] = sheet;
			}

			if (!root.styleSheets.Contains(sheet))
				root.styleSheets.Add(sheet);
		}
	}
}