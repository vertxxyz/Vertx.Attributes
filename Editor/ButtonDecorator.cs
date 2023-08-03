using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Vertx.Attributes.Editor
{
	internal sealed class RepositionDrawerElement<T> : VisualElement
	{
		private readonly T _reference;
		private readonly ButtonAttribute.Location _location;
		private readonly Func<T, SerializedProperty, VisualElement> _createElement;
		private VisualElement _element;

		public RepositionDrawerElement(Func<T, SerializedProperty, VisualElement> createElement, ButtonAttribute.Location displayLocation, T reference)
		{
			_createElement = createElement;
			_reference = reference;
			_location = displayLocation;
			RegisterCallback<AttachToPanelEvent, RepositionDrawerElement<T>>(Attach, this);
			RegisterCallback<DetachFromPanelEvent, RepositionDrawerElement<T>>(Detach, this);
		}

		private static void Attach(AttachToPanelEvent evt, RepositionDrawerElement<T> rde)
		{
			if (!VisualElementUtilities.TryFindInParent(rde, out PropertyField field))
			{
				Debug.LogWarning($"{nameof(PropertyField)} parent could not be located. Please report a bug with Vertx.Attributes.");
				return;
			}

			SerializedProperty property = VisualElementUtilities.GetSerializedProperty(field);

			switch (rde._location)
			{
				case ButtonAttribute.Location.Bottom:
					if (!VisualElementUtilities.TryFindInParent(rde, out InspectorElement inspector))
					{
						Debug.LogWarning($"{nameof(ButtonAttribute)} with this location is not currently supported, as a {nameof(InspectorElement)} parent could not be found.");
						return;
					}

					inspector[inspector.childCount - 1].Add(rde._element ??= rde._createElement(rde._reference, property));
					break;
				case ButtonAttribute.Location.Below:
					EditorApplication.delayCall += () =>
					{
						if (rde.panel != null)
							field.Add(rde._element ??= rde._createElement(rde._reference, property));
					};
					break;
				case ButtonAttribute.Location.Above:
					rde.Add(rde._element ??= rde._createElement(rde._reference, property)); // Do nothing
					return;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private static void Detach(DetachFromPanelEvent evt, RepositionDrawerElement<T> rde) => rde._element?.RemoveFromHierarchy();
	}

	[CustomPropertyDrawer(typeof(ButtonAttribute))]
	public sealed class ButtonDecorator : DecoratorDrawer
	{
		public override VisualElement CreatePropertyGUI()
		{
			var buttonAttribute = (ButtonAttribute)attribute;
			return new RepositionDrawerElement<ButtonAttribute>(
				CreateButton,
				buttonAttribute.DisplayLocation,
				buttonAttribute
			);
		}

		private static Button CreateButton(ButtonAttribute buttonAttribute, SerializedProperty property) =>
			new(() => Invoke(buttonAttribute, property))
			{
				text = string.IsNullOrEmpty(buttonAttribute.DisplayNameOverride)
					? ObjectNames.NicifyVariableName(buttonAttribute.MethodName)
					: buttonAttribute.DisplayNameOverride
			};

		private static void Invoke(ButtonAttribute attribute, SerializedProperty serializedProperty)
		{
			// TODO write something that invokes the correct method.
		}

		public override void OnGUI(Rect position) => EditorGUI.HelpBox(position, $"{nameof(ButtonAttribute)} is unsupported when using IMGUI.", MessageType.Warning);
	}
}