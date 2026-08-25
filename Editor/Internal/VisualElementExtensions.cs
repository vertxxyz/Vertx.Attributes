using System;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Vertx.Attributes.Editor.Internal
{
	public static class VisualElementExtensions
	{
		public static void RegisterSerializedPropertyBindEvent(this VisualElement element, Action<EventBase> callback) => element.RegisterCallback<SerializedPropertyBindEvent>(e => callback(e));
	}
}