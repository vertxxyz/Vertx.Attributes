using System;
using System.Reflection;
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
#if UNITY_2022_1_OR_NEWER
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
			new Button(() => Invoke(buttonAttribute, property))
			{
				text = string.IsNullOrEmpty(buttonAttribute.DisplayNameOverride)
					? ObjectNames.NicifyVariableName(buttonAttribute.MethodName)
					: buttonAttribute.DisplayNameOverride
			};

		private static void Invoke(ButtonAttribute attribute, SerializedProperty property)
		{
			UnityEngine.Object o = property.serializedObject?.targetObject;
			
			if (attribute.StaticMethodType != null)
			{
				if (InvokeStatic(attribute, attribute.StaticMethodType, property))
					return;
			}
			
			if (o == null)
			{
				Debug.LogError($"No object was found and {nameof(ButtonAttribute.StaticMethodType)} was not provided to {nameof(ButtonAttribute)}, so no static method can be invoked.");
				return;
			}

			Type type = o.GetType();
			MethodInfo method = type.GetMethod(attribute.MethodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (method == null)
			{
				if (!InvokeStatic(attribute, type, property))
					Debug.LogError($"Method \"{attribute.MethodName}\" could not be found on {type}.");
				return;
			}
			
			switch (ValidateParameters(method))
			{
				case ParameterResult.None:
					method.Invoke(o, null);
					break;
				case ParameterResult.SerializedProperty:
					method.Invoke(o, new object[] { property });
					break;
				case ParameterResult.Invalid:
					Debug.LogError($"Parameters on method \"{method}\" must be none, or one {nameof(SerializedProperty)}.");
					return;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private static bool InvokeStatic(ButtonAttribute attribute, Type type, SerializedProperty property)
		{
			MethodInfo method = type.GetMethod(attribute.MethodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			if (method == null)
				return false;
			
			switch (ValidateParameters(method))
			{
				case ParameterResult.None:
					method.Invoke(null, null);
					return true;
				case ParameterResult.SerializedProperty:
					method.Invoke(null, new object[] { property });
					return true; 
				case ParameterResult.Invalid:
					Debug.LogError($"Parameters on method \"{method}\" must be none, or one {nameof(SerializedProperty)}.");
					return true;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		enum ParameterResult
		{
			None,
			SerializedProperty,
			Invalid
		}

		private static ParameterResult ValidateParameters(MethodInfo method)
		{
			ParameterInfo[] parameters = method.GetParameters();
			if (parameters.Length == 0)
				return ParameterResult.None;
			if (parameters.Length != 1)
				return ParameterResult.Invalid;
			return parameters[0].ParameterType == typeof(SerializedProperty) ? ParameterResult.SerializedProperty : ParameterResult.Invalid;
		}
#endif

		public override void OnGUI(Rect position) => EditorGUI.HelpBox(position, $"{nameof(ButtonAttribute)} is unsupported when using IMGUI.", MessageType.Warning);
	}
}