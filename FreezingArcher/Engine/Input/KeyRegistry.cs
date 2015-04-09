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
using Pencil.Gaming.MathUtils;
using System.Collections.Generic;
using FreezingArcher.Messaging;
using FreezingArcher.Configuration;
using Section = System.Collections.Generic.Dictionary<string, FreezingArcher.Configuration.Value>;
using Pencil.Gaming;
using FreezingArcher.Messaging.Interfaces;
using System;

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
        public static readonly KeyRegistry Instance = new KeyRegistry ();

        /// <summary>
        /// The key mapping.
        /// </summary>
        public Dictionary<string, Key> Keys = new Dictionary<string, Key> ();

        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Input.KeyRegistry"/> class.
        /// </summary>
        public KeyRegistry ()
        {
            ValidMessages = new int[] { (int) MessageId.ConfigFileValueSet };
            Keys.Add ("forward", Key.W);
            Keys.Add ("backward", Key.S);
            Keys.Add ("left", Key.A);
            Keys.Add ("right", Key.D);
            Keys.Add ("sneek", Key.LeftShift);
            Keys.Add ("run", Key.LeftControl);
            Keys.Add ("close", Key.Escape);
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
            Vector2 mouseMovement, Vector2 mouseScroll, double deltaTime)
        {
            string s;

            foreach (KeyboardInput i in keys)
                if (CachedConfig.TryGetValue (i.Key, out s))
                    i.KeyAction = s;

            return new InputMessage (keys, mouse, mouseMovement, mouseScroll, deltaTime);
        }

        Dictionary<Key, string> CachedConfig = new Dictionary<Key, string> ();

        /// <summary>
        /// Regenerate config cache.
        /// </summary>
        internal void RecacheConfig ()
        {
            CachedConfig.Clear ();
            Section section;
            ConfigManager.DefaultConfig.B.TryGetValue ("keymapping", out section);
            foreach (var pair in section)
                CachedConfig.Add (
                    (Key) Enum.Parse (typeof (Key), ConfigManager.Instance["freezing_archer"]
                        .GetString ("keymapping", pair.Key)), pair.Key);
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
