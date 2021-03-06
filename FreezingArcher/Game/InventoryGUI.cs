﻿//
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
using FreezingArcher.Game.Maze;
using FreezingArcher.Renderer.Scene;
using FreezingArcher.Renderer.Compositor;

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
            this.MessageProvider = messageProvider;
        }

        readonly int boxSize;
        readonly Inventory inventory;
        readonly List<InventoryBarButton> barItems;
        public MessageProvider MessageProvider { get; private set;}
        readonly InventoryGUI inventoryGui;

        public void SwitchMessageProvider(MessageProvider mpv)
        {
            this.MessageProvider = mpv;
        }

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
                        var btn = new InventoryBarButton(Parent, MessageProvider, barItems, inventory, inventoryGui,
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

            MessageProvider = messageProvider;

            messageProvider += this;
        }

        public MessageProvider MessageProvider { get; private set;}

        public void SwitchMessageProvider(MessageProvider mpv)
        {
            MessageProvider.UnregisterMessageConsumer (this);

            this.MessageProvider = mpv;

            MessageProvider.RegisterMessageConsumer (this);
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

                if (m_Image != null && Height / Width != 0)
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
            this.MessageProvider = messageProvider;
        }

        readonly ItemComponent item;
        readonly int boxSize;
        readonly Inventory inventory;
        readonly List<InventoryBarButton> barItems;
        readonly ProgressBar usageProgress;
        public MessageProvider MessageProvider { get; private set;}
        readonly InventoryGUI inventoryGui;

        public void SwitchMessageProvider(MessageProvider mpv)
        {
            MessageProvider.UnregisterMessageConsumer (this);

            this.MessageProvider = mpv;

            MessageProvider.RegisterMessageConsumer (this);

            //Go through fucking shitty list
            foreach (InventoryBarButton ibb in barItems)
                ibb.SwitchMessageProvider (mpv);
        }

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
                        var btn = new InventoryBarButton(Parent, MessageProvider, barItems, inventory, inventoryGui,
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
        ImagePanel bla_unfug_crosshair;

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
        Gwen.ControlInternal.Text Item_Text;

        public void Init(Base parent, Inventory inventory)
        {
            this.inventory = inventory;

            Item_Text = new Gwen.ControlInternal.Text (parent);
            Item_Text.Font = new Gwen.Font (application.RendererContext.GwenRenderer);
            Item_Text.Y = 5;
            Item_Text.Font.Size = 15;

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


            bla_unfug_crosshair = new ImagePanel (canvasFrame);
            bla_unfug_crosshair.SetSize (16, 16);
            bla_unfug_crosshair.ImageName = "Content/crosshair.png";
            bla_unfug_crosshair.SetPosition ((canvasFrame.Width / 2.0f) - (bla_unfug_crosshair.Width / 2.0f), 
                (canvasFrame.Height / 2.0f) - (bla_unfug_crosshair.Width / 2.0f));
            bla_unfug_crosshair.BringToFront ();

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
                        MessageCreated(new ItemUseMessage(player, GameState.Scene, toggledBtn.Item, ItemUsage.Eatable));
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
                barSpaces[i] = new InventoryBarSpace(inventoryBar, MessageProvider, inventory, this, barItems, barBoxSize);
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

        public void CreateInitialFlashlight ()
        {
            var flashlight = Inventory.CreateNewItem (GameState.MessageProxy, GameState,
                "flashlight_initial",
                MazeGenerator.ItemTemplates[1].ImageLocation,
                MazeGenerator.ItemTemplates[1].Description,
                MazeGenerator.ItemTemplates[1].ModelPath,
                MazeGenerator.ItemTemplates[1].Size,
                MazeGenerator.ItemTemplates[1].PositionOffset,
                MazeGenerator.ItemTemplates[1].Rotation,
                MazeGenerator.ItemTemplates[1].Shape,
                ItemLocation.Inventory,
                MazeGenerator.ItemTemplates[1].AttackClasses,
                MazeGenerator.ItemTemplates[1].ItemUsages,
                MazeGenerator.ItemTemplates[1].Protection,
                MazeGenerator.ItemTemplates[1].PhysicsMaterial,
                MazeGenerator.ItemTemplates[1].Mass,
                MazeGenerator.ItemTemplates[1].HealthDelta,
                MazeGenerator.ItemTemplates[1].UsageDeltaPerUsage,
                MazeGenerator.ItemTemplates[1].AttackStrength,
                MazeGenerator.ItemTemplates[1].ThrowPower,
                MazeGenerator.ItemTemplates[1].Usage
            );

            var item_body = flashlight.Entity.GetComponent<PhysicsComponent>();
            var item_model = flashlight.Entity.GetComponent<ModelComponent>();
            item_body.RigidBody.Position = flashlight.Entity.GetComponent<TransformComponent>().Position.ToJitterVector();
            item_model.Model.Position = flashlight.Entity.GetComponent<TransformComponent>().Position;
            flashlight.Entity.AddSystem<LightSystem> ();
            var light = flashlight.Entity.GetComponent<LightComponent> ().Light;
            light = new Light (LightType.SpotLight);
            light.Color = new Color4 (0.1f, 0.1f, 0.1f, 1.0f);
            light.PointLightLinearAttenuation = 0.01f;
            light.SpotLightConeAngle = MathHelper.ToRadians (30f);
            light.On = false;

            flashlight.Entity.GetComponent<LightComponent> ().Light = light;

            GameState.Scene.AddLight (light);

            AddItem (flashlight);
        }

        public InventoryGUI (Application application, GameState state, Entity player, MessageProvider messageProvider,
            CompositorWarpingNode warpingNode)
        {
            this.application = application;
            this.player = player;
            this.MessageProvider = messageProvider;
            this.GameState = state;
            this.warpingNode = warpingNode;
            ValidMessages = new[] {
                (int) MessageId.WindowResize,
                (int) MessageId.UpdateLocale,
                (int) MessageId.Input,
                (int) MessageId.Update,
                (int) MessageId.ActiveInventoryBarItemChanged
            };
            messageProvider += this;
        }

        CompositorWarpingNode warpingNode;

        public void AddItem (ItemComponent item, Vector2i position, bool insert = false, bool no_message = false)
        {
            if (insert)
                inventory.Insert(item, position);

            item.Player = player;

            var btn = new InventoryButton(itemGridFrame, MessageProvider, inventory, item, this, position, BoxSize);

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

            if (MessageCreated != null && !no_message)
                MessageCreated (new ItemCollectedMessage(item.Player, item, position));

            items.Add(btn);
        }

        public void AddItem (ItemComponent item, bool no_message = false)
        {
            if (inventory.Insert(item))
            {
                var pos = inventory.GetPositionOfItem(item);
                AddItem(item, pos, no_message: no_message);
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
        public GameState GameState { get; private set;}
        public MessageProvider MessageProvider { get; private set;}

        public void SwitchMessageProvider(MessageProvider mpv)
        {
            MessageProvider.UnregisterMessageConsumer (this);

            this.MessageProvider = mpv;

            MessageProvider.RegisterMessageConsumer (this);
        }

        public void SwitchGameState(GameState source, GameState dest, FreezingArcher.Content.Game game)
        {
            GameState = dest;

            inventory.SwitchItemsToGameState (source, dest, game);
        }

        #region IMessageConsumer implementation

        bool maze_left = false;

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

                Item_Text.X = application.Window.Size.X - Item_Text.Width - 5;

                bla_unfug_crosshair.SetPosition ((wrm.Width / 2.0f) - (bla_unfug_crosshair.Width / 2.0f), 
                    (wrm.Height / 2.0f) - (bla_unfug_crosshair.Width / 2.0f));
                bla_unfug_crosshair.BringToFront ();
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

                if (mouseCollisionBody != null)
                {
                    var entity = (mouseCollisionBody.Tag as Entity);
                    if (entity != null)
                    {
                        var name = entity.Name;
                        int idx = name.IndexOf ('.');
                        if (idx >= 0)
                        {
                            name = name.Substring (0, idx);
                            Item_Text.String = Localizer.Instance.GetValueForName (name);
                            Item_Text.X = application.Window.Size.X - Item_Text.Width - 5;
                        }
                    }
                }
                else
                {
                    Item_Text.String = "";
                    Item_Text.X = application.Window.Size.X - Item_Text.Width - 5;
                }
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

                #if DEBUG
                if (im.IsActionPressed("drop"))
                {
                    Localizer.Instance.CurrentLocale =
                        Localizer.Instance.CurrentLocale == LocaleEnum.en_US ? LocaleEnum.de_DE : LocaleEnum.en_US;
                }
                #endif

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

                if (im.IsActionPressed("pause") && window.IsVisible)
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

                        if (entity.Name.Contains ("exit") && mouseCollisionBodyFraction < 1)
                        {
                            if (!maze_left)
                            {
                                warpingNode.Stop();
                                GameState endstate = application.Game.GetGameState ("endscreen_state");
                                endstate.Scene = application.Game.CurrentGameState.Scene;

                                application.Game.SwitchToGameState ("endscreen_state");

                                if (MessageCreated != null)
                                    MessageCreated (new GameEndedMessage ());

                                maze_left = true;

                                Logger.Log.AddLogEntry (LogLevel.Info, "InventoryGUI", "Leaving maze...");
                                return;
                            }
                        }

                        var mapItem = entity.GetComponent<ItemComponent>();
                        if (mapItem != null && mapItem.Location != ItemLocation.Inventory && mouseCollisionBodyFraction < 5)
                        {
                            AddItem(mapItem);
                            return;
                        }
                    }
                    else if (application.Window.IsMouseCaptured() && inventory.ActiveBarItem != null)
                    {
                        if (inventory.ActiveBarItem.Entity.HasComponent<LightComponent>())
                        {
                            var light = inventory.ActiveBarItem.Entity.GetComponent<LightComponent>().Light;
                            light.On = !light.On;
                            MessageCreated (new FlashlightToggledMessage(light.On));
                        }
                        else
                        {
                            MessageCreated(new ItemUseMessage(player, GameState.Scene, inventory.ActiveBarItem,
                                ItemUsage.Hitable | ItemUsage.Eatable));
                        }
                    }
                    else if (application.Window.IsMouseCaptured() && inventory.ActiveBarItem == null && GameState.Scene.CameraManager.ActiveCamera != null)
                    {
                        RigidBody rb;
                        JVector n;
                        float f;
                        GameState.PhysicsManager.World.CollisionSystem.Raycast(
                            player.GetComponent<TransformComponent>().Position.ToJitterVector(),
                            Vector3.Transform(Vector3.UnitZ, GameState.Scene.CameraManager.ActiveCamera.Rotation).ToJitterVector() * 5,
                            new Jitter.Collision.RaycastCallback((body, normal, fraction) => {
                                var entity = body.Tag as Entity;
                                return entity != null && entity.Name.Contains ("wall") && fraction < 1 && !entity.GetComponent<WallComponent> ().IsEdge;
                            }),
                            out rb, out n, out f);
                        if (rb != null)
                        {
                            var entity = rb.Tag as Entity;

                            if (entity != null)
                            {
                                var model = entity.GetComponent<ModelComponent>().Model;
                                var health = entity.GetComponent<HealthComponent>();
                                var wallcomp = entity.GetComponent<WallComponent>();
                                var temp_health = health.Health - 1f;
                                health.Health = temp_health < 0 ? 0 : temp_health;
                                var tmp = (health.MaximumHealth - health.Health) / health.MaximumHealth;
                                var tmp_pos = model.Position;
                                tmp_pos.Y = 7.5f * tmp + 0.25f;

                                if (wallcomp.IsOverworld)
                                    tmp_pos.Y *= -2;

                                model.Position = tmp_pos;
                                var rbpos = rb.Position;
                                rbpos.Y = tmp_pos.Y + 8;
                                rb.Position = rbpos;
                                wallcomp.IsMoveable = false;
                                return;
                            }
                        }
                    }
                }

                if (im.IsMouseButtonPressed(MouseButton.RightButton))
                {
                    if (application.Window.IsMouseCaptured() && inventory.ActiveBarItem != null)
                    {
                        var btn = barItems.FirstOrDefault(b => b.ToggleState);
                        var invBtn = items.FirstOrDefault(b => b.Item == btn.Item);
                        if (btn != null)
                        {
                            dropItem(invBtn, btn.Item, inventory);
                            MessageCreated(new ItemUseMessage(player, GameState.Scene, btn.Item,
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
                var physics = GameState.PhysicsManager;
                var transform = player.GetComponent<TransformComponent>();
                var camera = GameState.Scene.CameraManager.ActiveCamera;

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
                        if (entity != null && entity.Name.Contains("exit"))
                        {
                            return true;
                        }
                        return false;
                    }), out mouseCollisionBody, out mouseCollisionBodyNormal, out mouseCollisionBodyFraction
                );

                if (mouseCollisionBody != null)
                {
                    var entity = (mouseCollisionBody.Tag as Entity);
                    if (entity != null)
                    {
                        var name = entity.Name;
                        int idx = name.IndexOf ('.');
                        if (idx >= 0)
                        {
                            name = name.Substring (0, idx);
                            Item_Text.String = Localizer.Instance.GetValueForName (name);
                            Item_Text.X = application.Window.Size.X - Item_Text.Width - 5;
                        }
                    }
                }
                else
                {
                    Item_Text.String = "";
                    Item_Text.X = application.Window.Size.X - Item_Text.Width - 5;
                }
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
