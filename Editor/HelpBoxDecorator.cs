using UnityEditor;
using UnityEngine.UIElements;

namespace Vertx.Attributes.Editor
{
	[CustomPropertyDrawer(typeof(HelpBoxAttribute))]
	public sealed class HelpBoxDecorator : DecoratorDrawer
	{
#if UNITY_2022_1_OR_NEWER
		public override VisualElement CreatePropertyGUI()
		{
			var a = (HelpBoxAttribute)attribute;
			HelpBoxMessageType type;
			switch (a.Type)
			{
				case HelpBoxAttribute.MessageType.None:
					type = HelpBoxMessageType.None;
					break;
				case HelpBoxAttribute.MessageType.Info:
					type = HelpBoxMessageType.Info;
					break;
				case HelpBoxAttribute.MessageType.Warning:
					type = HelpBoxMessageType.Warning;
					break;
				case HelpBoxAttribute.MessageType.Error:
					type = HelpBoxMessageType.Error;
					break;
				default:
					goto case HelpBoxAttribute.MessageType.None;
			}

			var helpBox = new HelpBox(a.Text, type);
			if (a.Size == HelpBoxAttribute.MessageSize.Small)
			{
				VisualElement element = helpBox.Q(null, HelpBox.iconUssClassName);
				if (element != null)
				{
					element.style.minHeight = 18;
				}
			}
			return helpBox;
		}
#endif
	}
}