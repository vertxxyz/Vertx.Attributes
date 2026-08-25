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
			var textField = new TextField
			{
				bindingPath = property.propertyPath,
				isDelayed = true
			};
			root.Add(textField);

			var helpBox = new HelpBox("Directory is invalid or has not been not set", HelpBoxMessageType.Error)
			{
				style = { display = DisplayStyle.None }
			};
			root.Add(helpBox);
			// ReSharper disable once HeapView.CanAvoidClosure
			helpBox.RegisterCallback<ClickEvent>(_ => SelectDirectory());

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
				var dA = (DirectoryAttribute)attribute;
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

			void UpdateHelpBoxDisplay() => helpBox.style.display = DirectoryExists(property) ? DisplayStyle.None : DisplayStyle.Flex;
		}

		private static bool DirectoryExists(SerializedProperty property) => !string.IsNullOrEmpty(property.stringValue) && Directory.Exists(property.stringValue);
	}
}