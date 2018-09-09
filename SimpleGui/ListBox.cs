using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace SimpleGui
{
    public class ListBox : Control
    {
        public List<ListBoxItem> Items { get; set; } = new List<ListBoxItem>();

        public ListBoxItem SelectedItem { get; protected set; }

        public int ItemHeight = 34;
        public int ItemPadding = 2;

        public ListBox()
        {
            ColorType = ControlColorType.Input;
        }

        public void AddItem(string text)
        {
            var item = new ListBoxItem(this)
            {
                Position = new Vector2(ItemPadding, (Items.Count * (ItemHeight + ItemPadding)) + ItemPadding),
                Size = new Vector2(Size.X - (2 * ItemPadding), ItemHeight),
                Text = text,
                IsToggleable = true,
                ClickUntoggles = true,
                Toggled = () =>
                {
                }
            };
            item.Initialize();
            AddChild(item);
            Items.Add(item);
        }

        public void SelectItem(ListBoxItem item)
        {
            SelectedItem = item;
            foreach (var i in Items)
            {
                if (i != item)
                {
                    i.IsToggled = false;
                }
            }
        }
    }
}
