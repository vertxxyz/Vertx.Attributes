using System;
using UnityEngine.UIElements;

namespace Vertx.Attributes.Editor
{
	/// <summary>
	/// A UIToolkit GenericMenu dropdown button.
	/// </summary>
	internal class DropdownButton : BaseField<string>
	{
		public const string UssClassName = "vertx-dropdown-button";
		public const string LabelUssClassName = UssClassName + "__label";
		public const string DropdownUssClassName = UssClassName + "__dropdown";
		public const string IconUssClassName = UssClassName + "__icon";

		public string Text
		{
			get => _textElement.text;
			set => _textElement.text = value;
		}

		public string Tooltip
		{
			set => labelElement.tooltip = value;
		}

#if UNITY_2022_2_OR_NEWER
		private HelpBoxMessageType _iconType = HelpBoxMessageType.None;

		public HelpBoxMessageType IconType
		{
			get => _iconType;
			set
			{
				if (_iconType == value)
					return;

				if (_iconElement == null)
				{
					// When switching off and we have nothing, exit early
					if (value == HelpBoxMessageType.None)
					{
						_iconType = value;
						return;
					}

					// Switching on, create the icon element
					_iconElement = new VisualElement
					{
						pickingMode = PickingMode.Ignore
					};
					_iconElement.AddToClassList(HelpBox.iconUssClassName);
					_iconElement.AddToClassList(IconUssClassName);
					labelElement.Add(_iconElement);
				}

				if (value == HelpBoxMessageType.None)
				{
					// Switching off, remove the last icon class and make this invisible.
					_iconElement.RemoveFromClassList(GetIconClass(_iconType));
					_iconElement.visible = false;
					_iconType = value;
					return;
				}

				// Switching on
				_iconElement.visible = true;
				if (_iconType != HelpBoxMessageType.None)
					_iconElement.RemoveFromClassList(GetIconClass(_iconType));
				_iconElement.AddToClassList(GetIconClass(value));
				_iconType = value;
			}
		}

		private static string GetIconClass(HelpBoxMessageType messageType) =>
			messageType switch
			{
				HelpBoxMessageType.Info => HelpBox.iconInfoUssClassName,
				HelpBoxMessageType.Warning => HelpBox.iconwarningUssClassName,
				HelpBoxMessageType.Error => HelpBox.iconErrorUssClassName,
				_ => null
			};
#endif

		private readonly TextElement _textElement;
		private VisualElement _iconElement;
		private VisualElement _internalVisualInput;
		private VisualElement VisualInput => _internalVisualInput ??= this.Q<VisualElement>(null, inputUssClassName);

#if UNITY_2022_2_OR_NEWER
		public DropdownButton(string displayValue, HelpBoxMessageType iconType = HelpBoxMessageType.None)
			: this(null, displayValue, iconType) { }
#else
		public DropdownButton(string displayValue, Action<PointerDownEvent, DropdownButton> onSelect)
			: this(null, displayValue) { }
#endif


		public DropdownButton(
			string label,
			string displayValue,
#if UNITY_2022_2_OR_NEWER
			HelpBoxMessageType iconType = HelpBoxMessageType.None
#endif
		) : base(label, null)
		{
			RegisterCallback<AttachToPanelEvent, string>(StyleSheetUtils.AddStyleSheetOnPanelEvent, SheetPaths.AttributeStyles);

			// Label
			labelElement.AddToClassList(BasePopupField<string, string>.labelUssClassName);
			labelElement.AddToClassList(LabelUssClassName);

			// Visual input
			// VisualInput.pickingMode = PickingMode.Position;
			VisualInput.AddToClassList(BasePopupField<string, string>.inputUssClassName);
			VisualInput.AddToClassList(DropdownUssClassName);

			// Text element
			_textElement = new TextElement { pickingMode = PickingMode.Ignore };
			_textElement.AddToClassList(BasePopupField<string, string>.textUssClassName);
			_textElement.text = displayValue;
			VisualInput.Add(_textElement);

			// Arrow element
			var arrowElement = new VisualElement { pickingMode = PickingMode.Ignore };
			arrowElement.AddToClassList(BasePopupField<string, string>.arrowUssClassName);
			VisualInput.Add(arrowElement);

			// Initialisation
			AddToClassList(BasePopupField<string, string>.ussClassName);
			AddToClassList(alignedFieldUssClassName);
			AddToClassList(UssClassName);

#if UNITY_2022_2_OR_NEWER
			IconType = iconType;
#endif
			// pickingMode = PickingMode.Ignore;
		}

		public delegate void ButtonCallback<in T>(PointerDownEvent evt, DropdownButton button, T data);

		public void RegisterClickCallback<T>(ButtonCallback<T> callback, T data, bool bindToRoot = false)
		{
			VisualElement bindTo;
			if (bindToRoot)
				bindTo = this;
			else
			{
				VisualInput.pickingMode = PickingMode.Position;
				bindTo = VisualInput;
			}

			bindTo.RegisterCallback<PointerDownEvent, (ButtonCallback<T> callback, T data, DropdownButton button)>(
				(evt, args) => args.callback(evt, args.button, args.data), (callback, data, this)
			);
		}
	}
}