using UnityEngine;
using UnityEditor;
using System.IO;
using Vertx.Utilities.Editor;

namespace Vertx.Attributes.Editor
{
	[CustomPropertyDrawer(typeof(DirectoryAttribute))]
	public class DirectoryAttributeDrawer : PropertyDrawer
	{
		/// <summary>
		/// Provide a text field that operates like a button which opens a folder dialogue
		/// A help box is shown when the resulting string is invalid
		/// </summary>
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			DirectoryAttribute dA = (DirectoryAttribute) attribute;
			Rect r = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
			EditorGUI.PropertyField(r, property, GUIContent.none);
			r.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

			if (string.IsNullOrEmpty(property.stringValue) || !Directory.Exists(property.stringValue))
			{
				string path = dA.DirectoryIsLocalToProject ? "Assets" : Application.dataPath;

				DirectoryButton(r, property, dA, path);
				r.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
				r.height *= 2;
				EditorGUI.HelpBox(r, "Directory is invalid or empty", MessageType.Error);
				DirectoryButton(r, property, dA, property.stringValue, true);
			}
			else
			{
				DirectoryButton(r, property, dA, property.stringValue);
			}
		}

		private static void DirectoryButton(Rect position, SerializedProperty sP, DirectoryAttribute dA, string path, bool overPrevious = false)
		{
			if (!overPrevious)
			{
				if (!GUI.Button(position, $"Set {sP.displayName}"))
					return;
			}
			else
			{
				if (!GUI.Button(position, GUIContent.none, GUIStyle.none))
					return;
			}

			string newDirectory = EditorUtility.OpenFolderPanel("Choose Directory", path, path.Equals("Assets") ? string.Empty : path);
			if (string.IsNullOrEmpty(newDirectory))
			{
				GUIUtility.ExitGUI();
				return;
			}

			if (!dA.DirectoryIsLocalToProject)
				sP.stringValue = newDirectory;
			else if (newDirectory.StartsWith(Application.dataPath))
				sP.stringValue = $"Assets{newDirectory.Substring(Application.dataPath.Length)}";
			else
				Debug.LogWarning("Directory must be local to project, eg. Assets...");

			sP.serializedObject.ApplyModifiedProperties();
			GUIUtility.ExitGUI();
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			if (string.IsNullOrEmpty(property.stringValue) || !Directory.Exists(property.stringValue))
				return EditorGUIUtility.singleLineHeight * 4 + EditorGUIUtility.standardVerticalSpacing;
			
			DirectoryAttribute dA = (DirectoryAttribute) attribute;
			if (dA.DirectoryIsLocalToProject && property.stringValue.StartsWith(Application.dataPath))
				return EditorGUIUtility.singleLineHeight * 3 + EditorGUIUtility.standardVerticalSpacing;
			
			return EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing;
		}
	}
}