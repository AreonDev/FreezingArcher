//
//  MessageID.cs
//
//  Author:
//       Martin Koppehel <martin.koppehel@st.ovgu.de>
//       Willy Failla <>
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
namespace FreezingArcher.Messaging
{
    /// <summary>
    /// Enum which holds all known MessageId's 
    /// MessageId's must be unique for each type of IMessage
    /// </summary>
    public enum MessageId
    {
        /// <summary>
        /// This type tells the message providers to send all messages to the consumer.
        /// </summary>
        All = 0,
        /// <summary>
        /// Occurs when the locale is updated.
        /// </summary>
        UpdateLocale = 1,
        /// <summary>
        /// Occurs when the input manager is flushed.
        /// </summary>
        Input,
        /// <summary>
        /// Occurs when an update needs to be processed.
        /// </summary>
        Update,
        /// <summary>
        /// Occurs when Application.Run is called.
        /// </summary>
        Running,
        /// <summary>
        /// Occurs when a config file value is set.
        /// </summary>
        ConfigFileValueSet = 10,
        /// <summary>
        /// Occurs when a config file is saved.
        /// </summary>
        ConfigFileSaved,
        /// <summary>
        /// Occurs when an item is added to the config manager.
        /// </summary>
        ConfigManagerItemAdded,
        /// <summary>
        /// Occurs when an item is removed from the config manager.
        /// </summary>
        ConfigManagerItemRemoved,
        /// <summary>
        /// Occurs when a window is closed.
        /// </summary>
        WindowClose = 20,
        /// <summary>
        /// Occurs when a window gets an error.
        /// </summary>
        WindowError,
        /// <summary>
        /// Occurs when a window gets focus.
        /// </summary>
        WindowFocus,
        /// <summary>
        /// Occurs when a window is minimized.
        /// </summary>
        WindowMinimize,
        /// <summary>
        /// Occurs when the mouse enters or leaves the window.
        /// </summary>
        WindowMouseOver,
        /// <summary>
        /// Occurs when the window is moved.
        /// </summary>
        WindowMove,
        /// <summary>
        /// Occurs when the window is resized.
        /// </summary>
        WindowResize,
        /// <summary>
        /// Occurs when the mouse is captured or released from a window.
        /// </summary>
        MouseCapture,
        /// <summary>
        /// Occurs when a movement of an entity is requested on a MovementSystem.
        /// </summary>
        Movement,
        /// <summary>
        /// Occurs when a forward or backward movement along the view direction of an entity is requested on a
        /// MovementSystem.
        /// </summary>
        MoveStraight,
        /// <summary>
        /// Occurs when a left or right movement orthogonally to the view direction of an entity is requested on a
        /// MovementSystem.
        /// </summary>
        MoveSidewards,
        /// <summary>
        /// Occurs, when an up or down movement is requested of an entity on a MovementSystem.
        /// </summary>
        MoveVertical,
        /// <summary>
        /// Occurs when an item is used.
        /// </summary>
        ItemUse = 40,
        /// <summary>
        /// Occurs when an entity collects an item.
        /// </summary>
        ItemCollected,
        /// <summary>
        /// Occurs when an entity drops an item from its inventory.
        /// </summary>
        ItemDropped,
        /// <summary>
        /// Occurs when the usage of an item changes.
        /// </summary>
        ItemUsageChanged,
        /// <summary>
        /// Occurs when the health of an entity changes.
        /// </summary>
        HealthChanged,
        /// <summary>
        /// Occurs when an item is removed from the inventory.
        /// </summary>
        RemoveItemFromInventory,
        /// <summary>
        /// Occurs when an item is added to the inventory.
        /// </summary>
        ItemAddedToInventory,
        /// <summary>
        /// Occurs when an item from the inventory should be added to the inventory bar.
        /// </summary>
        ItemAddedToInventoryBar,
        /// <summary>
        /// Occurs when an item is removed from the inventory bar.
        /// </summary>
        ItemRemovedFromInventoryBar,
        /// <summary>
        /// Occurs when the active item in the inventory bar should be changed to a given position.
        /// </summary>
        ActiveInventoryBarItemChanged,
        /// <summary>
        /// Occurs when an inventory bar item moved to a different position.
        /// </summary>
        BarItemMoved,
    }
}
