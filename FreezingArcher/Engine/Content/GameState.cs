//
//  GameState.cs
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
using FreezingArcher.Renderer.Scene;
using FreezingArcher.Core.Interfaces;
using FreezingArcher.Messaging.Interfaces;
using FreezingArcher.Messaging;
using FreezingArcher.Core;

namespace FreezingArcher.Content
{
    /// <summary>
    /// Game state class. This class represents a single game state such as a level or a menu.
    /// </summary>
    public sealed class GameState : IManageable, IMessageConsumer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Content.GameState"/> class.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="env">Environment.</param>
        /// <param name="messageProvider">Message Manager.</param>
        public GameState(string name, Environment env, MessageProvider messageProvider)
        {
            Name = name;
            Environment = env;
            PhysicsManager = new PhysicsManager();
            ValidMessages = new[] { (int) MessageId.Update };
            MessageProxy = new MessageProxy(messageProvider);
        }

        /// <summary>
        /// Gets or sets the environment.
        /// </summary>
        /// <value>The environment.</value>
        public Environment Environment { get; set; }

        /// <summary>
        /// Gets the message proxy.
        /// </summary>
        /// <value>The message proxy.</value>
        public MessageProxy MessageProxy { get; private set; }

        /// <summary>
        /// Gets or sets the scene.
        /// </summary>
        /// <value>The scene.</value>
        public CoreScene Scene { get; set; }

        /// <summary>
        /// Gets the physics manager.
        /// </summary>
        /// <value>The physics manager.</value>
        public PhysicsManager PhysicsManager { get; private set; }

        // TODO add transition effects

        #region IManageable implementation

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        #endregion

        #region IMessageConsumer implementation

        /// <summary>
        /// Processes the incoming message
        /// </summary>
        /// <param name="msg">Message to process</param>
        public void ConsumeMessage(IMessage msg)
        {
            UpdateMessage um = msg as UpdateMessage;

            if (um != null)
            {
                PhysicsManager.Update(um.TimeStamp);
            }
        }

        /// <summary>
        /// Gets the valid messages which can be used in the ConsumeMessage method
        /// </summary>
        /// <value>The valid messages</value>
        public int[] ValidMessages { get; private set; }

        #endregion

        /// <summary>
        /// Destroy this instance.
        /// </summary>
        public void Destroy()
        {
            PhysicsManager.Destroy();
        }
    }
}
