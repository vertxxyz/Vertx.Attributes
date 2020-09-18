using UnityEngine;
using UnityEditor;
using System.IO;
using Vertx.Utilities.Editor;

namespace Vertx.Attributes.Editor
{
	[CustomPropertyDrawer(typeof(FileAttribute))]
	public class FileAttributeDrawer : PropertyDrawer
	{
		/// <summary>
		/// Provide a text field that operates like a button which opens a file dialogue
		/// A help box is shown when the resulting string is invalid
		/// </summary>
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			FileAttribute fA = (FileAttribute) attribute;
			Rect r = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
			EditorGUI.PropertyField(r, property, GUIContent.none);
			r.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

			if (string.IsNullOrEmpty(property.stringValue) || !File.Exists(property.stringValue))
			{
				string path = fA.FileIsLocalToProject ? "Assets" : Application.dataPath;

				FileButton(r, property, fA, path);
				r.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
				r.height *= 2;
				EditorGUI.HelpBox(r, "File is invalid or has not been not set", MessageType.Error);
				FileButton(r, property, fA, property.stringValue, true);
			}
			else
			{
				FileButton(r, property, fA, property.stringValue);
			}
		}

		private static void FileButton(Rect position, SerializedProperty sP, FileAttribute fA, string path, bool overPrevious = false)
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

			string newFile = EditorUtility.OpenFilePanel("Choose File", path, null);
			if (string.IsNullOrEmpty(newFile))
			{
				GUIUtility.ExitGUI();
				return;
			}

			if (!fA.FileIsLocalToProject)
				sP.stringValue = newFile;
			else if (newFile.StartsWith(Application.dataPath))
				sP.stringValue = $"Assets{newFile.Substring(Application.dataPath.Length)}";
			else
				Debug.LogWarning("File must be local to project, eg. Assets...");

			sP.serializedObject.ApplyModifiedProperties();
			GUIUtility.ExitGUI();
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			if (string.IsNullOrEmpty(property.stringValue) || !File.Exists(property.stringValue))
				return EditorGUIUtility.singleLineHeight * 4 + EditorGUIUtility.standardVerticalSpacing;
			
			FileAttribute fA = (FileAttribute) attribute;
			if (fA.FileIsLocalToProject && property.stringValue.StartsWith(Application.dataPath))
				return EditorGUIUtility.singleLineHeight * 3 + EditorGUIUtility.standardVerticalSpacing;
			
			return EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing;
		}
	}
}