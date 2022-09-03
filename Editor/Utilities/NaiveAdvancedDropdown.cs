using System;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Vertx.Attributes.Editor
{
	internal class NaiveAdvancedDropdown<T> : AdvancedDropdown
	{
		// ReSharper disable once ArrangeObjectCreationWhenTypeEvident
		// ReSharper disable once StaticMemberInGenericType
		private static readonly AdvancedDropdownState s_State = new AdvancedDropdownState();
		private readonly string _title;
		private readonly string[] _enumNames;
		private readonly Action<int, T> _onSelected;
		private readonly T _data;

		public NaiveAdvancedDropdown(
			Vector2 minSize,
			string title,
			string[] enumNames,
			Action<int, T> onSelected,
			T data
		) : base(s_State)
		{
			minimumSize = minSize;
			_title = title;
			_enumNames = enumNames;
			_onSelected = onSelected;
			_data = data;
		}

		protected override AdvancedDropdownItem BuildRoot()
		{
			AdvancedDropdownItem root = new AdvancedDropdownItem(_title) { id = int.MaxValue };
			for (var i = 0; i < _enumNames.Length; i++)
			{
				string name = _enumNames[i];
				root.AddChild(new AdvancedDropdownItem(name) { id = i });
			}

			return root;
		}

		protected override void ItemSelected(AdvancedDropdownItem item) => _onSelected(item.id, _data);
	}
}