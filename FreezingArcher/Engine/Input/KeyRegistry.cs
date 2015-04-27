﻿//
//  KeyRegistry.cs
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
using System.Collections.Generic;
using FreezingArcher.Configuration;
using FreezingArcher.Messaging;
using FreezingArcher.Messaging.Interfaces;
using Pencil.Gaming;
using Pencil.Gaming.MathUtils;
using Section = System.Collections.Generic.Dictionary<string, FreezingArcher.Configuration.Value>;

namespace FreezingArcher.Input
{
    /// <summary>
    /// Key registryto map keycodes to game actions.
    /// </summary>
    public sealed class KeyRegistry : IMessageConsumer
    {
        /// <summary>
        /// Global instance of the key registry.
        /// </summary>
        public static KeyRegistry Instance { get; internal set; }

        /// <summary>
        /// The key mapping.
        /// </summary>
        public static Dictionary<string, Key> Keys = new Dictionary<string, Key> ();

        static KeyRegistry ()
        {
            Keys.Add ("forward", Key.W);
            Keys.Add ("backward", Key.S);
            Keys.Add ("left", Key.A);
            Keys.Add ("right", Key.D);
            Keys.Add ("sneek", Key.LeftShift);
            Keys.Add ("run", Key.LeftControl);
            Keys.Add ("close", Key.Escape);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Input.KeyRegistry"/> class.
        /// </summary>
        public KeyRegistry ()
        {
            ValidMessages = new int[] { (int) MessageId.ConfigFileValueSet };
            RecacheConfig ();
        }

        /// <summary>
        /// Generates the input message.
        /// </summary>
        /// <returns>The input message.</returns>
        /// <param name="keys">Keys.</param>
        /// <param name="mouse">Mouse.</param>
        /// <param name="mouseMovement">Mouse movement.</param>
        /// <param name="mouseScroll">Mouse scroll.</param>
        /// <param name="deltaTime">Delta time.</param>
        public InputMessage GenerateInputMessage (List<KeyboardInput> keys, List<MouseInput> mouse,
            Vector2 mouseMovement, Vector2 mouseScroll, TimeSpan deltaTime)
        {
            string s;

            foreach (KeyboardInput i in keys)
                if (CachedConfig.TryGetValue (i.Key, out s))
                {
                    if (s != null)
                        i.KeyAction = s;
                }

            return new InputMessage (keys, mouse, mouseMovement, mouseScroll, deltaTime);
        }

        Dictionary<Key, string> CachedConfig = new Dictionary<Key, string> ();

        void RecacheConfig ()
        {
            CachedConfig.Clear ();
            Section section;
            ConfigManager.DefaultConfig.B.TryGetValue ("keymapping", out section);
            foreach (var pair in section)
                CachedConfig.Add (
                    (Key) Enum.Parse (typeof (Key), ConfigManager.Instance["freezing_archer"]
                        .GetString ("keymapping", pair.Key)), pair.Key);
        }

        /// <summary>
        /// Register an action with the given key.
        /// </summary>
        /// <param name="actionName">Action name.</param>
        /// <param name="key">Key.</param>
        public void RegisterKey (string actionName, Key key)
        {
            Keys.Add (actionName, key);
            Section keymap;
            ConfigManager.DefaultConfig.B.TryGetValue ("keymapping", out keymap);
            keymap.Add (actionName, new Value (key.ToString ()));
            CachedConfig.Add (key, actionName);
        }

        /// <summary>
        /// Update the specified action with the given key.
        /// </summary>
        /// <param name="actionName">Action name.</param>
        /// <param name="key">Key.</param>
        public void UpdateKey (string actionName, Key key)
        {
            Keys[actionName] = key;
            ConfigManager.Instance["freezing_archer"].SetString ("keymapping", actionName, key.ToString ());
            CachedConfig[key] = actionName;
        }

        /// <summary>
        /// Register or update the specified action with the given key.
        /// </summary>
        /// <param name="actionName">Action name.</param>
        /// <param name="key">Key.</param>
        public void RegisterOrUpdateKey (string actionName, Key key)
        {
            try
            {
                RegisterKey (actionName, key);
            }
            catch (ArgumentException)
            {
                UpdateKey (actionName, key);
            }
        }

        #region IMessageConsumer implementation

        /// <summary>
        /// Processes the invoming message
        /// </summary>
        /// <param name="msg">Message to process</param>
        public void ConsumeMessage(IMessage msg)
        {
            RecacheConfig ();
        }

        /// <summary>
        /// Gets the valid messages which can be used in the ConsumeMessage method
        /// </summary>
        /// <value>The valid messages</value>
        public int[] ValidMessages { get; private set; }

        #endregion
    }
}