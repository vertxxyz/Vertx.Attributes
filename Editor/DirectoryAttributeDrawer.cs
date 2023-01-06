using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.UIElements;

namespace Vertx.Attributes.Editor
{
	[CustomPropertyDrawer(typeof(DirectoryAttribute))]
	public sealed class DirectoryAttributeDrawer : PropertyDrawer
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
			root.Add(helpBox = new HelpBox("Directory is invalid or has not been not set", HelpBoxMessageType.Error)
			{
				style = { display = DisplayStyle.None }
			});
			// ReSharper disable once HeapView.CanAvoidClosure
			helpBox.RegisterCallback<ClickEvent>(_ => SelectDirectory());
#endif
			textField.RegisterValueChangedCallback(evt =>
			{
				if (SetNewDirectoryLocation(evt.newValue, (DirectoryAttribute)attribute) != LocationValidity.InvalidLocation)
				{
					UpdateHelpBoxDisplay();
					return;
				}

				textField.SetValueWithoutNotify(evt.previousValue);
				property.stringValue = evt.previousValue;
				property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
				UpdateHelpBoxDisplay();
			});
			
			root.Add(new Button(SelectDirectory)
			{
				text = $"Set {property.displayName}"
			});
			return root;

			void SelectDirectory()
			{
				DirectoryAttribute dA = (DirectoryAttribute)attribute;
				string path = dA.DirectoryIsLocalToProject ? "Assets" : Application.dataPath;
				string newFile = EditorUtility.OpenFolderPanel("Choose Directory", path, path.Equals("Assets") ? string.Empty : path);
				if (SetNewDirectoryLocation(newFile, dA) != LocationValidity.ValidLocation)
				{
					UpdateHelpBoxDisplay();
					return;
				}

				textField.SetValueWithoutNotify(property.stringValue);
				property.serializedObject.ApplyModifiedProperties();
				UpdateHelpBoxDisplay();
			}

			LocationValidity SetNewDirectoryLocation(string newDirectory, DirectoryAttribute dA)
			{
				if (string.IsNullOrEmpty(newDirectory))
					return LocationValidity.NoLocation;

				if (!dA.DirectoryIsLocalToProject)
					property.stringValue = newDirectory;
				else
				{
					if (newDirectory.StartsWith(Application.dataPath))
						property.stringValue = $"Assets{newDirectory.Substring(Application.dataPath.Length)}";
					else if (!newDirectory.StartsWith("Assets/"))
					{
						Debug.LogWarning("Directory must be local to project, eg. Assets...");
						return LocationValidity.InvalidLocation;
					}
				}

				return LocationValidity.ValidLocation;
			}

			void UpdateHelpBoxDisplay()
			{
#if UNITY_2020_1_OR_NEWER
				helpBox.style.display = DirectoryExists(property) ? DisplayStyle.None : DisplayStyle.Flex;
#endif
			}
		}

		private static bool DirectoryExists(SerializedProperty property) => !string.IsNullOrEmpty(property.stringValue) && Directory.Exists(property.stringValue);
		
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			DirectoryAttribute dA = (DirectoryAttribute)attribute;
			Rect r = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
			EditorGUI.PropertyField(r, property, GUIContent.none);
			r.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

			if (!DirectoryExists(property))
			{
				string path = dA.DirectoryIsLocalToProject ? "Assets" : Application.dataPath;

				DirectoryButton(r, property, dA, path);
				r.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
				r.height *= 2;
				EditorGUI.HelpBox(r, "Directory is invalid or has not been not set", MessageType.Error);
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

			DirectoryAttribute dA = (DirectoryAttribute)attribute;
			if (dA.DirectoryIsLocalToProject && property.stringValue.StartsWith(Application.dataPath))
				return EditorGUIUtility.singleLineHeight * 3 + EditorGUIUtility.standardVerticalSpacing;

			return EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing;
		}
	}
}