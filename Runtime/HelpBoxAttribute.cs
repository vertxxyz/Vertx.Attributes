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
		
		public string Text { get; }
		public MessageType Type { get; }

		public HelpBoxAttribute(string text, MessageType type)
		{
			Text = text;
			Type = type;
		}
	}
}