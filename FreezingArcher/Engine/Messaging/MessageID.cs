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
        /// Occurs when the locale is updated.
        /// </summary>
        UpdateLocale = 1,
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
        WindowCloseMessage = 20,
        /// <summary>
        /// Occurs when a window gets an error.
        /// </summary>
        WindowErrorMessage,
        /// <summary>
        /// Occurs when a window gets focus.
        /// </summary>
        WindowFocusMessage,
        /// <summary>
        /// Occurs when a window is minimized.
        /// </summary>
        WindowMinimizeMessage,
        /// <summary>
        /// Occurs when the mouse enters or leaves the window.
        /// </summary>
        WindowMouseOverMessage,
        /// <summary>
        /// Occurs when the window is moved.
        /// </summary>
        WindowMoveMessage,
        /// <summary>
        /// Occurs when the window is resized.
        /// </summary>
        WindowResizeMessage
    }
}