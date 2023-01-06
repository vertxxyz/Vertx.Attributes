using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.UIElements;

namespace Vertx.Attributes.Editor
{
	[CustomPropertyDrawer(typeof(FileAttribute))]
	public sealed class FileAttributeDrawer : PropertyDrawer
	{
		private enum LocationValidity
		{
			NoLocation,
			ValidLocation,
			InvalidLocation
		}

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var root = new VisualElement();
			TextField textField = new TextField
			{
				bindingPath = property.propertyPath,
				isDelayed = true
			};
			root.Add(textField);
#if UNITY_2020_1_OR_NEWER
			// Sorry, UIToolkit only supports HelpBox in 2020.
			HelpBox helpBox;
			root.Add(helpBox = new HelpBox("File is invalid or has not been not set", HelpBoxMessageType.Error)
			{
				style = { display = DisplayStyle.None }
			});
			// ReSharper disable once HeapView.CanAvoidClosure
			helpBox.RegisterCallback<ClickEvent>(_ => SelectFolder());
#endif
			textField.RegisterValueChangedCallback(evt =>
			{
				if (SetNewFileLocation(evt.newValue, (FileAttribute)attribute) != LocationValidity.InvalidLocation)
				{
					UpdateHelpBoxDisplay();
					return;
				}

				textField.SetValueWithoutNotify(evt.previousValue);
				property.stringValue = evt.previousValue;
				property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
				UpdateHelpBoxDisplay();
			});
			
			root.Add(new Button(SelectFolder)
			{
				text = $"Set {property.displayName}"
			});
			return root;

			void SelectFolder()
			{
				FileAttribute fA = (FileAttribute)attribute;
				string path = fA.FileIsLocalToProject ? "Assets" : Application.dataPath;
				string newFile = EditorUtility.OpenFilePanel("Choose File", path, null);
				if (SetNewFileLocation(newFile, fA) != LocationValidity.ValidLocation)
				{
					UpdateHelpBoxDisplay();
					return;
				}

				textField.SetValueWithoutNotify(property.stringValue);
				property.serializedObject.ApplyModifiedProperties();
				UpdateHelpBoxDisplay();
			}

			LocationValidity SetNewFileLocation(string newFile, FileAttribute fA)
			{
				if (string.IsNullOrEmpty(newFile))
					return LocationValidity.NoLocation;

				if (!fA.FileIsLocalToProject)
					property.stringValue = newFile;
				else
				{
					if (newFile.StartsWith(Application.dataPath))
						property.stringValue = $"Assets{newFile.Substring(Application.dataPath.Length)}";
					else if (!newFile.StartsWith("Assets/"))
					{
						Debug.LogWarning("File must be local to project, eg. Assets...");
						return LocationValidity.InvalidLocation;
					}
				}

				return LocationValidity.ValidLocation;
			}

			void UpdateHelpBoxDisplay()
			{
#if UNITY_2020_1_OR_NEWER
				helpBox.style.display = FileExists(property) ? DisplayStyle.None : DisplayStyle.Flex;
#endif
			}
		}

		private static bool FileExists(SerializedProperty property) => !string.IsNullOrEmpty(property.stringValue) && File.Exists(property.stringValue);

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			FileAttribute fA = (FileAttribute)attribute;
			Rect r = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
			EditorGUI.PropertyField(r, property, GUIContent.none);
			r.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

			if (!FileExists(property))
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

			FileAttribute fA = (FileAttribute)attribute;
			if (fA.FileIsLocalToProject && property.stringValue.StartsWith(Application.dataPath))
				return EditorGUIUtility.singleLineHeight * 3 + EditorGUIUtility.standardVerticalSpacing;

			return EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing;
		}
	}
}