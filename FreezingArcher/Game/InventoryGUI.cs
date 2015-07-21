//
//  InventoryGUI.cs
//
//  Author:
//       Fin Christensen <christensen.fin@gmail.com>
//
//  Copyright (c) 2015 Fin Christensen
//
//  This program is free software; you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation; either version 2 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program; if not, write to the Free Software
//  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
//
using System;
using FreezingArcher.Messaging.Interfaces;
using FreezingArcher.Content;
using FreezingArcher.Messaging;
using FreezingArcher.Math;
using FreezingArcher.Core;
using Gwen.Control;
using Gwen.Control.Layout;
using Gwen.DragDrop;
using FreezingArcher.Output;
using System.Drawing;
using FreezingArcher.DataStructures;
using FreezingArcher.Localization;
using Gwen;
using System.Collections.Generic;
using System.Linq;
using Pencil.Gaming;

namespace FreezingArcher.Game
{
    public class InventorySpace : Button
    {
        public InventorySpace (Base parent, int boxSize, Inventory inventory) : base (parent)
        {
            DrawDebugOutlines = true;
            foreach (var child in Children)
                child.DrawDebugOutlines = false;
            ShouldDrawBackground = false;
            BoundsOutlineColor = parent.GetCanvas ().Skin.Colors.Label.Dark;
            DragAndDrop_SetPackage (true, "item_drag");
            this.boxSize = boxSize;
            this.inventory = inventory;
        }

        readonly int boxSize;
        readonly Inventory inventory;

        public override bool DragAndDrop_CanAcceptPackage (Package p)
        {
            return true;
        }

        public override bool DragAndDrop_Draggable ()
        {
            return false;
        }

        public override void DragAndDrop_HoverEnter (Package p, int x, int y)
        {
            ShouldDrawBackground = true;
        }

        public override void DragAndDrop_HoverLeave (Package p)
        {
            ShouldDrawBackground = false;
        }

        public override bool DragAndDrop_HandleDrop (Package p, int x, int y)
        {
            var item = p.UserData as Tuple<int, int, InventoryButton, ItemComponent>;

            int pos_x = X / boxSize - item.Item1;
            int pos_y = Y / boxSize - item.Item2;
            int pos_w = item.Item3.Width / boxSize;
            int pos_h = item.Item3.Height / boxSize;

            if (pos_x < 0 || pos_x + pos_w >= inventory.Size.X || pos_y < 0 || pos_y + pos_h >= inventory.Size.Y)
                return false;

            var old_pos = inventory.GetPositionOfItem (item.Item4);
            inventory.TakeOut (item.Item4);
            if (inventory.Insert (item.Item4, new Vector2i (pos_x, pos_y)))
            {
                item.Item3.X = pos_x * boxSize + 1;
                item.Item3.Y = pos_y * boxSize + 1;
                return true;
            }
            if (!inventory.Insert (item.Item4, old_pos))
            {
                Logger.Log.AddLogEntry (LogLevel.Error, "InventorySpace", "Lost an item from inventory!");
                item.Item3.DelayedDelete ();
            }
            return false;
        }
    }

    public class InventoryButton : Button
    {
        public InventoryButton (Base parent, Inventory inventory, ItemComponent item, Vector2i position, int boxSize) :
            base (parent)
        {
            this.boxSize = boxSize;
            this.IsToggle = true;
            this.inventory = inventory;
            this.item = item;
            X = position.X * boxSize + 1;
            Y = position.Y * boxSize + 1;

            if (!string.IsNullOrEmpty(item.ImageLocation))
                SetImage(item.ImageLocation, true);

            UpdateSize();
            DragAndDrop_SetPackage (true, "item_drag");
        }

        readonly Inventory inventory;
        readonly ItemComponent item;
        readonly int boxSize;

        public void UpdateSize()
        {
            if (item.Orientation == Orientation.Horizontal)
            {
                Width = item.Size.X * boxSize - 1;
                Height = item.Size.Y * boxSize - 1;
                if (m_Image != null)
                {
                    m_Image.Width = Width;
                    m_Image.Height = Height;
                    m_Image.SetUV(0, 0, 1, 1);
                }
            }
            else
            {
                Width = item.Size.Y * boxSize - 1;
                Height = item.Size.X * boxSize - 1;
                if (m_Image != null)
                {
                    m_Image.Width = Width;
                    m_Image.Height = Height;
                    m_Image.SetUV(0, 1, 1, 0);
                }
            }
        }

        public ItemComponent Item
        {
            get
            {
                return item;
            }
        }

        public override void DragAndDrop_EndDragging (bool success, int x, int y)
        {
            base.DragAndDrop_EndDragging (success, x, y);
            Show();
            IsDepressed = false;
        }

        public override bool DragAndDrop_CanAcceptPackage (Package p)
        {
            return false;
        }

        public override bool DragAndDrop_Draggable ()
        {
            return true;
        }

        public override void DragAndDrop_StartDragging (Package package, int x, int y)
        {
            var loc = CanvasPosToLocal (new Point (x, y));
            int pos_x = loc.X / boxSize;
            int pos_y = loc.Y / boxSize;
            package.UserData = new Tuple<int, int, InventoryButton, ItemComponent> (pos_x, pos_y, this, item);
            Hide ();
            base.DragAndDrop_StartDragging (package, x, y);
        }
    }

    public class InventoryGUI : IMessageConsumer
    {
        public InventoryGUI (Application application, Inventory inventory, MessageProvider messageProvider, Base parent)
        {
            this.inventory = inventory;
            this.application = application;
            ValidMessages = new[] { (int) MessageId.WindowResize, (int) MessageId.UpdateLocale, (int) MessageId.Input };
            messageProvider += this;
            Localizer.Instance.CurrentLocale = LocaleEnum.de_DE;

            spaces = new InventorySpace[inventory.Size.X, inventory.Size.Y];

            canvasFrame = new Base(parent);
            canvasFrame.Width = parent.Width;
            canvasFrame.Height = parent.Height;

            window = new WindowControl (canvasFrame, Localizer.Instance.GetValueForName("inventory"));
            window.DisableResizing ();
            window.IsMoveable = false;

            itemGridFrame = new Base (window);
            itemGridFrame.SetSize ((BoxSize + 1) * inventory.Size.X, (BoxSize + 1) * inventory.Size.Y);

            itemInfoFrame = new Base (window);
            const int infoFrameSize = 300;
            itemInfoFrame.SetSize (infoFrameSize, itemGridFrame.Height);
            itemGridFrame.X += itemInfoFrame.Width + 4;

            const int toolbarFrameSize = 30;
            toolbarFrame = new Base(window);
            toolbarFrame.Width = itemGridFrame.Width + itemInfoFrame.Width;
            toolbarFrame.Height = toolbarFrameSize;
            toolbarFrame.Y = itemGridFrame.Height - 4;

            dropBtn = new Button(toolbarFrame);
            dropBtn.AutoSizeToContents = true;
            dropBtn.Padding = btnPadding;
            dropBtn.Text = Localizer.Instance.GetValueForName("drop");
            dropBtn.X = toolbarFrame.Width - dropBtn.Width;
            dropBtn.Y = (toolbarFrameSize - dropBtn.Height) / 2;
            dropBtn.IsDisabled = true;

            useBtn = new Button(toolbarFrame);
            useBtn.AutoSizeToContents = true;
            useBtn.Padding = btnPadding;
            useBtn.Text = Localizer.Instance.GetValueForName("use_equip");
            useBtn.X = dropBtn.X - useBtn.Width - 8;
            useBtn.Y = (toolbarFrameSize - useBtn.Height) / 2;
            useBtn.IsDisabled = true;

            rotateBtn = new Button(toolbarFrame);
            rotateBtn.AutoSizeToContents = true;
            rotateBtn.Padding = btnPadding;
            rotateBtn.Text = Localizer.Instance.GetValueForName("rotate");
            rotateBtn.X = useBtn.X - rotateBtn.Width - 8;
            rotateBtn.Y = (toolbarFrameSize - rotateBtn.Height) / 2;
            rotateBtn.IsDisabled = true;

            rotateBtn.Clicked += (sender, argument) => {
                if (rotateBtn.IsDisabled)
                    return;
                
                var pos = inventory.GetPositionOfItem(toggledBtn.Item);
                var item = inventory.TakeOut(pos);
                var prev_orientation = item.Orientation;
                item.Orientation =
                    item.Orientation == Orientation.Horizontal ? Orientation.Vertical : Orientation.Horizontal;
                if (!inventory.Insert(item, pos))
                {
                    item.Orientation = prev_orientation;
                    if (!inventory.Insert(item, pos))
                    {
                        Logger.Log.AddLogEntry(LogLevel.Error, "InventoryGUI",
                            "Lost an inventory item while rotating!");
                        toggledBtn.DelayedDelete();
                        toggledBtn = null;
                        return;
                    }
                }
                toggledBtn.UpdateSize();
            };

            window.SetSize (itemGridFrame.Width + itemInfoFrame.Width + 16,
                itemGridFrame.Height + toolbarFrameSize + 28);
            window.SetPosition ((canvasFrame.Width - window.Width) / 2, (canvasFrame.Height - window.Height) / 2);
            window.Hide();

            int w = 0, h = 0;

            for (int y = 0; y < inventory.Size.Y; y++)
            {
                for (int x = 0; x < inventory.Size.X; x++)
                {
                    spaces [x, y] = new InventorySpace (itemGridFrame, BoxSize, inventory);
                    spaces [x, y].X = w;
                    spaces [x, y].Y = h;
                    spaces [x, y].Width = BoxSize + 1;
                    spaces [x, y].Height = BoxSize + 1;

                    w += BoxSize;
                }
                h += BoxSize;
                w = 0;
            }

            imagePanel = new ImagePanel(itemInfoFrame);
            imagePanel.Width = infoFrameSize;
            imagePanel.Height = itemGridFrame.Height / 3;
            imagePanel.Hide();

            items = new List<InventoryButton>();
            inventory.Items.ForEach((item, position) => {
                var btn = new InventoryButton(itemGridFrame, inventory, item, position, BoxSize);

                btn.ToggledOn += (sender, arguments) => {
                    if (toggledBtn != null)
                        toggledBtn.ToggleState = false;

                    toggledBtn = btn;
                    imagePanel.ImageName = btn.Item.ImageLocation;
                    imagePanel.Show();
                    setDescriptionLabel(Localizer.Instance.GetValueForName(btn.Item.Description));
                    dropBtn.IsDisabled = false;
                    useBtn.IsDisabled = false;
                    rotateBtn.IsDisabled = false;
                };

                btn.ToggledOff += (sender, arguments) => {
                    imagePanel.Hide();
                    setDescriptionLabel(string.Empty);
                    toggledBtn = null;
                    dropBtn.IsDisabled = true;
                    useBtn.IsDisabled = true;
                    rotateBtn.IsDisabled = true;
                };

                items.Add(btn);
            });
        }

        Label[] labels;

        void setDescriptionLabel(string text, int height = 4)
        {
            var texts = text.Split('\n');
            var y = imagePanel.Height + height;

            if (labels != null)
            {
                foreach (var l in labels)
                {
                    l.DelayedDelete();
                }
            }

            labels = new Label[texts.Length];
            int i = 0;
            foreach (var t in texts)
            {
                labels[i] = new Label(itemInfoFrame);
                labels[i].Width = itemInfoFrame.Width;
                labels[i].Y = y;
                labels[i].Text = t;
                y += labels[i].Height + height;
                i++;
            }
        }

        public static readonly int BoxSize = 64;

        static readonly Padding btnPadding = new Padding(10, 0, 10, 0);

        InventoryButton toggledBtn;

        readonly Inventory inventory;
        readonly ImagePanel imagePanel;
        readonly Button dropBtn;
        readonly Button useBtn;
        readonly Button rotateBtn;
        readonly InventorySpace[,] spaces;
        readonly WindowControl window;
        readonly Base itemGridFrame;
        readonly Base itemInfoFrame;
        readonly Base toolbarFrame;
        readonly Base canvasFrame;
        readonly List<InventoryButton> items;
        readonly Application application;

        #region IMessageConsumer implementation

        public void ConsumeMessage (IMessage msg)
        {
            if (msg.MessageId == (int) MessageId.WindowResize)
            {
                var wrm = msg as WindowResizeMessage;

                canvasFrame.Width = wrm.Width;
                canvasFrame.Height = wrm.Height;
                window.SetPosition ((canvasFrame.Width - window.Width) / 2, (canvasFrame.Height - window.Height) / 2);
            }

            if (msg.MessageId == (int) MessageId.UpdateLocale)
            {
                window.Title = Localizer.Instance.GetValueForName("inventory");
                dropBtn.Text = Localizer.Instance.GetValueForName("drop");
                dropBtn.X = toolbarFrame.Width - dropBtn.Width;
                useBtn.Text = Localizer.Instance.GetValueForName("use_equip");
                useBtn.X = dropBtn.X - useBtn.Width - 8;
                rotateBtn.Text = Localizer.Instance.GetValueForName("rotate");
                rotateBtn.X = useBtn.X - rotateBtn.Width - 8;
                if (toggledBtn != null)
                    setDescriptionLabel(Localizer.Instance.GetValueForName(toggledBtn.Item.Description));
            }

            if (msg.MessageId == (int) MessageId.Input)
            {
                var im = msg as InputMessage;

                if (im.Keys.Any(k => k.Action == KeyAction.Press && k.KeyAction == "drop"))
                {
                    Localizer.Instance.CurrentLocale =
                        Localizer.Instance.CurrentLocale == LocaleEnum.en_US ? LocaleEnum.de_DE : LocaleEnum.en_US;
                }

                if (im.Keys.Any(k => k.Action == KeyAction.Press && k.KeyAction == "inventory"))
                {
                    if (window.IsVisible)
                    {
                        window.Hide();
                        application.Window.CaptureMouse();
                    }
                    else
                    {
                        window.Show();
                        application.Window.ReleaseMouse();
                    }
                }

                if (im.Keys.Any(k => k.Action == KeyAction.Press && k.KeyAction == "camera"))
                {
                    inventory.PrintStorage("Inventory:");
                }
            }
        }

        public int[] ValidMessages { get; private set; }

        #endregion
    }
}
