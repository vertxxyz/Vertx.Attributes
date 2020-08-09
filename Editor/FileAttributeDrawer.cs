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
			FileAttribute dA = (FileAttribute) attribute;
			Rect r = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
			EditorGUI.PropertyField(r, property, GUIContent.none);
			r.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

			if (string.IsNullOrEmpty(property.stringValue) || !File.Exists(property.stringValue))
			{
				string path = dA.FileIsLocalToProject ? "Assets" : Application.dataPath;

				property.stringValue = FileButton(r, property, dA, path);
				r.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
				r.height *= 2;
				EditorGUI.HelpBox(r, "File is invalid or has not been not set", MessageType.Error);
				property.stringValue = FileButton(default, property, dA, property.stringValue, true);
			}
			else
			{
				property.stringValue = FileButton(r, property, dA, property.stringValue);
			}
		}

		private static string FileButton(Rect position, SerializedProperty sP, FileAttribute dA, string path, bool overPrevious = false)
		{
			if (!overPrevious)
			{
				if (!GUI.Button(position, $"Set {sP.displayName}")) return sP.stringValue;
			}
			else
			{
				if (!EditorGUIUtils.ButtonOverPreviousControl()) return sP.stringValue;
			}

			string newFile = EditorUtility.OpenFilePanel("Choose File", path, string.Empty);
			if (string.IsNullOrEmpty(newFile))
				return sP.stringValue;
			if (dA.FileIsLocalToProject)
			{
				if (!newFile.StartsWith(Application.dataPath))
				{
					Debug.LogWarning("File must be local to project, eg. Assets...");
					return sP.stringValue;
				}

				return $"Assets{newFile.Substring(Application.dataPath.Length)}";
			}

			return newFile;
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			if (string.IsNullOrEmpty(property.stringValue) || !File.Exists(property.stringValue))
				return EditorGUIUtility.singleLineHeight * 4 + EditorGUIUtility.standardVerticalSpacing;
			FileAttribute dA = attribute as FileAttribute;
			if (dA.FileIsLocalToProject && property.stringValue.StartsWith(Application.dataPath))
				return EditorGUIUtility.singleLineHeight * 3 + EditorGUIUtility.standardVerticalSpacing;
			return EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing;
		}
	}
}