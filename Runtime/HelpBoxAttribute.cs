using UnityEngine;

namespace Vertx.Attributes
{
	public class HelpBoxAttribute : PropertyAttribute
	{
		public enum MessageType
		{
			None,
			Info,
			Warning,
			Error
		}

		public enum MessageSize
		{
			Default,
			Small
		}
		
		public string Text { get; }
		public MessageType Type { get; }
		public MessageSize Size { get; }

		public HelpBoxAttribute(string text, MessageType type = MessageType.Info, MessageSize size = MessageSize.Default)
		{
			Text = text;
			Type = type;
			Size = size;
		}
	}
}