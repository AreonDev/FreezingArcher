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
using Gwen.DragDrop;
using FreezingArcher.Output;
using System.Drawing;
using FreezingArcher.DataStructures;
using FreezingArcher.Localization;
using Gwen;
using System.Collections.Generic;
using System.Linq;
using Pencil.Gaming;
using Jitter.Dynamics;
using Jitter.LinearMath;
using Jitter.Collision.Shapes;
using FreezingArcher.Renderer.Scene.SceneObjects;

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
            return p.Name == "item_drag";
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

            var bar_pos = inventory.GetPositionOfBarItem(item.Item4);

            if (bar_pos < inventory.InventoryBar.Length)
                inventory.RemoveFromBar(bar_pos);

            var old_pos = inventory.GetPositionOfItem (item.Item4);
            inventory.TakeOut (item.Item4);

            if (inventory.Insert (item.Item4, new Vector2i (pos_x, pos_y)))
            {
                if (bar_pos < inventory.InventoryBar.Length)
                    inventory.PutInBar(pos_x, pos_y, bar_pos);
                
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

    public class InventoryBarSpace : Button
    {
        public InventoryBarSpace (Base parent, MessageProvider messageProvider, Inventory inventory,
            InventoryGUI inventoryGui, List<InventoryBarButton> barItems, int boxSize) : base (parent)
        {
            DrawDebugOutlines = true;
            foreach (var child in Children)
                child.DrawDebugOutlines = false;
            ShouldDrawBackground = false;
            BoundsOutlineColor = Color.Gray;
            this.boxSize = boxSize;
            this.inventory = inventory;
            this.barItems = barItems;
            this.inventoryGui = inventoryGui;
            this.messageProvider = messageProvider;
        }

        readonly int boxSize;
        readonly Inventory inventory;
        readonly List<InventoryBarButton> barItems;
        readonly MessageProvider messageProvider;
        readonly InventoryGUI inventoryGui;

        public override bool DragAndDrop_CanAcceptPackage (Package p)
        {
            return p.Name == "item_drag" || p.Name == "bar_drag";
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
            if (p.Name == "bar_drag")
            {
                var invBarBtn = p.UserData as InventoryBarButton;

                if (invBarBtn != null)
                {
                    var pos = (byte) (X / boxSize);
                    invBarBtn.X = X + 1;
                    inventory.MoveBarItemToPosition(invBarBtn.Item, pos);
                    invBarBtn.Item.Entity.GetComponent<ModelComponent>().Model.Enabled = pos == inventory.ActiveBarPosition;
                    invBarBtn.ToggleState = pos == inventory.ActiveBarPosition;
                    return true;
                }
            }
            else if (p.Name == "item_drag")
            {
                var invBarTuple = p.UserData as Tuple<int, int, InventoryButton, ItemComponent>;

                if (invBarTuple != null)
                {
                    var pos = (byte) (X / boxSize);
                    if (inventory.PutInBar(inventory.GetPositionOfItem(invBarTuple.Item4), pos))
                    {
                        var btn = new InventoryBarButton(Parent, messageProvider, barItems, inventory, inventoryGui,
                            invBarTuple.Item4, pos, boxSize);
                        barItems.Add(btn);
                        btn.Item.Entity.GetComponent<ModelComponent>().Model.Enabled = pos == inventory.ActiveBarPosition;
                        btn.ToggleState = pos == inventory.ActiveBarPosition;
                        return true;
                    }
                }
            }

            return false;
        }
    }

    public class InventoryButton : Button, IMessageConsumer
    {
        public InventoryButton (Base parent, MessageProvider messageProvider, Inventory inventory, ItemComponent item,
            InventoryGUI inventoryGui, Vector2i position, int boxSize) : base (parent)
        {
            ValidMessages = new[] { (int) MessageId.ItemUsageChanged };
            this.boxSize = boxSize;
            this.IsToggle = true;
            this.inventory = inventory;
            this.inventoryGui = inventoryGui;
            this.item = item;
            X = position.X * boxSize + 1;
            Y = position.Y * boxSize + 1;

            if (!string.IsNullOrEmpty(item.ImageLocation))
                SetImage(item.ImageLocation, true);

            usageProgress = new ProgressBar(this);
            usageProgress.AutoLabel = false;
            usageProgress.Text = string.Empty;
            usageProgress.Value = 1 - item.Usage;
            usageProgress.TextPadding = Padding.Zero;
            UpdateSize();
            DragAndDrop_SetPackage (true, "item_drag");

            messageProvider += this;
        }

        readonly Inventory inventory;
        readonly ItemComponent item;
        readonly int boxSize;
        readonly ProgressBar usageProgress;
        readonly InventoryGUI inventoryGui;

        public void UpdateSize()
        {
            if (item.Orientation == Orientation.Horizontal)
            {
                Width = item.Size.X * boxSize - 1;
                Height = item.Size.Y * boxSize - 1;
                usageProgress.SetSize(Width, 10);
                usageProgress.Y = Height - usageProgress.Height;
                usageProgress.X = 0;

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
                usageProgress.SetSize(Width, 10);
                usageProgress.Y = Height - usageProgress.Height;
                usageProgress.X = 0;

                if (m_Image != null)
                {
                    m_Image.Width = Width;
                    m_Image.Height = Width / (Height / Width);
                    m_Image.SetUV(0, 0, 1, 1);
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
            return p.Name == "item_drag";
        }

        public override bool DragAndDrop_Draggable ()
        {
            return true;
        }

        public override bool DragAndDrop_HandleDrop (Package p, int x, int y)
        {
            return false;
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

        #region IMessageConsumer implementation

        public void ConsumeMessage (IMessage msg)
        {
            if (msg.MessageId == (int) MessageId.ItemUsageChanged)
            {
                var ucm = msg as ItemUsageChangedMessage;
                if (ucm.Entity.Name == item.Entity.Name)
                {
                    if (ucm.Usage > 0.999)
                    {
                        inventoryGui.dropItem(this, Item, inventory, true);
                    }
                    else
                        usageProgress.Value = 1 - ucm.Usage;
                }
            }
        }

        public int[] ValidMessages { get; private set; }

        #endregion
    }

    public class InventoryBarButton : Button, IMessageConsumer
    {
        public InventoryBarButton (Base parent, MessageProvider messageProvider, List<InventoryBarButton> barItems,
            Inventory inventory, InventoryGUI inventoryGui, ItemComponent item, byte position, int boxSize) :
        base (parent)
        {
            ValidMessages = new[] { (int) MessageId.ItemUsageChanged, (int) MessageId.BarItemMoved };
            this.boxSize = boxSize;
            this.IsToggle = true;
            this.item = item;
            this.inventory = inventory;
            this.barItems = barItems;
            this.inventoryGui = inventoryGui;

            if (!string.IsNullOrEmpty(item.ImageLocation))
                SetImage(item.ImageLocation, true);

            Width = boxSize - 1;
            Height = boxSize - 1;
            X = boxSize * position + 1;
            Y = 2;

            usageProgress = new ProgressBar(this);
            usageProgress.AutoLabel = false;
            usageProgress.Text = string.Empty;
            usageProgress.TextPadding = Padding.Zero;
            usageProgress.Value = 1 - item.Usage;
            usageProgress.SetSize(Width, 10);
            usageProgress.Y = Height - usageProgress.Height;
            usageProgress.X = 0;

            var max = item.Size.X > item.Size.Y ? item.Size.X : item.Size.Y;
            m_Image.Width = (boxSize / max) * item.Size.X;
            m_Image.Height = (boxSize / max) * item.Size.Y;
            DragAndDrop_SetPackage (true, "bar_drag");

            messageProvider += this;
            this.messageProvider = messageProvider;
        }

        readonly ItemComponent item;
        readonly int boxSize;
        readonly Inventory inventory;
        readonly List<InventoryBarButton> barItems;
        readonly ProgressBar usageProgress;
        readonly MessageProvider messageProvider;
        readonly InventoryGUI inventoryGui;

        public ItemComponent Item
        {
            get
            {
                return item;
            }
        }

        protected override void OnClicked (int x, int y)
        {}

        public override void DragAndDrop_EndDragging (bool success, int x, int y)
        {
            base.DragAndDrop_EndDragging (success, x, y);

            if (success)
                Show();
            else
            {
                Parent.RemoveChild(this, true);
                inventory.RemoveFromBar(inventory.GetPositionOfBarItem(item));
            }

            IsDepressed = false;
        }

        public override bool DragAndDrop_CanAcceptPackage (Package p)
        {
            return p.Name == "bar_drag" || p.Name == "item_drag";
        }

        public override bool DragAndDrop_Draggable ()
        {
            return true;
        }

        public override bool DragAndDrop_HandleDrop (Package p, int x, int y)
        {
            if (p.Name == "bar_drag")
            {
                var invBarBtn = p.UserData as InventoryBarButton;

                if (invBarBtn != null)
                {
                    var pos = (byte) (X / boxSize);
                    invBarBtn.X = X;
                    inventory.RemoveFromBar(pos);
                    Parent.RemoveChild(this, true);
                    inventory.MoveBarItemToPosition(invBarBtn.Item, pos);
                    invBarBtn.Item.Entity.GetComponent<ModelComponent>().Model.Enabled = pos == inventory.ActiveBarPosition;
                    invBarBtn.ToggleState = pos == inventory.ActiveBarPosition;
                    return true;
                }
            }
            else if (p.Name == "item_drag")
            {
                var invBarTuple = p.UserData as Tuple<int, int, InventoryButton, ItemComponent>;

                if (invBarTuple != null)
                {
                    var pos = (byte) (X / boxSize);
                    inventory.RemoveFromBar(pos);
                    Parent.RemoveChild(this, true);
                    if (inventory.PutInBar(inventory.GetPositionOfItem(invBarTuple.Item4), pos))
                    {
                        var btn = new InventoryBarButton(Parent, messageProvider, barItems, inventory, inventoryGui,
                            invBarTuple.Item4, pos, boxSize);
                        barItems.Add(btn);
                        btn.Item.Entity.GetComponent<ModelComponent>().Model.Enabled = pos == inventory.ActiveBarPosition;
                        btn.ToggleState = pos == inventory.ActiveBarPosition;
                        return true;
                    }
                }
            }

            return false;
        }

        public override void DragAndDrop_StartDragging (Package package, int x, int y)
        {
            package.UserData = this;
            Hide ();
            base.DragAndDrop_StartDragging (package, x, y);
        }

        #region IMessageConsumer implementation

        public void ConsumeMessage (IMessage msg)
        {
            if (msg.MessageId == (int) MessageId.ItemUsageChanged)
            {
                var ucm = msg as ItemUsageChangedMessage;
                if (ucm.Entity.Name == item.Entity.Name)
                {
                    if (ucm.Usage > 0.999)
                    {
                        inventoryGui.dropItem(this, Item, inventory, true);
                    }
                    else
                        usageProgress.Value = 1 - ucm.Usage;
                }
            }

            if (msg.MessageId == (int) MessageId.BarItemMoved)
            {
                var bim = msg as BarItemMovedMessage;
                if (bim.Item == Item)
                {
                    X = boxSize * bim.Position + 1;
                }
            }
        }

        public int[] ValidMessages { get; private set; }

        #endregion
    }

    public class InventoryBackground : Base
    {
        public InventoryBackground(Base parent, Inventory inventory, InventoryGUI inventoryGui) : 
        base(parent)
        {
            this.inventory = inventory;
            this.inventoryGui = inventoryGui;
        }

        readonly Inventory inventory;
        readonly InventoryGUI inventoryGui;

        public override bool DragAndDrop_CanAcceptPackage (Package p)
        {
            return p.Name == "item_drag";
        }

        public override bool DragAndDrop_Draggable ()
        {
            return false;
        }

        public override bool DragAndDrop_HandleDrop (Package p, int x, int y)
        {
            var item = p.UserData as Tuple<int, int, InventoryButton, ItemComponent>;
            inventoryGui.dropItem(item.Item3, item.Item4, inventory);
            return false;
        }
    }

    public class InventoryGUI : IMessageConsumer, IMessageCreator
    {
        internal void dropItem(Base btn, ItemComponent item, Inventory inv, bool destroy = false)
        {
            var barbtn = barItems.FirstOrDefault(b => b.Item == item);
            barItems.Remove(barbtn);
            if (barbtn != null)
                barbtn.Parent.RemoveChild(barbtn, true);

            inv.TakeOut (item);
            btn.Parent.RemoveChild(btn, true);
            imagePanel.Hide();
            setDescriptionLabel(string.Empty);
            item.Location = ItemLocation.Ground;
            if (!destroy)
                createMessage(new ItemDroppedMessage(item));
            else
                item.Entity.Destroy();
        }

        const int toolbarFrameSize = 30;
        const int infoFrameSize = 300;
        const int barBoxSize = 50;
        int imagePanelHeight;

        public void Init(Base parent, Inventory inventory)
        {
            this.inventory = inventory;

            spaces = new InventorySpace[inventory.Size.X, inventory.Size.Y];
            barItems = new List<InventoryBarButton>();

            canvasFrame = new InventoryBackground(parent, inventory, this);
            canvasFrame.Width = parent.Width;
            canvasFrame.Height = parent.Height;

            window = new WindowControl (canvasFrame, Localizer.Instance.GetValueForName("inventory"));
            window.DisableResizing ();
            window.IsMoveable = false;
            window.OnClose += (sender, arguments) => application.Window.CaptureMouse ();

            itemGridFrame = new Base (window);
            itemGridFrame.SetSize ((BoxSize + 1) * inventory.Size.X, (BoxSize + 1) * inventory.Size.Y);

            itemInfoFrame = new Base (window);
            itemInfoFrame.SetSize (infoFrameSize, itemGridFrame.Height);
            itemGridFrame.X += itemInfoFrame.Width + 4;

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
            dropBtn.Clicked += (sender, arguments) => {
                if (dropBtn.IsDisabled)
                    return;

                if (toggledBtn != null)
                {
                    dropItem(toggledBtn, toggledBtn.Item, inventory);
                }
            };

            useBtn = new Button(toolbarFrame);
            useBtn.AutoSizeToContents = true;
            useBtn.Padding = btnPadding;
            useBtn.Text = Localizer.Instance.GetValueForName("use");
            useBtn.X = dropBtn.X - useBtn.Width - 8;
            useBtn.Y = (toolbarFrameSize - useBtn.Height) / 2;
            useBtn.IsDisabled = true;
            useBtn.Clicked += (sender, arguments) => {
                if (useBtn.IsDisabled)
                    return;

                if (toggledBtn != null)
                {
                    if (MessageCreated != null)
                        MessageCreated(new ItemUseMessage(player, gameState.Scene, toggledBtn.Item, ItemUsage.Eatable));
                }
            };

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

            inventoryBar = new TextBox(canvasFrame);
            inventoryBar.Disable();
            inventoryBar.KeyboardInputEnabled = false;
            inventoryBar.Height = barBoxSize + 2;
            inventoryBar.Width = barBoxSize * inventory.InventoryBar.Length + 1;
            inventoryBar.Y = canvasFrame.Height - inventoryBar.Height;
            inventoryBar.X = (canvasFrame.Width - inventoryBar.Width) / 2;
            barSpaces = new InventoryBarSpace[inventory.InventoryBar.Length];
            for (int i = 0; i < inventory.InventoryBar.Length; i++)
            {
                barSpaces[i] = new InventoryBarSpace(inventoryBar, messageProvider, inventory, this, barItems, barBoxSize);
                barSpaces[i].X = i * barBoxSize;
                barSpaces[i].Y = 1;
                barSpaces[i].Width = barBoxSize + 1;
                barSpaces[i].Height = barBoxSize + 1;
                barSpaces[i].DrawDebugOutlines = false;

                if (i == inventory.ActiveBarPosition)
                {
                    barSpaces[i].DrawDebugOutlines = true;
                    barSpaces[i].Children.ForEach(c => c.DrawDebugOutlines = false);
                }
            }

            window.SetSize (itemGridFrame.Width + itemInfoFrame.Width + 16,
                itemGridFrame.Height + toolbarFrameSize + 28);
            window.SetPosition ((canvasFrame.Width - window.Width) / 2,
                (canvasFrame.Height - window.Height - inventoryBar.Height) / 2);
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
            imagePanelHeight = itemGridFrame.Height / 3;
            imagePanel.Hide();

            items = new List<InventoryButton>();
            inventory.Items.ForEach((item, position) => {
                AddItem(item, position);
            });
        }

        public InventoryGUI (Application application, GameState state, Entity player, MessageProvider messageProvider)
        {
            this.application = application;
            this.player = player;
            this.messageProvider = messageProvider;
            this.gameState = state;
            ValidMessages = new[] {
                (int) MessageId.WindowResize,
                (int) MessageId.UpdateLocale,
                (int) MessageId.Input,
                (int) MessageId.Update,
                (int) MessageId.ActiveInventoryBarItemChanged
            };
            messageProvider += this;
            Localizer.Instance.CurrentLocale = LocaleEnum.de_DE;
        }

        public void AddItem (ItemComponent item, Vector2i position, bool insert = false)
        {
            if (insert)
                inventory.Insert(item, position);

            item.Player = player;

            var btn = new InventoryButton(itemGridFrame, messageProvider, inventory, item, this, position, BoxSize);

            btn.ToggledOn += (sender, arguments) => {
                if (toggledBtn != null && !toggledBtn.IsDisposed)
                {
                    toggledBtn.Item.Entity.GetComponent<ModelComponent>().Model.Enabled = false;
                    toggledBtn.ToggleState = false;
                }

                toggledBtn = btn;
                imagePanel.ImageName = btn.Item.ImageLocation;

                var max = toggledBtn.Item.Size.X > toggledBtn.Item.Size.Y ?
                    toggledBtn.Item.Size.X : toggledBtn.Item.Size.Y;

                int max_height = infoFrameSize;
                if ((infoFrameSize / max) * toggledBtn.Item.Size.Y > imagePanelHeight)
                    max_height = imagePanelHeight;

                imagePanel.Width = (max_height / max) * item.Size.X;
                imagePanel.Height = (max_height / max) * item.Size.Y;
                imagePanel.X = (infoFrameSize - imagePanel.Width) / 2;
                imagePanel.Show();
                setDescriptionLabel(Localizer.Instance.GetValueForName(btn.Item.Description));
                dropBtn.IsDisabled = false;
                useBtn.IsDisabled &= !toggledBtn.Item.ItemUsages.HasFlag (ItemUsage.Eatable);
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

            item.Location = ItemLocation.Inventory;

            if (MessageCreated != null)
                MessageCreated (new ItemCollectedMessage(item.Player, item, position));

            items.Add(btn);
        }

        public void AddItem (ItemComponent item)
        {
            if (inventory.Insert(item))
            {
                var pos = inventory.GetPositionOfItem(item);
                AddItem(item, pos);
            }
        }

        Label[] labels;

        internal void setDescriptionLabel(string text, int height = 4)
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

        Inventory inventory;
        internal ImagePanel imagePanel;
        Button dropBtn;
        Button useBtn;
        Button rotateBtn;
        InventorySpace[,] spaces;
        WindowControl window;
        Base itemGridFrame;
        Base itemInfoFrame;
        Base toolbarFrame;
        Base canvasFrame;
        List<InventoryButton> items;
        List<InventoryBarButton> barItems;
        TextBox inventoryBar;
        InventoryBarSpace[] barSpaces;
        Application application;
        Entity player;
        GameState gameState;
        MessageProvider messageProvider;

        #region IMessageConsumer implementation

        public void ConsumeMessage (IMessage msg)
        {
            if (msg.MessageId == (int) MessageId.WindowResize)
            {
                var wrm = msg as WindowResizeMessage;

                canvasFrame.Width = wrm.Width;
                canvasFrame.Height = wrm.Height;
                inventoryBar.Y = canvasFrame.Height - inventoryBar.Height;
                inventoryBar.X = (canvasFrame.Width - inventoryBar.Width) / 2;
                window.SetPosition ((canvasFrame.Width - window.Width) / 2,
                    (canvasFrame.Height - window.Height - inventoryBar.Height) / 2);
            }

            if (msg.MessageId == (int) MessageId.UpdateLocale)
            {
                window.Title = Localizer.Instance.GetValueForName("inventory");
                dropBtn.Text = Localizer.Instance.GetValueForName("drop");
                dropBtn.X = toolbarFrame.Width - dropBtn.Width;
                useBtn.Text = Localizer.Instance.GetValueForName("use");
                useBtn.X = dropBtn.X - useBtn.Width - 8;
                rotateBtn.Text = Localizer.Instance.GetValueForName("rotate");
                rotateBtn.X = useBtn.X - rotateBtn.Width - 8;
                if (toggledBtn != null)
                    setDescriptionLabel(Localizer.Instance.GetValueForName(toggledBtn.Item.Description));
            }

            if (msg.MessageId == (int) MessageId.ActiveInventoryBarItemChanged)
            {
                var aic = msg as ActiveInventoryBarItemChangedMessage;
                foreach (var s in barSpaces)
                {
                    s.DrawDebugOutlines = false;
                }

                barSpaces[aic.Position].DrawDebugOutlines = true;
                barSpaces[aic.Position].Children.ForEach(c => c.DrawDebugOutlines = false);

                foreach (var i in barItems)
                {
                    i.Item.Entity.GetComponent<ModelComponent>().Model.Enabled = false;
                    i.ToggleState = false;
                }

                var btn = barItems.FirstOrDefault(i => inventory.GetPositionOfBarItem(i.Item) == aic.Position);
                if (btn != null)
                {
                    btn.Item.Entity.GetComponent<ModelComponent>().Model.Enabled = true;
                    btn.ToggleState = true;
                }
            }

            if (msg.MessageId == (int) MessageId.Input)
            {
                var im = msg as InputMessage;

                if (im.IsActionPressed("drop"))
                {
                    Localizer.Instance.CurrentLocale =
                        Localizer.Instance.CurrentLocale == LocaleEnum.en_US ? LocaleEnum.de_DE : LocaleEnum.en_US;
                }

                if (im.IsActionPressed("inventory"))
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

                if (im.IsActionPressed("inventory_item1"))
                {
                    inventory.SetActiveBarItem(0);
                }
                if (im.IsActionPressed("inventory_item2"))
                {
                    inventory.SetActiveBarItem(1);
                }
                if (im.IsActionPressed("inventory_item3"))
                {
                    inventory.SetActiveBarItem(2);
                }
                if (im.IsActionPressed("inventory_item4"))
                {
                    inventory.SetActiveBarItem(3);
                }
                if (im.IsActionPressed("inventory_item5"))
                {
                    inventory.SetActiveBarItem(4);
                }
                if (im.IsActionPressed("inventory_item6"))
                {
                    inventory.SetActiveBarItem(5);
                }
                if (im.IsActionPressed("inventory_item7"))
                {
                    inventory.SetActiveBarItem(6);
                }
                if (im.IsActionPressed("inventory_item8"))
                {
                    inventory.SetActiveBarItem(7);
                }
                if (im.IsActionPressed("inventory_item9"))
                {
                    inventory.SetActiveBarItem(8);
                }

                if (im.IsActionPressed("close") && window.IsVisible)
                {
                    window.Hide();
                    application.Window.CaptureMouse();
                }

                if (im.IsMouseButtonPressed(MouseButton.LeftButton))
                {
                    if (mouseCollisionBody != null)
                    {
                        var entity = mouseCollisionBody.Tag as Entity;
                        if (entity == null)
                            return;
                        
                        var mapItem = entity.GetComponent<ItemComponent>();
                        if (mapItem != null && mapItem.Location != ItemLocation.Inventory)
                        {
                            AddItem(mapItem);
                            return;
                        }
                    }
                    else if (/*application.Window.IsMouseCaptured() && */inventory.ActiveBarItem != null)
                    {
                        MessageCreated(new ItemUseMessage(player, gameState.Scene, inventory.ActiveBarItem,
                            ItemUsage.Hitable | ItemUsage.Eatable));
                    }
                }

                if (im.IsMouseButtonPressed(MouseButton.RightButton))
                {
                    if (/*application.Window.IsMouseCaptured() && */inventory.ActiveBarItem != null)
                    {
                        var btn = barItems.FirstOrDefault(b => b.ToggleState);
                        var invBtn = items.FirstOrDefault(b => b.Item == btn.Item);
                        if (btn != null)
                        {
                            dropItem(invBtn, btn.Item, inventory);
                            MessageCreated(new ItemUseMessage(player, gameState.Scene, btn.Item,
                                ItemUsage.Throwable));
                        }
                    }
                }

                var pos = inventory.ActiveBarPosition - (int) im.MouseScroll.Y;
                while (pos < 0)
                    pos += inventory.InventoryBar.Length;

                while (pos >= inventory.InventoryBar.Length)
                    pos -= inventory.InventoryBar.Length;
                
                inventory.SetActiveBarItem((byte) pos);
            }

            if (msg.MessageId == (int) MessageId.Update)
            {
                var physics = gameState.PhysicsManager;
                var transform = player.GetComponent<TransformComponent>();
                var camera = gameState.Scene.CameraManager.ActiveCamera;

                if (camera == null)
                    return;

                physics.World.CollisionSystem.Raycast(
                    camera.Position.ToJitterVector(),
                    Vector3.Transform(Vector3.UnitZ, camera.Rotation).ToJitterVector() * 5,
                    new Jitter.Collision.RaycastCallback((rb, n, f) => {
                        var entity = rb.Tag as Entity;
                        if (entity != null && entity.HasComponent<ItemComponent>())
                        {
                            Logger.Log.AddLogEntry(LogLevel.Debug, "ItemSystem", "Raytrace: {0}", entity.Name);
                            return true;
                        }
                        return false;
                    }), out mouseCollisionBody, out mouseCollisionBodyNormal, out mouseCollisionBodyFraction
                );
            }
        }

        RigidBody mouseCollisionBody;
        JVector mouseCollisionBodyNormal;
        float mouseCollisionBodyFraction;

        internal void createMessage(IMessage msg)
        {
            if (MessageCreated != null)
                MessageCreated(msg);
        }

        public int[] ValidMessages { get; private set; }

        #endregion

        #region IMessageCreator implementation

        public event MessageEvent MessageCreated;

        #endregion
    }
}
