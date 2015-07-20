//
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
            Keys.Add ("up", Key.E);
            Keys.Add ("down", Key.Q);
            Keys.Add ("inventory", Key.I);
            Keys.Add ("drop", Key.O);
            Keys.Add ("frame", Key.F); 
            Keys.Add ("sneek", Key.LeftShift);
            Keys.Add ("run", Key.LeftControl);
            Keys.Add ("close", Key.Escape);
            Keys.Add ("jump", Key.Space);
            Keys.Add ("camera", Key.C);
            Keys.Add ("fullscreen", Key.F11);
            Keys.Add ("capture_mouse", Key.F1);
            Keys.Add ("save", Key.F2);
            Keys.Add ("inventory_item0", Key.Zero);
            Keys.Add ("inventory_item1", Key.One);
            Keys.Add ("inventory_item2", Key.Two);
            Keys.Add ("inventory_item3", Key.Three);
            Keys.Add ("inventory_item4", Key.Four);
            Keys.Add ("inventory_item5", Key.Five);
            Keys.Add ("inventory_item6", Key.Six);
            Keys.Add ("inventory_item7", Key.Seven);
            Keys.Add ("inventory_item8", Key.Eight);
            Keys.Add ("inventory_item9", Key.Nine);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Input.KeyRegistry"/> class.
        /// </summary>
        /// <param name="messageProvider">The message manager.</param>
        internal KeyRegistry (MessageProvider messageProvider)
        {
            ValidMessages = new int[] { (int) MessageId.ConfigFileValueSet };
            messageProvider += this;
            RecacheConfig ();
        }

        /// <summary>
        /// Generates the input message.
        /// </summary>
        /// <returns>The input message.</returns>
        /// <param name="keys">Keys.</param>
        /// <param name="mouse">Mouse.</param>
        /// <param name="mouseMovement">Mouse movement.</param>
        /// <param name="mousePosition">Mouse position.</param>
        /// <param name="mouseScroll">Mouse scroll.</param>
        /// <param name="deltaTime">Delta time.</param>
        internal InputMessage GenerateInputMessage (List<KeyboardInput> keys, List<MouseInput> mouse,
            Vector2 mouseMovement, Vector2 mousePosition, Vector2 mouseScroll, TimeSpan deltaTime, FreezingArcher.Core.Application app)
        {
            string s;

            lock(keys)
            {
                foreach (KeyboardInput i in keys)
                {
                    if (i == null)
                        continue;
                    
                    if (CachedConfig.TryGetValue (i.Key, out s))
                    {
                        if (s != null)
                           i.KeyAction = s;
                    }
                }
            }

            return new InputMessage (keys, mouse, mouseMovement, mousePosition, mouseScroll, deltaTime, app);
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
