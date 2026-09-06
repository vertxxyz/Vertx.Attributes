using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Vertx.Attributes.Editor.Internal;

namespace Vertx.Attributes.Editor
{
	public sealed class DecoratePropertyElement : VisualElement
	{
		public bool HasTarget => _target != null;

		private readonly Action<SerializedProperty, VisualElement> _modifyElement;
		private int _modifiedChildCount;
		private PropertyField? _target;
		private SerializedProperty? _property;

		public DecoratePropertyElement(Action<SerializedProperty, VisualElement> modifyElement)
		{
			_modifyElement = modifyElement;
			RegisterCallback<AttachToPanelEvent, DecoratePropertyElement>(Attach, this);
			// RegisterCallback<DetachFromPanelEvent, DecoratePropertyElement>(Detach, this);
		}

		private static void Attach(AttachToPanelEvent evt, DecoratePropertyElement de)
		{
			if (!VisualElementUtilities.TryFindInParent(de, out PropertyField? field))
			{
				Debug.LogWarning($"{nameof(PropertyField)} parent could not be located. Please report a bug with Vertx.Attributes.");
				return;
			}

			de._target = field;
			de._property = VisualElementUtilities.GetSerializedProperty(field);

			field.RegisterSerializedPropertyBindEvent(
				_ =>
				{
					if (de._target.childCount <= 1)
						return;

					de._modifyElement(VisualElementUtilities.GetSerializedProperty(de._target)!, de._target[de._target.childCount - 1]);
				}
			);
			de.ModifyAll();
		}

		public void ModifyAll()
		{
			if (_target == null)
				return;

			for (var i = 1; i < _target.childCount; i++)
				_modifyElement(_property!, _target[i]);
		}

		public void Decorate(SerializedProperty property, VisualElement element) => _modifyElement(property, element);
	}
}