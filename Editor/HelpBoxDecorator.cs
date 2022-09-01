using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Vertx.Attributes.Editor
{
	[CustomPropertyDrawer(typeof(HelpBoxAttribute))]
	public class HelpBoxDecorator : DecoratorDrawer
	{
#if UNITY_2020_1_OR_NEWER
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
			return new HelpBox(a.Text, type);
		}
#endif
		
		public override void OnGUI(Rect position)
		{
			position.height -= EditorGUIUtility.standardVerticalSpacing;
			HelpBoxAttribute a = (HelpBoxAttribute) attribute;
			EditorGUI.HelpBox(position, a.Text, (MessageType) a.Type);
		}

		public override float GetHeight()
		{
			HelpBoxAttribute a = (HelpBoxAttribute) attribute;
			string texture;
			switch (a.Type)
			{
				case HelpBoxAttribute.MessageType.None:
					texture = null;
					break;
				case HelpBoxAttribute.MessageType.Info:
					texture = "console.infoicon";
					break;
				case HelpBoxAttribute.MessageType.Warning:
					texture = "console.warnicon";
					break;
				case HelpBoxAttribute.MessageType.Error:
					texture = "console.erroricon";
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			float height = EditorStyles.helpBox.CalcHeight(
				EditorGUIUtility.TrTextContentWithIcon(a.Text, texture),
				EditorGUIUtility.currentViewWidth - 37 // This is conservative to count the scrollbar (24 otherwise)
			);

			return height + EditorGUIUtility.standardVerticalSpacing;
		}
	}
}